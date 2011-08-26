using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.ServiceModel;
using System.Diagnostics;

using NadaConfigService;
using cMailSlot;

namespace NadaConfigClient
{
    public class NadaClient
    {
        private string env;
        private string servername;
        private Dictionary<string, Dictionary<string, string>> configDictionary = new Dictionary<string, Dictionary<string, string>>(StringComparer.InvariantCultureIgnoreCase);
        private string configServiceEndpoint;
        private bool serviceFound = false;

        public string Environment
        {
            get { return env; }
        }
        
        public string ServerName
        {
            get { return servername; }
        }
        public string ServiceName
        {
            get { return configServiceEndpoint; }
        }

        public bool isConnected
        {
            get { return serviceFound; }
        }


        // This constructor forces the service endpoint and does not use mailslot to search the service
        public NadaClient(string environment, string serviceEndpoint)
        {
            configServiceEndpoint = serviceEndpoint;
            env = environment;
            serviceFound = true;
        }

        // search environment with default timeout of 10 seconds
        public NadaClient(string environment) : this(environment, null, 10000) { }
        
        // search environment with a provided timeout
        public NadaClient(string environment, int timeout) : this(environment, null, timeout) { }
        
        // Forces environment search on specific server with a provided timeout
        public NadaClient(string environment, string server, int timeout)
        {
            if (!String.IsNullOrEmpty(environment))
            {
                env = environment;
                MailslotClient msClient;
                if (!String.IsNullOrEmpty(server))
                {
                    servername = server;
                    msClient = new MailslotClient(environment, server);
                }
                else
                {
                    msClient = new MailslotClient(environment);
                }

                if (msClient.FindService(timeout))
                {
                    configServiceEndpoint = msClient.configServiceName;
                    servername = msClient.configHostName;
                    serviceFound = true;
                }
                else
                {
                }
            }
        }

        private bool LoadSection(string section)
        {
            try
            {
                if (serviceFound && (!configDictionary.ContainsKey(section)))
                {
                    INadaConfigService configservice = ChannelFactory<INadaConfigService>.CreateChannel(new NetTcpBinding(SecurityMode.None, false), new EndpointAddress(this.configServiceEndpoint));

                    var sectionDict = configservice.GetSection(this.env, section);
                    if (sectionDict != null)
                        configDictionary.Add(section, sectionDict);

                    ((IClientChannel)configservice).Close();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetConfigItem(string section, string name)
        {
            LoadSection(section);
            if (configDictionary.ContainsKey(section) && configDictionary[section].ContainsKey(name))
                return configDictionary[section][name];
            else
                return string.Empty;
        }

    }

}
