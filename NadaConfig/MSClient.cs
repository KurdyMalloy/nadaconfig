using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cMailSlot;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace NadaConfigClient
{
    class MailslotClient
    {
        string environmentName;
        string scopeName;

        public string configHostName;
        public string configServiceName;

        public string msgwaiting;


        private MailslotClient()
        {
        }

        public MailslotClient(string environment)
        {
            environmentName = environment;
            scopeName = "*";
        }

        public MailslotClient(string environment, string scope)
        {
            environmentName = environment;
            scopeName = scope;
        }

        public bool FindService(int timeout)
        {

            MailSlot listening_ms = null, ms = null;
            StreamWriter sw = null;
            bool listenOK = false;  //flag to make sure listening mailslot is setup OK
            bool done = false;  // flag to know if we did timeout or not

            try
            {
                // Create Listening mailslot
                Process currentProcess = Process.GetCurrentProcess();
                string mailslotname = @"nadaConfigListener" + currentProcess.Id.ToString();
                listening_ms = new MailSlot(mailslotname);
                //Creating Remote mailslot
                ms = new MailSlot(environmentName, scopeName);
                sw = new StreamWriter(ms.FStream);
                String message = String.Format(@"{0}|{1}|{2}", Environment.MachineName + DateTime.Now.ToBinary().ToString(), Environment.MachineName, mailslotname);
                sw.WriteLine(message);
                sw.Flush();
                sw.Dispose();
                ms.Dispose();
                listenOK = true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                System.Diagnostics.Trace.WriteLine(e.StackTrace);
                if (listening_ms != null)
                    listening_ms.Dispose();
                listenOK = false;
            }

            if (listenOK)
            {
                StreamReader sr = new StreamReader(listening_ms.FStream);
                bool timedout = false;
                int numtry = 0;

                while (!done && !timedout)
                {
                    try
                    {
                        numtry += 1;
                        if (listening_ms.isMessagesWaiting > 0)
                        {
                            //System.Diagnostics.Trace.WriteLine(string.Format(@"Messages {0}, {1}", listening_ms.isMessagesWaiting, sr.ReadLine()));
                            string message = sr.ReadLine();
                            var msgParts = message.Split('|');
                            configServiceName = msgParts[1];
                            configHostName = msgParts[0];
                            done = true;
                            msgwaiting = listening_ms.isMessagesWaiting.ToString();
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                        System.Diagnostics.Trace.WriteLine(e.StackTrace);
                        //return false;
                    }
                    if (!done)
                    {
                        Thread.Sleep(100);
                        if ((numtry * 100) > timeout)
                            timedout = true;
                    }
                }
                sr.Dispose();
                listening_ms.Dispose();
            }

            if (done)
                return true;

            return false;
        }
    }
}
