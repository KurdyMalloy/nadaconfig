using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NadaConfigClient;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"Connecting to environment --> Production");
            NadaClient myClient = new NadaClient(@"Production"); //default timeout 10seconds
            //NadaClient myClient = new NadaClient(@"Production", @"net.tcp://servername:8081");
            if (myClient.isConnected)
            {
                Console.WriteLine(@"Connected");
                string val = myClient.GetConfigItem(@"Database", @"test");
                Console.WriteLine(@"Value for Database-test is --> {0}", val);
            }
            else
            {
                Console.WriteLine(@"Timeout");
            }
        }
    }

}
