using SanteDB.Core.Configuration;
using SanteDB.Dcg.Services;
using SanteDB.DisconnectedClient.Core.Configuration;
using SanteDB.DisconnectedClient.UI;
using SanteGuard.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Dcg.Configuration
{
    /// <summary>
    /// Initial configuration provider for SanteGuard
    /// </summary>
    public class SanteGuardInitialConfigurationProvider : IInitialConfigurationProvider
    {

        /// <summary>
        /// Provide initial configuration
        /// </summary>
        public SanteDBConfiguration Provide(SanteDBConfiguration existing)
        {
            if (existing.GetSection<SanteGuardConfiguration>() == null)
            {
                existing.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(SanteGuard.Messaging.Syslog.SyslogMessageHandler).AssemblyQualifiedName);

                existing.AddSection(new SanteGuardConfiguration()
                {
                    DefaultEnterpriseSiteID = "AUDIT^^^AUDIT",
                    Endpoints = new List<EndpointConfiguration>()
                    {
                        new EndpointConfiguration()
                        {
                            AddressXml = "udp://127.0.0.1:514",
                            Name = "Audit UDP",
                            MaxSize = ushort.MaxValue,
                            ReadTimeout = new TimeSpan(0, 0, 15),
                            Timeout = new TimeSpan(0,0,15),
                            ActionXml = new List<string>()
                            {
                                typeof(SanteGuardGatewayAction).AssemblyQualifiedName,
                                typeof(SanteGuard.Messaging.Syslog.Action.LogAction).AssemblyQualifiedName
                            },
                            LogFileLocation ="messages.txt"
                        }
                    }
                });
            }

            return existing;
        }
    }
}
