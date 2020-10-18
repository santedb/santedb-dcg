/*
 * Portions Copyright 2015-2019 Mohawk College of Applied Arts and Technology
 * Portions Copyright 2019-2019 SanteSuite Contributors (See NOTICE)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: Justin Fyfe
 * Date: 2019-8-8
 */
using MohawkCollege.Util.Console.Parameters;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Model.Security;
using SanteDB.DisconnectedClient.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SanteDB.Core.Configuration;
using SanteDB.Core;
using SanteDB.Core.Applets.Services;
using SanteDB.Core.Services.Impl;
using SanteDB.DisconnectedClient.Security;
using SanteDB.DisconnectedClient.Backup;
using SanteDB.DisconnectedClient.Configuration;
using SanteDB.Core.Services;

namespace SanteDB.Dcg
{
    /// <summary>
    /// Main SanteDB DCG Disconnected Gateway
    /// </summary>
    static class Program
    {

        /// <summary>
        /// Quit event
        /// </summary>
        static ManualResetEvent m_quitEvent = new ManualResetEvent(false);

        /// <summary>
        /// Prompt for a masked password prompt
        /// </summary>
        internal static string PasswordPrompt(string prompt)
        {
            Console.Write(prompt);

            var c = (ConsoleKey)0;
            StringBuilder passwd = new StringBuilder();
            while (c != ConsoleKey.Enter)
            {
                var ki = Console.ReadKey();
                c = ki.Key;

                if (c == ConsoleKey.Backspace)
                {
                    if (passwd.Length > 0)
                    {
                        passwd = passwd.Remove(passwd.Length - 1, 1);
                        Console.Write(" \b");
                    }
                    else
                        Console.CursorLeft = Console.CursorLeft + 1;
                }
                else if (c == ConsoleKey.Escape)
                    return String.Empty;
                else if (c != ConsoleKey.Enter)
                {
                    passwd.Append(ki.KeyChar);
                    Console.Write("\b*");
                }
            }
            Console.WriteLine();
            return passwd.ToString();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(String[] args)
        {
            // Output main header
            var parser = new ParameterParser<ConsoleParameters>();
            var parms = parser.Parse(args);
            parms.InstanceName = String.IsNullOrEmpty(parms.InstanceName) ? "default" : parms.InstanceName;

            // Output copyright info
            var entryAsm = Assembly.GetEntryAssembly();
            Console.WriteLine("SanteDB Disconnected Gateway (SanteDB-DCG) {0} ({1})", entryAsm.GetName().Version, entryAsm.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
            Console.WriteLine("{0}", entryAsm.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright);
            Console.WriteLine("Complete Copyright information available at http://github.com/santedb/santedb-www");

            // Parameters to force load?
            if (parms.Force)
                foreach (var itm in Directory.GetFiles(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "*.dll"))
                {
                    try
                    {
                        var asm = Assembly.LoadFile(itm);
                        Console.WriteLine("Force Loaded {0}", asm.FullName);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERR: Cannot load {0} due to {1}", itm, e.Message);
                    }
                }

            Console.CancelKeyPress += (o, e) =>
            {
                m_quitEvent.Set();
                e.Cancel = true;
            };


            AppDomain.CurrentDomain.AssemblyResolve += (o, e) =>
            {
                string pAsmName = e.Name;
                if (pAsmName.Contains(","))
                    pAsmName = pAsmName.Substring(0, pAsmName.IndexOf(","));

                var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => e.Name == a.FullName) ??
                    AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => pAsmName == a.GetName().Name);
                return asm;
            };


            try
            {

                // Detect platform
                if (System.Environment.OSVersion.Platform != PlatformID.Win32NT)
                    Trace.TraceWarning("Not running on WindowsNT, some features may not function correctly");
                else if (!EventLog.SourceExists("SanteDB Gateway Process"))
                    EventLog.CreateEventSource("SanteDB Gateway Process", "santedb-dcg");

                // Security Application Information
                var applicationIdentity = new SecurityApplication()
                {
                    Key = Guid.Parse("feeca9f3-805e-4be9-a5c7-30e6e495939b"),
                    ApplicationSecret = parms.ApplicationSecret ?? "SDB$$DEFAULT$$APPSECRET",
                    Name = parms.ApplicationName ?? "org.santedb.disconnected_client.gateway"
                };

                // Setup basic parameters
                String[] directory = {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SanteDB", parms.InstanceName),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SanteDB", parms.InstanceName)
                };

                foreach (var dir in directory)
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                // Token validator
                TokenValidationManager.SymmetricKeyValidationCallback += (o, k, i) =>
                {
                    Trace.TraceError("Trust issuer {0} failed", i);
                    return false;
                };
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, error) =>
                {
                    if (certificate == null || chain == null)
                        return false;
                    else
                    {
                        if (chain.ChainStatus.Length > 0 || error != SslPolicyErrors.None)
                        {
                            Trace.TraceWarning("The remote certificate is not trusted. The error was {0}. The certificate is: \r\n{1}", error, certificate.Subject);
                            return false;
                        }
                        return true;
                    }
                };


                if (parms.ShowHelp)
                    parser.WriteHelp(Console.Out);
                else if (parms.Reset)
                {
                    var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SanteDB", parms.InstanceName);
                    var cData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SanteDB", parms.InstanceName);
                    if (Directory.Exists(appData)) Directory.Delete(cData, true);
                    if (Directory.Exists(appData)) Directory.Delete(appData, true);
                    Console.WriteLine("Environment Reset Successful");
                    return;
                }
                else if (!String.IsNullOrEmpty(parms.BackupFile))
                {

                    DcApplicationContext.StartRestore(new ConsoleDialogProvider(), $"dcg-{parms.InstanceName}", applicationIdentity, SanteDBHostType.Test);

                    // Attempt to unpack
                    try
                    {
                        string serviceName = $"sdb-dcg-{parms.InstanceName}";
                        if (ServiceTools.ServiceInstaller.ServiceIsInstalled(serviceName))
                            try
                            {
                                ServiceTools.ServiceInstaller.StopService(serviceName);
                            }
                            catch (Exception e) { Console.Write("Could not stop service: {0}", e.Message); }

                        var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SanteDB", parms.InstanceName);
                        var cData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SanteDB", parms.InstanceName);
                        if (Directory.Exists(appData)) Directory.Delete(cData, true);
                        if (Directory.Exists(appData)) Directory.Delete(appData, true);

                        var password = PasswordPrompt($"Enter Password for {Path.GetFileNameWithoutExtension(parms.BackupFile)}:");

                        var restoreDirectory = DcApplicationContext.Current.GetService<IConfigurationPersister>().ApplicationDataDirectory;
                        if (parms.SystemRestore && Environment.OSVersion.Platform == PlatformID.Win32NT)
                        {
                            Console.WriteLine("SERIOUS SECURITY WARNING --> IF YOU RESTORE THIS CONFIGURATION ENSURE THE ORIGINAL MACHINE THAT GENERATED IT DOES NOT CONNECT TO THE");
                            Console.WriteLine("                             CENTRAL SERVER. ENSURE THE ORIGINAL IS OFFLINE BEFORE PERFORMING THIS PROCEDURE");
                            Console.Write("TO CONTINUE TYPE \"OK\":");
                            if (Console.ReadLine() != "OK")
                                return;
                            restoreDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "config", "systemprofile", "appdata", "local", "santedb", $"dcg-{parms.InstanceName}");
                        }
                        Console.WriteLine("Performing restore to directory {0}...", restoreDirectory);
                        new DefaultBackupService().RestoreFiles(parms.BackupFile, password, restoreDirectory);

                        // Move the config
                        if (parms.SystemRestore && Environment.OSVersion.Platform == PlatformID.Win32NT)
                        {

                            var configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "config", "systemprofile", "appdata", "roaming", "santedb", $"dcg-{parms.InstanceName}", "santedb.config");
                            if (!File.Exists(configFile))
                            {
                                if (!Directory.Exists(Path.GetDirectoryName(configFile)))
                                    Directory.CreateDirectory(Path.GetDirectoryName(configFile));
                                using (var fs = File.Create(configFile))
                                {
                                    // The SID for this user 
                                    ApplicationServiceContext.Current.GetService<IConfigurationManager>().Configuration.Save(fs);
                                }
                            }
                            // Now copy the source backup to the restore directory
                            Directory.CreateDirectory(Path.Combine(restoreDirectory, "restore"));
                            File.Copy(parms.BackupFile, Path.Combine(restoreDirectory, "restore", Path.GetFileName(parms.BackupFile)));

                            // Restart 
                            Console.WriteLine("Starting SanteDB Service");
                            if (ServiceTools.ServiceInstaller.ServiceIsInstalled(serviceName))
                                try
                                {
                                    ServiceTools.ServiceInstaller.StartService(serviceName);
                                }
                                catch (Exception e) { Console.Write("Could not stop service: {0}", e.Message); }
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error restoring {Path.GetFileNameWithoutExtension(parms.BackupFile)} - {e}");
                    }
                }
                else if (parms.ConsoleMode)
                {
#if DEBUG
                    Tracer.AddWriter(new DisconnectedClient.Diagnostics.LogTraceWriter(System.Diagnostics.Tracing.EventLevel.LogAlways, "SanteDB.data"), System.Diagnostics.Tracing.EventLevel.LogAlways);
#else
                    Tracer.AddWriter(new DisconnectedClient.Diagnostics.LogTraceWriter(System.Diagnostics.Tracing.EventLevel.LogAlways, "SanteDB.data"), System.Diagnostics.Tracing.EventLevel.LogAlways);
#endif

                    SanteDB.DisconnectedClient.ApplicationContext.ProgressChanged += (o, e) =>
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(">>> PROGRESS >>> {0} : {1:#0%}", e.ProgressText, e.Progress);
                        Console.ResetColor();
                    };


                    if (!DcApplicationContext.StartContext(new ConsoleDialogProvider(), $"dcg-{parms.InstanceName}", applicationIdentity, SanteDBHostType.Gateway))
                        DcApplicationContext.StartTemporary(new ConsoleDialogProvider(), $"dcg-{parms.InstanceName}", applicationIdentity, SanteDBHostType.Gateway);
                    DcApplicationContext.Current.Configuration.GetSection<ApplicationServiceContextConfigurationSection>().AppSettings.RemoveAll(o => o.Key == "http.bypassMagic");
                    DcApplicationContext.Current.Configuration.GetSection<ApplicationServiceContextConfigurationSection>().AppSettings.Add(new AppSettingKeyValuePair() { Key = "http.bypassMagic", Value = DcApplicationContext.Current.ExecutionUuid.ToString() });

                    if (!parms.Forever)
                    {
                        Console.WriteLine("Press [Enter] key to close...");
                        Console.ReadLine();
                    }
                    else
                        m_quitEvent.WaitOne();
                    DcApplicationContext.Current.Stop();
                }

                else if (parms.Install)
                {
                    string serviceName = $"sdb-dcg-{parms.InstanceName}";
                    if (!ServiceTools.ServiceInstaller.ServiceIsInstalled(serviceName))
                    {
                        String argList = String.Empty;
                        if (!String.IsNullOrEmpty(parms.ApplicationName))
                            argList += $" --appname=\"{parms.ApplicationName}\"";
                        if (!String.IsNullOrEmpty(parms.ApplicationSecret))
                            argList += $" --appsecret=\"{parms.ApplicationSecret}\"";

                        ServiceTools.ServiceInstaller.Install(
                            serviceName, $"SanteDB DCG ({parms.InstanceName})",
                            $"{entryAsm.Location} --name=\"{parms.InstanceName}\" {argList}",
                            null, null, ServiceTools.ServiceBootFlag.AutoStart);
                    }
                    else
                        throw new InvalidOperationException("Service instance already installed");
                }
                else if (parms.Restart)
                {
                    string serviceName = $"sdb-dcg-{parms.InstanceName}";
                    if (ServiceTools.ServiceInstaller.ServiceIsInstalled(serviceName))
                    {
                        try
                        {
                            ServiceTools.ServiceInstaller.StopService(serviceName);
                        }
                        catch (Exception e) { Console.Write("Could not start service: {0}", e.Message); }
                        try { ServiceTools.ServiceInstaller.StartService(serviceName); }
                        catch (Exception e) { Console.Write("Could not start service: {0}", e.Message); }
                    }
                    else
                        throw new InvalidOperationException("Service instance not installed");
                }
                else if (parms.Uninstall)
                {
                    string serviceName = $"sdb-dcg-{parms.InstanceName}";
                    if (ServiceTools.ServiceInstaller.ServiceIsInstalled(serviceName))
                        ServiceTools.ServiceInstaller.Uninstall(serviceName);
                    else
                        throw new InvalidOperationException("Service instance not installed");
                }
                else
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                        new SanteDbService(parms.InstanceName, applicationIdentity)
                    };
                    ServiceBase.Run(ServicesToRun);
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.TraceError("011 899 981 199 911 9725 3!!! {0}", e.ToString());
                Console.WriteLine("011 899 981 199 911 9725 3!!! {0}", e.ToString());

#else
                Trace.TraceError("Error encountered: {0}. Will terminate", e.Message);
                EventLog.WriteEntry("SanteDB Gateway Process", $"Fatal service error: {e}", EventLogEntryType.Error, 911);
#endif
                Environment.Exit(911);
            }
        }
    }
}
