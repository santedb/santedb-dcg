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
using SanteGuard.Messaging.Syslog.Action;
using SanteGuard.Messaging.Syslog.TransportProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SanteGuard.Core.Model;
using SanteDB.Core;
using SanteDB.Core.Services;
using SanteDB.Core.Security.Services;
using System.Diagnostics;
using SanteDB.Core.Diagnostics;

namespace SanteDB.Dcg.Services
{
    /// <summary>
    /// Ensures that the audit messages are stored and fwded
    /// </summary>
    public class SanteGuardGatewayAction : ISyslogAction
    {

        // Tacer
        private Tracer m_tracer = Tracer.GetTracer(typeof(SanteGuardGatewayAction));

        /// <summary>
        /// Handle an invalid message
        /// </summary>
        public void HandleInvalidMessage(object sender, SyslogMessageReceivedEventArgs e)
        {
            // No implementation
        }

        /// <summary>
        /// Handle a message received
        /// </summary>
        public void HandleMessageReceived(object sender, SyslogMessageReceivedEventArgs e)
        {
            ApplicationServiceContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem(o =>
            {
                var parseResult = ((MessageUtil.ParseAuditResult)o);
                if (parseResult.Message == null)
                {
                    this.m_tracer.TraceError("Could not process audit:");
                    foreach (var m in parseResult.Details)
                        this.m_tracer.TraceError("\t{0}:{1}", m.Priority, m.Text);
                }
                else
                {
                    var audit = ((MessageUtil.ParseAuditResult)o).Message.ToAudit().ToAuditData();
                    if (audit != null)
                    {
                        audit = ApplicationServiceContext.Current.GetService<IAuditRepositoryService>().Insert(audit);
                        this.m_tracer.TraceInfo("Stored audit {0}", audit.Key);
                    }
                    else
                    {
                        this.m_tracer.TraceError("Invalid audit message received");
                        foreach (var i in ((MessageUtil.ParseAuditResult)o).Details)
                            this.m_tracer.TraceError("{0} : {1}", i.Priority, i.Text);
                    }
                }
            }, MessageUtil.ParseAudit(e.Message));
        }
    }
}
