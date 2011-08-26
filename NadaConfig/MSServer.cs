using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using cMailSlot;
using System.IO;
using System.Threading;

namespace NadaConfigServer
{
    public static class Network
    {
        #region DNS
        public static IPAddress FindIPAddress(bool localPreference)
        {
            return FindIPAddress(Dns.GetHostEntry(Dns.GetHostName()),
            localPreference);
        }

        public static IPAddress FindIPAddress(IPHostEntry host, bool
        localPreference)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            if (host.AddressList.Length == 1)
                return host.AddressList[0];
            else
            {
                foreach (System.Net.IPAddress address in host.AddressList)
                {
                    bool local = IsLocal(address);

                    if (local && localPreference)
                        return address;
                    else if (!local && !localPreference)
                        return address;
                }

                return host.AddressList[0];
            }
        }

        public static bool IsLocal(IPAddress address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            byte[] addr = address.GetAddressBytes();

            return addr[0] == 10
            || (addr[0] == 192 && addr[1] == 168)
            || (addr[0] == 172 && addr[1] >= 16 && addr[1] <= 31);
        }
        #endregion
    }


    public class MailslotServer
    {
        string machineName;
        string hostName;
        string ipAdress;
        string environmentName;
        string serviceName;

        bool stopProcess = false;
        Thread listeningThread;

        MailSlot listeningMailslot;
        StreamReader mailslotReader;

        MsgIDLeveler leveler;

        public string Environment
        {
            get { return environmentName; }
        }

                
        public MailslotServer(string environment, string service)
        {
            machineName = System.Environment.MachineName;
            hostName = Dns.GetHostName();
            ipAdress = Network.FindIPAddress(true).ToString();
            this.environmentName = environment;
            serviceName = service.Replace("localhost", ipAdress);

            leveler = new MsgIDLeveler();

        }
        
        private MailslotServer()
        {
        }

        private bool CreateMailslot ()
        {
            try
            {
                listeningMailslot = new MailSlot(environmentName);
                System.Diagnostics.Debug.WriteLine(String.Format(@"Listening MailSlot Created --> {0}", listeningMailslot.Filename));
                mailslotReader = new StreamReader(listeningMailslot.FStream);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                System.Diagnostics.Trace.WriteLine(e.StackTrace);
                return false;
            }
        }

        private void CloseMailslot()
        {
            if (mailslotReader != null)
                mailslotReader.Dispose();
            if (listeningMailslot != null)
                listeningMailslot.Dispose();
        }

        public bool Start()
        {
            if (!CreateMailslot())
            {
                CloseMailslot();
                return false;
            }

            stopProcess = false;
            
            listeningThread = new Thread(new ThreadStart(StartProcessing));
            listeningThread.IsBackground = true;
            listeningThread.Start();

            return true;
        }

        public void Stop()
        {
            stopProcess = true;
            listeningThread.Join(1000).ToString();
            CloseMailslot();
        }

        void StartProcessing()
        {
            while (!stopProcess)
            {
                try
                {
                    while ((listeningMailslot.isMessagesWaiting > 0) && !stopProcess)
                    {
                        ProcessMessage();
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    System.Diagnostics.Trace.WriteLine(e.StackTrace);
                }
                Thread.Sleep(200);
            }
        }

        void ProcessMessage()
        {
            string message = mailslotReader.ReadLine();
            if (!String.IsNullOrEmpty(message))
            {
                System.Diagnostics.Debug.WriteLine(string.Format(@"Received Message --> {0}", message));

                var msgParts = message.Split('|');
                string msgID = msgParts[0];
                string scope = msgParts[1];
                string remote_env = msgParts[2];

                if (leveler.IsMsgFirst(msgID))
                {
                    //Console.WriteLine(@"Remote Mailslot Name --> {0}", remote_env);
                    //Console.WriteLine(@"Remote Adresse --> {0}", scope);
                    //Create remote mailslot to answer
                    MailSlot remote_ms = new MailSlot(remote_env, scope);
                    StreamWriter sw = new StreamWriter(remote_ms.FStream);

                    sw.WriteLine(machineName + "|" + serviceName);
                    System.Diagnostics.Debug.WriteLine(String.Format(@"Writing Answer --> {0} To --> {1}", machineName + "|" + serviceName, remote_ms.Filename));
                    sw.Flush();
                    sw.Dispose();
                    remote_ms.Dispose();
                }
            }
        }

    }
}
