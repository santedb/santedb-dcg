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
using SanteDB.Client.Configuration;
using SanteDB.Core;
using SanteDB.Core.Configuration;
using SanteDB.Core.Model.Constants;
using SanteDB.Messaging.HL7;
using SanteDB.Messaging.HL7.Configuration;
using SanteDB.Messaging.HL7.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SanteDB.Dcg.Configuration
{
    /// <summary>
    /// Represents a configuration provider that initializes the HL7v2 interface
    /// </summary>
    public class Hl7InitialConfigurationProvider : IInitialConfigurationProvider
    {
        /// <summary>
        /// Get the order of application
        /// </summary>
        public int Order => Int16.MaxValue;

        private IHL7MessageHandler CreateHandlerNull(Type handlerType)
        {
            return handlerType.CreateInjected() as IHL7MessageHandler;
        }

        /// <summary>
        /// Provide initial configuration
        /// </summary>
        public SanteDBConfiguration Provide(SanteDBHostType hostType, SanteDBConfiguration configuration)
        {
            if (!configuration.Sections.Any(o => o is SanteDB.Messaging.HL7.Configuration.Hl7ConfigurationSection))
            {

                configuration.GetSection<ApplicationServiceContextConfigurationSection>().ServiceProviders.Add(new TypeReferenceConfiguration(typeof(SanteDB.Messaging.HL7.HL7MessageHandler)));

                configuration.AddSection(new Hl7ConfigurationSection()
                {
                    LocalAuthority = new Core.Model.DataTypes.IdentityDomain()
                    {
                        DomainName = "LOCAL_AUTH",
                        Oid = $"2.25.{BitConverter.ToUInt64(Guid.NewGuid().ToByteArray(), 0)}",
                        Url = "auth.local"
                    },
                    LocalFacility = Guid.Empty,
                    RequireAuthenticatedApplication = true,
                    SsnAuthority = new Core.Model.DataTypes.IdentityDomain()
                    {
                        DomainName = "SSN",
                        Oid = "2.16.840.1.113883.4.1",
                        Url = "http://hl7.org/fhir/sid/us-ssn",
                        IdentifierClassificationKey = IdentifierTypeKeys.NationalInsurance
                    },
                    Security = Hl7AuthenticationMethod.Msh8,
                    IdentifierReplacementBehavior = IdentifierReplacementMode.Specific,
                    BirthplaceClassKeys = new List<Guid>()
                    {
                        EntityClassKeys.CityOrTown,
                        EntityClassKeys.ServiceDeliveryLocation,
                        EntityClassKeys.PrecinctOrBorough,
                        EntityClassKeys.Country,
                        EntityClassKeys.CountyOrParish
                    },
                    Services = new List<Hl7ServiceDefinition>()
                    {
                        new Hl7ServiceDefinition()
                        {
                            AddressXml = "llp://127.0.0.1:12100",
                            Name = "Default Endpoint",
                            ReceiveTimeout = 30000
                        }
                    },
                    StrictAssigningAuthorities = true,
                    StrictMetadataMatch = true
                }); ;
            }
            return configuration;

        }
    }
}
