﻿/*
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
using SanteDB.Dcg.Services;
using SanteDB.DisconnectedClient.Configuration;
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
                existing.GetSection<ApplicationServiceContextConfigurationSection>().ServiceProviders.Add(new TypeReferenceConfiguration(typeof(SanteGuard.Messaging.Syslog.SyslogMessageHandler)));

                existing.AddSection(new SanteGuardConfiguration()
                {
                    DefaultEnterpriseSiteID = "AUDIT^^^AUDIT",
                    Endpoints = new List<EndpointConfiguration>()
                    {
                        new EndpointConfiguration()
                        {
                            AddressXml = "udp://127.0.0.1:11514",
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
