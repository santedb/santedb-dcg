﻿using SanteGuard.Messaging.Syslog.Action;
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