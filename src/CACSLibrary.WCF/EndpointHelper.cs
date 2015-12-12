using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;

namespace CACSLibrary.WCF
{
    /// <summary>
    /// 
    /// </summary>
    public class EndpointHelper
    {
        private static void AddBehaviors(string behaviorConfiguration, ServiceEndpoint serviceEndpoint, ServiceModelSectionGroup group)
        {
            if (!string.IsNullOrEmpty(behaviorConfiguration))
            {
                EndpointBehaviorElement element = group.Behaviors.EndpointBehaviors[behaviorConfiguration];
                for (int i = 0; i < element.Count; i++)
                {
                    BehaviorExtensionElement target = element[i];
                    object obj2 = target.GetType().InvokeMember("CreateBehavior", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, target, null);
                    if (obj2 != null)
                    {
                        serviceEndpoint.Behaviors.Add((IEndpointBehavior)obj2);
                    }
                }
            }
        }

        private static Binding CreateBinding(string bindingName, ServiceModelSectionGroup group)
        {
            BindingCollectionElement element = group.Bindings[bindingName];
            if (element.ConfiguredBindings.Count > 0)
            {
                IBindingConfigurationElement configurationElement = element.ConfiguredBindings[0];
                Binding binding = GetBinding(configurationElement);
                if (configurationElement != null)
                {
                    configurationElement.ApplyConfiguration(binding);
                }
                return binding;
            }
            return null;
        }

        private static Binding GetBinding(IBindingConfigurationElement configurationElement)
        {
            if (configurationElement is CustomBindingElement)
            {
                return new CustomBinding();
            }
            if (configurationElement is BasicHttpBindingElement)
            {
                return new BasicHttpBinding();
            }
            if (configurationElement is NetMsmqBindingElement)
            {
                return new NetMsmqBinding();
            }
            if (configurationElement is NetNamedPipeBindingElement)
            {
                return new NetNamedPipeBinding();
            }
#pragma warning disable 0618
            if (configurationElement is NetPeerTcpBindingElement)
            {
                return new NetPeerTcpBinding();
            }
            if (configurationElement is NetTcpBindingElement)
            {
                return new NetTcpBinding();
            }
            if (configurationElement is WSDualHttpBindingElement)
            {
                return new WSDualHttpBinding();
            }
            if (configurationElement is WSHttpBindingElement)
            {
                return new WSHttpBinding();
            }
            if (configurationElement is WSFederationHttpBindingElement)
            {
                return new WSFederationHttpBinding();
            }
            return null;
        }

        private static EndpointIdentity GetIdentity(IdentityElement element)
        {
            PropertyInformationCollection properties = element.ElementInformation.Properties;
            if (properties["userPrincipalName"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateUpnIdentity(element.UserPrincipalName.Value);
            }
            if (properties["servicePrincipalName"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateSpnIdentity(element.ServicePrincipalName.Value);
            }
            if (properties["dns"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateDnsIdentity(element.Dns.Value);
            }
            if (properties["rsa"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateRsaIdentity(element.Rsa.Value);
            }
            if (properties["certificate"].ValueOrigin != PropertyValueOrigin.Default)
            {
                X509Certificate2Collection supportingCertificates = new X509Certificate2Collection();
                supportingCertificates.Import(Convert.FromBase64String(element.Certificate.EncodedValue));
                if (supportingCertificates.Count == 0)
                {
                    throw new InvalidOperationException("UnableToLoadCertificateIdentity");
                }
                X509Certificate2 primaryCertificate = supportingCertificates[0];
                supportingCertificates.RemoveAt(0);
                return EndpointIdentity.CreateX509CertificateIdentity(primaryCertificate, supportingCertificates);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configPath"></param>
        /// <param name="configName"></param>
        /// <param name="contractName"></param>
        /// <returns></returns>
        public static ServiceEndpoint GetServiceEndpoint(string configPath, string configName, string contractName)
        {
            ServiceEndpoint serviceEndpoint = new ServiceEndpoint(new ContractDescription(contractName))
            {
                Contract = { ConfigurationName = contractName }
            };
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configPath
            };
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            if (!config.HasFile)
            {
                throw new FileNotFoundException("无法找到配置文件 '" + config.FilePath + "' ");
            }
            ServiceModelSectionGroup sectionGroup = ServiceModelSectionGroup.GetSectionGroup(config);
            ChannelEndpointElement element = null;
            foreach (ChannelEndpointElement element2 in sectionGroup.Client.Endpoints)
            {
                if ((element2.Contract == serviceEndpoint.Contract.ConfigurationName) && ((configName == null) || (configName == element2.Name)))
                {
                    element = element2;
                    break;
                }
            }
            if (element != null)
            {
                if (serviceEndpoint.Binding == null)
                {
                    serviceEndpoint.Binding = CreateBinding(element.Binding, sectionGroup);
                }
                if (serviceEndpoint.Address == null)
                {
                    serviceEndpoint.Address = new EndpointAddress(element.Address, GetIdentity(element.Identity), element.Headers.Headers);
                }
                if ((serviceEndpoint.Behaviors.Count == 0) && (element.BehaviorConfiguration != null))
                {
                    AddBehaviors(element.BehaviorConfiguration, serviceEndpoint, sectionGroup);
                }
                serviceEndpoint.Name = element.Contract;
            }
            return serviceEndpoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static ServiceEndpoint[] GetServiceEndpoints(string configPath)
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configPath
            };
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            if (!config.HasFile)
            {
                throw new FileNotFoundException("无法找到配置文件 '" + config.FilePath + "' ");
            }
            ServiceModelSectionGroup sectionGroup = ServiceModelSectionGroup.GetSectionGroup(config);
            List<ServiceEndpoint> list = new List<ServiceEndpoint>();
            foreach (ChannelEndpointElement element in sectionGroup.Client.Endpoints)
            {
                ServiceEndpoint serviceEndpoint = new ServiceEndpoint(new ContractDescription(element.Contract));
                if (serviceEndpoint.Binding == null)
                {
                    serviceEndpoint.Binding = CreateBinding(element.Binding, sectionGroup);
                }
                if (serviceEndpoint.Address == null)
                {
                    serviceEndpoint.Address = new EndpointAddress(element.Address, GetIdentity(element.Identity), element.Headers.Headers);
                }
                if ((serviceEndpoint.Behaviors.Count == 0) && (element.BehaviorConfiguration != null))
                {
                    AddBehaviors(element.BehaviorConfiguration, serviceEndpoint, sectionGroup);
                }
                serviceEndpoint.Name = element.Contract;
                list.Add(serviceEndpoint);
            }
            return list.ToArray();
        }
    }
}
