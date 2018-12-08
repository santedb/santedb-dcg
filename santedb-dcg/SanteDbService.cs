﻿/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
 *
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
 * User: justin
 * Date: 2018-10-14
 */
using SanteDB.Core.Model.Security;
using SanteDB.DisconnectedClient.Core.Configuration;
using SanteDB.DisconnectedClient.UI;
using SanteDB.DisconnectedClient.Xamarin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Dcg
{
    public partial class SanteDbService : ServiceBase
    {
        // THe application identity
        private SecurityApplication m_applicationIdentity;

        /// <summary>
        /// SanteDB Service
        /// </summary>
        public SanteDbService(string instanceName, SecurityApplication applicationIdentity)
        {
            InitializeComponent();
            this.m_applicationIdentity = applicationIdentity;
            this.ServiceName = instanceName;
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            XamarinApplicationContext.ProgressChanged += (o, e) =>
            {
                Trace.TraceInformation(">>> PROGRESS >>> {0} : {1:#0%}", e.ProgressText, e.Progress);
            };

            if (!DcApplicationContext.StartContext(new ConsoleDialogProvider(), $"dcg-{this.ServiceName}", this.m_applicationIdentity))
                DcApplicationContext.StartTemporary(new ConsoleDialogProvider(), $"dcg-{this.ServiceName}", this.m_applicationIdentity);

            DcApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().AppSettings.RemoveAll(o => o.Key == "http.bypassMagic");
            DcApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().AppSettings.Add(new AppSettingKeyValuePair() { Key = "http.bypassMagic", Value = DcApplicationContext.Current.ExecutionUuid.ToString() });
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            DcApplicationContext.Current.Stop();
        }
    }
}