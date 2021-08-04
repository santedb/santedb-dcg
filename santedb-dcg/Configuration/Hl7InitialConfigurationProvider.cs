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
using SanteDB.Core.Configuration;
using SanteDB.DisconnectedClient.Configuration;
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

                existing.GetSection<ApplicationServiceContextConfigurationSection>().ServiceProviders.Add(new TypeReferenceConfiguration(typeof(SanteDB.Messaging.HL7.HL7MessageHandler)));
                var adtHandler = new AdtMessageHandler();
                var qbpHandler = new QbpMessageHandler();

                existing.AddSection(new Hl7ConfigurationSection()
                {
                    LocalAuthority = new Core.Model.DataTypes.AssigningAuthority()
                    {
                        DomainName = "LOCAL_AUTH"
                    },
                    Security = AuthenticationMethod.Msh8,
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
