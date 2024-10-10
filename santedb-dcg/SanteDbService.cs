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
using SanteDB.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using SanteDB.Core.Configuration;
using SanteDB.Core;

namespace SanteDB.Dcg
{
    public partial class SanteDbService : ServiceBase
    {
        private bool m_serviceStop = false;

        private readonly IApplicationServiceContext m_applicationServiceContext;

        /// <summary>
        /// SanteDB Service
        /// </summary>
        public SanteDbService(string instanceName, IApplicationServiceContext applicationServiceContext)
        {
            InitializeComponent();
            this.m_applicationServiceContext = applicationServiceContext;
            this.ServiceName = $"SanteDB Gateway {instanceName}";
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            try
            {
                ServiceUtil.Start(Guid.NewGuid(), this.m_applicationServiceContext);
            }
            catch (Exception e)
            {
                Trace.TraceError("The service reported an error: {0}", e);
                EventLog.WriteEntry("SanteDB", $"Service Startup reported an error: {e}", EventLogEntryType.Error, 1911);
                Environment.FailFast($"Error starting DCG service: {e.Message}");
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                Trace.TraceInformation("Stopping Service");
                this.m_serviceStop = true;
                ServiceUtil.Stop();
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("SanteDB", $"Service Shutdown reported an error: {e}", EventLogEntryType.Error, 1911);
                Trace.TraceError("The service reported an error: {0}", e);
                Environment.FailFast($"Error stopping DCG service: {e.Message}");
            }
        }
    }
}
