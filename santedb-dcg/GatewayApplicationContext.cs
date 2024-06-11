using SanteDB.Client;
using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Dcg
{
    /// <summary>
    /// DCG application context
    /// </summary>
    internal class GatewayApplicationContext : ClientApplicationContextBase
    {
        // The original console parameters
        private ConsoleParameters m_consoleParameters;
        private Tracer m_tracer = Tracer.GetTracer(typeof(GatewayApplicationContext));
        private readonly IConfigurationManager m_configurationManager;
        private bool m_restartRequested = false;


        /// <inheritdoc/>
        public GatewayApplicationContext(ConsoleParameters debugParameters, IConfigurationManager configurationManager) : base(SanteDBHostType.Gateway, debugParameters.InstanceName, configurationManager)
        {
            this.m_configurationManager = configurationManager;
            this.m_consoleParameters = debugParameters;
            this.Stopped += (o, e) =>
            {
                if (this.m_restartRequested)
                {
                    this.m_tracer.TraceWarning("Attempting to restart SanteDB-dcg host");
                    if (this.m_consoleParameters.ConsoleMode)
                    {
                        var pi = new ProcessStartInfo(typeof(Program).Assembly.Location, string.Join(" ", this.m_consoleParameters.ToArgumentList()));
                        Process.Start(pi);
                    }
                    else
                    {
                        var pi = new ProcessStartInfo(typeof(Program).Assembly.Location, $"--restart --name={this.m_consoleParameters.InstanceName}");
                        Process.Start(pi);
                    }
                }
            };
        }

        /// <inheritdoc/>
        protected override void OnRestartRequested(object sender)
        {
            // Delay fire - allow other objects to finish up on the restart request event
            this.m_restartRequested = true;

            ServiceUtil.Stop();
            Environment.Exit(0);
        }

    }
}
