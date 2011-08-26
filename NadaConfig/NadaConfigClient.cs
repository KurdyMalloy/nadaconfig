using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.ServiceModel;
//using System.Net;
using System.Diagnostics;


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

    [ServiceContract]
    public interface INadaConfigService
    {
        //[OperationContract]
        //string GetData(int value);

        //[OperationContract]
        //CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here

        [OperationContract]
        Dictionary<string, Dictionary<string, string>> GetEnvironment(string env);

        [OperationContract]
        Dictionary<string, string> GetSection(string env, string section);

        [OperationContract]
        string GetConfigItem(string env, string section, string item);

    }

    public sealed class MailSlot : IDisposable
    {
        private bool _disposed = false;
        private SafeFileHandle _handleValue = null;
        private FileStream _fileStream = null;

        public enum ScopeType
        {
            local,
            remote
        }

        public enum SlotType
        {
            reader,
            writer
        }

        public uint Max_MessageSize { get; set; }
        public string Name { get; private set; }
        public ScopeType Scope { get; private set; }
        public SlotType Type { get; private set; }
        public string RemoteName { get; private set; }
        public string Filename { get; private set; }

        public FileStream FStream
        {
            get
            {
                return _fileStream;
            }
        }
        //public SafeFileHandle Handle
        //{
        //    get
        //    {
        //        // If the handle is valid,
        //        // return it.
        //        if (!handleValue.IsInvalid)
        //            return handleValue;
        //        else
        //            return null;
        //    }
        //}

        public bool isReady
        {
            get
            {
                return ((_handleValue != null) && (_fileStream != null) && (!_handleValue.IsInvalid));
            }
        }

        public uint isMessagesWaiting
        {
            get
            {
                uint num_messages = 0;
                if (isReady && Type == SlotType.reader)
                    GetMailslotInfo(_handleValue, IntPtr.Zero, IntPtr.Zero, out num_messages, IntPtr.Zero);
                return num_messages;
            }
        }

        // P/Invoke signatures for required API imports
        #region DLL Imports
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern SafeFileHandle CreateMailslot(string lpName,
                                            uint nMaxMessageSize,
                                            uint lReadTimeout,
                                            IntPtr lpSecurityAttributes);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern bool GetMailslotInfo(SafeFileHandle hMailslot,
                                           IntPtr lpMaxMessageSize,
                                           IntPtr lpNextSize,
                                           out uint lpMessageCount,
                                           IntPtr lpReadTimeout);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern SafeFileHandle CreateFile(
              string lpFileName,
              [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess,
              [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
              IntPtr SecurityAttributes,
              [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
              uint dwFlagsAndAttributes,
              IntPtr hTemplateFile
              );
        #endregion


        public MailSlot()
            : this(@"DefaultMailSlot")
        {
        }

        public MailSlot(string name)
        {
            Max_MessageSize = 4096; // arbitrary buffer length
            Open(name);
        }

        public MailSlot(string name, string remote)
        {
            Max_MessageSize = 4096; // arbitrary buffer length
            Open(name, remote);
        }

        // To create server (reader); cannot be created remote
        public bool Open(string name)
        {
            //Do not reopen if we are disposed
            if (_disposed)
                return false;

            //Want to close before we reopen
            Close();

            Name = name;
            Scope = ScopeType.local;
            Type = SlotType.reader;

            CreateMailSlotHandle();
            CreateFileStreamHandle();

            return isReady;
        }

        // To create client (writer); remote value can be "computername", "domainname", or "*" for current domain you can also pass null or empty string in remote to consume local mailslot; 
        public bool Open(string name, string remote)
        {
            //Do not reopen if we are disposed
            if (_disposed)
                return false;

            //Want to close before we reopen
            Close();

            Name = name;
            Scope = ScopeType.remote;
            Type = SlotType.writer;
            RemoteName = remote;

            if (remote == null || remote.Length == 0 || remote == @".")
            {
                RemoteName = @".";
                Scope = ScopeType.local;
            }

            CreateMailSlotHandle();
            CreateFileStreamHandle();

            return isReady;
        }

        public void Close()
        {
            if (_handleValue != null)
            {
                _handleValue.Dispose();
                _handleValue = null;
            }
            if (_fileStream != null)
            {
                _fileStream.Dispose();
                _fileStream = null;
            }
        }

        private void CreateCanonicalSlotName()
        {
            StringBuilder ret = new StringBuilder(@"\\");
            switch (Scope)
            {
                case ScopeType.local:
                    ret.Append(@".\");
                    break;
                case ScopeType.remote:
                    ret.Append(RemoteName);
                    ret.Append(@"\");
                    break;
                default:
                    break;
            }
            ret.Append(@"mailslot\");
            ret.Append(Name);
            Filename = ret.ToString();
        }

        private void CreateMailSlotHandle()
        {

            CreateCanonicalSlotName();

            if (Type == SlotType.reader)
            {
                _handleValue = CreateMailslot(Filename, 0, 0, IntPtr.Zero);
            }
            else if (Type == SlotType.writer)
            {
                // Try to open the file.
                _handleValue = CreateFile(Filename, FileAccess.Write, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            }

            // If the handle is invalid,
            // get the last Win32 error 
            // and throw a Win32Exception.
            if (_handleValue.IsInvalid)
            {
                _handleValue = null;
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
        }

        private void CreateFileStreamHandle()
        {
            if ((_handleValue != null) && (!_handleValue.IsInvalid))
            {
                try
                {
                    if (Type == SlotType.reader)
                    {
                        _fileStream = new FileStream(_handleValue, FileAccess.Read);
                    }
                    else if (Type == SlotType.writer)
                    {
                        _fileStream = new FileStream(_handleValue, FileAccess.Write);
                    }
                }
                catch (Exception e)
                {
                    _fileStream = null;
                    throw (e);
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Close();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MailSlot()
        {
            Dispose(false);
        }
    }

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
