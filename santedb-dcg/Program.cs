/*
 * Portions Copyright 2015-2019 Mohawk College of Applied Arts and Technology
 * Portions Copyright 2019-2023 SanteSuite Contributors (See NOTICE)
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
using SanteDB.Core.Services;
using System.Diagnostics.Tracing;
using SanteDB.Client.Rest;
using SanteDB.Core.Data.Backup;
using Mono.Unix.Native;
using Mono.Unix;
using SanteDB.Core.Diagnostics.Tracing;
using SanteDB.Client.Batteries;
using SanteDB.Client.Configuration.Upstream;
using SanteDB.Client.Configuration;
using DocumentFormat.OpenXml.Wordprocessing;

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
            Console.WriteLine("Complete Copyright information available at http://github.com/santedb/santedb-dcg");

            // Parameters to force load?
            var dllFiles = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Sante*.dll");
            if (parms.Force)
            {
                dllFiles = dllFiles.Union(Directory.GetFiles(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "*.dll")).ToArray();
            }

            foreach (var itm in dllFiles)
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
                try
                {
                    if (System.Environment.OSVersion.Platform != PlatformID.Win32NT)
                        Trace.TraceWarning("Not running on WindowsNT, some features may not function correctly");
                    else if (!EventLog.SourceExists("SanteDB"))
                        EventLog.CreateEventSource("SanteDB", "santedb-dcg");
                }
                catch(Exception e)
                {
                    Trace.TraceWarning("CAnnot detect Windows Event Log {0}", e.ToHumanReadableString());
                }

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

                // Different binding port?
                if (String.IsNullOrEmpty(parms.BaseUrl))
                {
                    parms.BaseUrl = "http://127.0.0.1:9200";
                }

                AppDomain.CurrentDomain.SetData(RestServiceInitialConfigurationProvider.BINDING_BASE_DATA, parms.BaseUrl);
                Trace.TraceInformation("Binding to {0}", AppDomain.CurrentDomain.GetData(RestServiceInitialConfigurationProvider.BINDING_BASE_DATA));

                if (parms.ShowHelp)
                    parser.WriteHelp(Console.Out);
                else if (parms.Reset)
                {
                    var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SanteDB", "dcg", parms.InstanceName);
                    var cData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SanteDB", "dcg", parms.InstanceName);
                    if (Directory.Exists(appData)) Directory.Delete(cData, true);
                    if (Directory.Exists(appData)) Directory.Delete(appData, true);
                    Console.WriteLine("Environment Reset Successful");
                    return;
                }
                else if (parms.ConsoleMode)
                {
#if DEBUG
                    Tracer.AddWriter(new ConsoleTraceWriter(EventLevel.Informational, "SanteDB.Data", new Dictionary<String, EventLevel>()), EventLevel.Informational);
#else
                    Tracer.AddWriter(new ConsoleTraceWriter(EventLevel.Warning, "SanteDB.Data", new Dictionary<String, EventLevel>()), EventLevel.Warning);
#endif
                    Trace.Listeners.Add(new ConsoleTraceListener());

                    var context = CreateContext(parms);

                    ServiceUtil.Start(Guid.NewGuid(), context);

                    if (!parms.Forever)
                    {
                        Console.WriteLine("Press [Enter] key to close...");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Will run in nohup daemon mode...");
                        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                        {
                            // Wait until cancel key is pressed
                            var mre = new ManualResetEventSlim(false);
                            // Application requested a restart
                            ApplicationServiceContext.Current.Stopped += (o, e) => mre.Set();
                            // User requested a stop
                            Console.CancelKeyPress += (o, e) => mre.Set();
                            mre.Wait();
                        }
                        else
                        {  // running on unix
                           // Now wait until the service is exiting va SIGTERM or SIGSTOP
                            UnixSignal[] signals = new UnixSignal[]
                            {
                                new UnixSignal(Mono.Unix.Native.Signum.SIGINT),
                                new UnixSignal(Mono.Unix.Native.Signum.SIGTERM),
                                new UnixSignal(Mono.Unix.Native.Signum.SIGQUIT),
                                new UnixSignal(Mono.Unix.Native.Signum.SIGHUP)
                            };

                            ApplicationServiceContext.Current.Stopped += (o, e) =>
                            {
                                Console.WriteLine("Service has stopped, will send SIGHUP to self for restart");
                                Syscall.kill(Syscall.getpid(), Signum.SIGHUP);
                            };

                            Console.WriteLine("Started - Send SIGINT, SIGTERM, SIGQUIT or SIGHUP to PID {0} to terminate", Process.GetCurrentProcess().Id);
                            int signal = UnixSignal.WaitAny(signals);
                        }
                    }

                    Console.WriteLine($"Received termination signal...");
                    ServiceUtil.Stop();

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
                        if (!String.IsNullOrEmpty(parms.BaseUrl))
                            argList += $" --base=\"{parms.BaseUrl}\"";

                        ServiceTools.ServiceInstaller.Install(
                            serviceName, $"SanteDB DCG ({parms.InstanceName})",
                            $"{entryAsm.Location} --name=\"{parms.InstanceName}\" {argList}",
                            null, null, ServiceTools.ServiceBootFlag.AutoStart);
                    }
                    else
                        throw new InvalidOperationException("Service instance already installed");
                }
                else if (parms.Uninstall)
                {
                    string serviceName = $"sdb-dcg-{parms.InstanceName}";
                    if (ServiceTools.ServiceInstaller.ServiceIsInstalled(serviceName))
                        ServiceTools.ServiceInstaller.Uninstall(serviceName);
                    else
                        throw new InvalidOperationException("Service instance not installed");
                }
                else if (parms.Restart)
                {
                    string serviceName = $"sdb-dcg-{parms.InstanceName}";
                    if (ServiceTools.ServiceInstaller.ServiceIsInstalled(serviceName))
                    {
                        Console.Write("Stopping {0}...", serviceName);
                        var niter = 0;
                        ServiceTools.ServiceInstaller.StopService(serviceName);
                        while (ServiceTools.ServiceInstaller.GetServiceStatus(serviceName) != ServiceTools.ServiceState.Stop && niter < 10)
                        {
                            Thread.Sleep(1000);
                            Console.Write(".");
                            niter++;
                        }
                        Console.Write("\r\nStarting {0}...", serviceName);
                        ServiceTools.ServiceInstaller.StartService(serviceName);
                        while (ServiceTools.ServiceInstaller.GetServiceStatus(serviceName) != ServiceTools.ServiceState.Run && niter < 20)
                        {
                            Thread.Sleep(1000);
                            Console.Write(".");
                            niter++;
                        }
                        Console.WriteLine("Restart Complete");
                    }
                }
                else
                {
                    Trace.TraceInformation("Starting as Windows Service");
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                        new SanteDbService(parms.InstanceName, CreateContext(parms))
                    };
                    ServiceBase.Run(ServicesToRun);
                    Trace.TraceInformation("Started As Windows Service...");
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


        /// <summary>
        /// Create context
        /// </summary>
        private static IApplicationServiceContext CreateContext(ConsoleParameters parms)
        {
            ServicePointManager.DefaultConnectionLimit = Environment.ProcessorCount;

            var configDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "santedb", "dcg", parms.InstanceName);
            var dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "santedb", "dcg", parms.InstanceName);

            // Running as system?
            if (configDirectory.Contains(Environment.GetFolderPath(Environment.SpecialFolder.Windows)))
            {
                configDirectory = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "instances", parms.InstanceName, "config");
                dataDirectory = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "instances", parms.InstanceName, "data");
            }

            Trace.TraceInformation("Configuration Directory: {0}", configDirectory);
            Trace.TraceInformation("Data Directory: {0}", dataDirectory);

            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }

            // Security Application Information
            var applicationIdentity = new UpstreamCredentialConfiguration()
            {
                Conveyance = UpstreamCredentialConveyance.Secret,
                CredentialName = parms.ApplicationName ?? "org.santedb.disconnected_client.gateway",
                CredentialSecret = parms.ApplicationSecret ?? "SDB$$DEFAULT$$APPSECRET",
                CredentialType = UpstreamCredentialType.Application
            };

            ClientBatteries.Initialize(dataDirectory, configDirectory, applicationIdentity);
            // Establish a configuration environment 
            IConfigurationManager configurationManager = null;
            var configurationFile = Path.Combine(configDirectory, "santedb.config");
            if (File.Exists(configurationFile))
            {
                configurationManager = new FileConfigurationService(configurationFile, true);
            }
            else
            {
                configurationManager = new InitialConfigurationManager(SanteDBHostType.Gateway, parms.InstanceName, configurationFile);
            }

#if DEBUG
            // TODO: Git Submodules would eliminate this
            configurationManager.Configuration.GetSection<ApplicationServiceContextConfigurationSection>().AllowUnsignedAssemblies = true;
#endif

            return new GatewayApplicationContext(parms, configurationManager);
        }
    }
}
