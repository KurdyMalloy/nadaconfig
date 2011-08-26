using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using System.ServiceModel;
using NadaConfigServer;
using NadaConfigService;


namespace NadaServer
{
    
    class Program
    {
        static void Main(string[] args)
        {

            using (ServiceHost host = new ServiceHost(typeof(NadaConfigService.NadaConfigService)))
            {
                System.Diagnostics.Trace.WriteLine(String.Format(@"Starting WCF host --> {0}", host.BaseAddresses[0].AbsoluteUri));
                host.Open();
                List<string> environments = ConfigSection.Instance.GetEnvironments();
                List<MailslotServer> MSServers = new List<MailslotServer>(environments.Count);
                foreach(string env in environments)
                {
                    MSServers.Add(new MailslotServer(env, host.BaseAddresses[0].AbsoluteUri));
                }
                //MailslotServer server = new MailslotServer(@"TestEnv", host.BaseAddresses[0].AbsoluteUri);

                foreach (MailslotServer server in MSServers)
                {
                    System.Diagnostics.Trace.WriteLine(string.Format(@"Starting MailSlot Server for env --> {0}", server.Environment));
                    server.Start();
                }
                Console.WriteLine(@"Service is available. " +
                    @"Hit anything to exit...");
                Console.ReadLine();
                foreach (MailslotServer server in MSServers)
                {
                    System.Diagnostics.Trace.WriteLine(string.Format(@"Stopping MailSlot Server for env --> {0}", server.Environment));
                    server.Stop();
                }

                System.Diagnostics.Trace.WriteLine(@"Stopping WCF host");
                host.Close();
            }

        }
    }
}
