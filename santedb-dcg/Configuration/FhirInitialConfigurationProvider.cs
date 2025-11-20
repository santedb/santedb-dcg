using SanteDB.Client.Configuration;
using SanteDB.Core;
using SanteDB.Core.Configuration;
using SanteDB.Messaging.FHIR;
using SanteDB.Messaging.FHIR.Configuration;
using SanteDB.Messaging.FHIR.Rest;
using SanteDB.Rest.Common.Behavior;
using SanteDB.Rest.Common.Configuration;
using SanteDB.Rest.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Dcg.Configuration
{
    /// <summary>
    /// Initial configuration provider for FHIR
    /// </summary>
    public class FhirInitialConfigurationProvider : IInitialConfigurationProvider
    {
        public const string BINDING_BASE_DATA = "http.binding";

        /// <inheritdoc/>
        public int Order => Int16.MaxValue;

        /// <inheritdoc/>
        public SanteDBConfiguration Provide(SanteDBHostType hostContextType, SanteDBConfiguration configuration)
        {
            if (AppDomain.CurrentDomain.GetData(BINDING_BASE_DATA) == null || hostContextType == SanteDBHostType.Test)
            {
                return configuration;
            }
            var bindingBase = new Uri(AppDomain.CurrentDomain.GetData(BINDING_BASE_DATA)?.ToString());
            if (bindingBase == null)
            {
                switch (hostContextType)
                {
                    case SanteDBHostType.Client:
                        bindingBase = new Uri("http://127.0.0.1:9200");
                        break;
                    case SanteDBHostType.Gateway:
                        bindingBase = new Uri("http://0.0.0.0:9200");
                        break;
                }
            }

            var appSection = configuration.GetSection<ApplicationServiceContextConfigurationSection>();
            if(!appSection.ServiceProviders.Any(o=>o.Type == typeof(FhirMessageHandler)))
            {
                var serviceUrlBase = $"{bindingBase.Scheme}://{bindingBase.Host}:{bindingBase.Port}/fhir";

                appSection.ServiceProviders.Add(new TypeReferenceConfiguration(typeof(FhirMessageHandler)));

                configuration.AddSection(new FhirServiceConfigurationSection()
                {
                    DefaultResponseFormat = FhirResponseFormatConfiguration.Json,
                    PersistElementId = true,
                    ResourceBaseUri = serviceUrlBase
                });

                // Add the rest configuration
                var restConfiguration = configuration.GetSection<RestConfigurationSection>();

                // Is there a service already registered for FHIR?
                if(!restConfiguration.Services.Any(s=>s.ConfigurationName == "FHIR" || s.ServiceType == typeof(FhirServiceBehavior)))
                {
                    restConfiguration.Services.Add(new RestServiceConfiguration()
                    {
                        ConfigurationName = "FHIR",
                        ServiceType = typeof(FhirServiceBehavior),
                        Behaviors = new List<RestServiceBehaviorConfiguration>()
                        {
                            new RestServiceBehaviorConfiguration(typeof(TokenAuthorizationAccessBehavior))
                        },
                        Endpoints = new List<RestEndpointConfiguration>()
                        {
                            new RestEndpointConfiguration()
                            {
                                Address = serviceUrlBase,
                                Contract = typeof(IFhirServiceContract),
                                ClientCertNegotiation = false,
                                Behaviors = new List<RestEndpointBehaviorConfiguration>()
                                {
                                    new RestEndpointBehaviorConfiguration(typeof(ServerMetadataServiceBehavior)),
                                    new RestEndpointBehaviorConfiguration(typeof(MessageLoggingEndpointBehavior)),
                                    new RestEndpointBehaviorConfiguration(typeof(MessageCompressionEndpointBehavior)),
                                    new RestEndpointBehaviorConfiguration(typeof(CorsEndpointBehavior))
                                    {
                                        ConfigurationString = @"<CorsEndpointBehaviorConfiguration xmlns=""http://santedb.org/configuration"">
                                            <resource name=""*"" domain=""*"">
                                              <verbs>
                                                <add>OPTIONS</add>
                                                <add>POST</add>
                                                <add>PUT</add>
                                                <add>PATCH</add>
                                                <add>DELETE</add>
                                                <add>GET</add>
                                              </verbs>
                                              <headers>
                                                <add>Content-Type</add>
                                                <add>Accept-Encoding</add>
                                                <add>Content-Encoding</add>
                                              </headers>
                                            </resource>
                                          </CorsEndpointBehaviorConfiguration>"
                                    }
                                }
                            }
                        }
                        
                    });
                }
            }


            return configuration;
        }
    }
}
