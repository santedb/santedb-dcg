using SanteDB.Core.Configuration;
using SanteDB.DisconnectedClient.Core.Configuration;
using SanteDB.DisconnectedClient.UI;
using SanteDB.Messaging.HL7.Configuration;
using SanteDB.Messaging.HL7.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Dcg.Configuration
{
    /// <summary>
    /// Represents a configuration provider that initializes the HL7v2 interface
    /// </summary>
    public class Hl7InitialConfigurationProvider : IInitialConfigurationProvider
    {
        /// <summary>
        /// Provide initial configuration
        /// </summary>
        public SanteDBConfiguration Provide(SanteDBConfiguration existing)
        {
            if (!existing.Sections.Any(o => o is SanteDB.Messaging.HL7.Configuration.Hl7ConfigurationSection))
            {

                existing.GetSection<ApplicationServiceContextConfigurationSection>().ServiceProviders.Add(new TypeReferenceConfiguration( typeof(SanteDB.Messaging.HL7.HL7MessageHandler).AssemblyQualifiedName));
                var adtHandler = new AdtMessageHandler();
                var qbpHandler = new QbpMessageHandler();

                existing.AddSection(new Hl7ConfigurationSection()
                {
                    LocalAuthority = new Core.Model.DataTypes.AssigningAuthority()
                    {
                        DomainName = "LOCAL_AUTH"
                    },
                    Security = SecurityMethod.Msh8,
                    Interceptors = new List<Hl7InterceptorConfigurationElement>(),
                    Services = new List<Hl7ServiceDefinition>()
                    {
                        new Hl7ServiceDefinition()
                        {
                            AddressXml = "llp://127.0.0.1:12100",
                            Name = "Default Endpoint",
                            ReceiveTimeout = 30000,
                            Handlers = new List<HandlerDefinition>()
                            {
                                new HandlerDefinition()
                                {
                                    Handler = adtHandler,
                                    Types = adtHandler.SupportedTriggers.Select(o=>new MessageDefinition()
                                    {
                                        IsQuery = false,
                                        Name = o
                                    }).ToList()
                                },
                                new HandlerDefinition()
                                {
                                    Handler = qbpHandler,
                                    Types = qbpHandler.SupportedTriggers.Select(o=>new MessageDefinition()
                                    {
                                        IsQuery = true,
                                        Name = o
                                    }).ToList()
                                }
                            }
                        }
                    }
                });
            }
            return existing;

        }
    }
}
