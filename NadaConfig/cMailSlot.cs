using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.IO;

namespace cMailSlot
{
    public sealed class MailSlot: IDisposable
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

        public uint Max_MessageSize {get; set;}
        public string Name {get; private set;}
        public ScopeType Scope {get; private set;}
        public SlotType Type {get; private set;}
        public string RemoteName {get; private set;}
        public string Filename {get; private set;}

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

  
        public MailSlot(): this(@"DefaultMailSlot")
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
            switch (Scope) {
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
}
