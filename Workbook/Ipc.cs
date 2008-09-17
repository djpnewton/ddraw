using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Security.AccessControl;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

namespace Workbook
{
    public enum IpcMessage { Show, ScreenAnnotate };
    public delegate void MessageReceivedHandler(IpcMessage msg);
    public class IpcMessager : MarshalByRefObject
    {
        public event MessageReceivedHandler MessageReceived;

        public IpcMessager()
        {

        }

        public void SendMessage(IpcMessage msg)
        {
            if (MessageReceived != null)
                MessageReceived(msg);
        }

        private int callCount = 0;

        public int GetCount()
        {
            Console.WriteLine("GetCount has been called.");
            callCount++;
            return (callCount);
        }
    }

    public enum IpcP2pMessage { Bork, QueryAutoSaveFile, ConfirmAutoSaveFile, RejectAutoSaveFile };
    public delegate IpcP2pMessage P2pMessageReceivedHandler(IpcP2pMessage msg, string data);
    public class IpcP2pMessager : MarshalByRefObject
    {
        public event P2pMessageReceivedHandler MessageReceived;

        public IpcP2pMessager()
        {

        }

        public IpcP2pMessage SendMessage(IpcP2pMessage msg, string data)
        {
            if (MessageReceived != null)
                return MessageReceived(msg, data);
            return IpcP2pMessage.Bork;
        }
    }

    public class Ipc : MarshalByRefObject
    {
        static Ipc ipc = null;
        public static Ipc GlobalIpc
        {
            get
            {
                if (ipc == null)
                    ipc = new Ipc();
                return ipc;
            }
        }

        public Ipc()
        {
            CreateMutex();
            if (!mutexUnauthorized)
                CreateIpcServerChannel();
            else
                CreateIpcClientChannel();
            CreateIpcP2pServerChannel();
        }

        // Mutex stuff ///////////////////////////////////

        public bool MutexUnauthorized
        {
            get { return mutexUnauthorized; }
        }

        const string mutexName = "4455a490-e96e-11dc-95ff-0800200c9a66";
        bool mutexDoesNotExist = false;
        bool mutexUnauthorized = false;
        bool mutexCreated = false;
        Mutex mutex;

        void CreateMutex()
        {
            // Attempt to open the named mutex.
            try
            {
                // Open the mutex with (MutexRights.Synchronize |
                // MutexRights.Modify), to enter and release the
                // named mutex.
                //
                mutex = Mutex.OpenExisting(mutexName);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                mutexDoesNotExist = true;
            }
            catch (UnauthorizedAccessException)
            {
                mutexUnauthorized = true;
            }

            if (mutexDoesNotExist)
            {
                // The mutex does not exist, so create it.

                // Create an access control list (ACL) that denies the
                // current user the right to enter or release the mutex
                string user = Environment.UserDomainName + "\\" + Environment.UserName;
                MutexSecurity mSec = new MutexSecurity();

                MutexAccessRule rule = new MutexAccessRule(user,
                    MutexRights.Synchronize | MutexRights.Modify | MutexRights.ChangePermissions,
                    AccessControlType.Deny);
                mSec.AddAccessRule(rule);

                rule = new MutexAccessRule(user,
                    MutexRights.ReadPermissions, AccessControlType.Allow);
                mSec.AddAccessRule(rule);

                // Create a Mutex object that represents the system
                // mutex named by the constant 'mutexName', with
                // initial ownership for this thread, and with the
                // specified security access. The Boolean value that 
                // indicates creation of the underlying system object
                // is placed in mutexWasCreated.
                mutex = new Mutex(true, mutexName, out mutexCreated, mSec);
            }
        }

        // Named Pipe stuff ///////////////////////////////////
        // see http://dotnetaddict.dotnetdevelopersjournal.com/ipc_remoting_real_world_example.htm

        public event MessageReceivedHandler MessageReceived;
        const string channelServerName = mutexName;
        const string IpcMessagerUri = "IpcMessager.rem";
        string IpcMessagerUrl
        {
            get { return string.Format("ipc://{0}/{1}", channelServerName, IpcMessagerUri); }
        }
        IpcChannel serverChan;

        public event P2pMessageReceivedHandler P2pMessageRecieved;
        public string channelP2pServerName;
        const string IpcP2pMessagerUri = "IpcP2pMessager.rem";
        string IpcP2pMessagerUrl
        {
            get { return string.Format("ipc://{0}/{1}", channelP2pServerName, IpcP2pMessagerUri); }
        }
        IpcChannel p2pServerChan;

        void CreateIpcServerChannel()
        {
            try
            {
                // need this or we can't use delegates and remoting.
                BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();
                serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
                
                BinaryClientFormatterSinkProvider clientProv = new BinaryClientFormatterSinkProvider();
                // ipc channel properties
                Dictionary<string, string> props = new Dictionary<string, string>();  
                props.Add("authorizedGroup", "Everyone");
                props.Add("portName", channelServerName);  
                props.Add("exclusiveAddressUse", "false");
                props.Add("name", channelServerName);
                // create and register channel
                serverChan = new IpcChannel(props, clientProv, serverProv);
                ChannelServices.RegisterChannel(serverChan, true);
                // register remote object
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(IpcMessager),
                    IpcMessagerUri, WellKnownObjectMode.Singleton);
                // access remote object and hookup event
                IpcMessager service = (IpcMessager)Activator.GetObject(typeof(IpcMessager), IpcMessagerUrl);
                service.MessageReceived += new MessageReceivedHandler(ipcMessager_MessageReceived);
            }
            catch (Exception)
            { }
        }

        void ipcMessager_MessageReceived(IpcMessage msg)
        {
            if (MessageReceived != null)
                MessageReceived(msg);
        }

        void CreateIpcClientChannel()
        {
            // create ipc client channel
            serverChan = new IpcChannel();
            ChannelServices.RegisterChannel(serverChan, true);
            // Register as client for remote object.
            WellKnownClientTypeEntry remoteType = new WellKnownClientTypeEntry(typeof(IpcMessager), IpcMessagerUrl);
            RemotingConfiguration.RegisterWellKnownClientType(remoteType);

        }

        public void SendMessage(IpcMessage msg)
        {
            // grab the remote object.
            IpcMessager service = (IpcMessager)Activator.GetObject(typeof(IpcMessager), IpcMessagerUrl);
            // send message
            if (service != null)
            {
                service.SendMessage(msg);
                int n = service.GetCount();
                Console.WriteLine("The remote object has been called {0} times.", n);
            }
        }

        void CreateIpcP2pServerChannel()
        {
            try
            {
                // need this or we can't use delegates and remoting.
                BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();
                serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

                BinaryClientFormatterSinkProvider clientProv = new BinaryClientFormatterSinkProvider();
                // ipc channel properties
                Dictionary<string, string> props = new Dictionary<string, string>();
                props.Add("authorizedGroup", "Everyone");
                channelP2pServerName = Guid.NewGuid().ToString();
                props.Add("portName", channelP2pServerName);
                props.Add("exclusiveAddressUse", "false");
                props.Add("name", channelP2pServerName);
                // create and register channel
                p2pServerChan = new IpcChannel(props, clientProv, serverProv);
                ChannelServices.RegisterChannel(p2pServerChan, true);
                // register remote object
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(IpcP2pMessager),
                    IpcP2pMessagerUri, WellKnownObjectMode.Singleton);
                // access remote object and hookup event
                IpcP2pMessager service = (IpcP2pMessager)Activator.GetObject(typeof(IpcP2pMessager), IpcP2pMessagerUrl);
                service.MessageReceived += new P2pMessageReceivedHandler(ipcP2pMessager_MessageReceived);
            }
            catch (Exception)
            { }
        }

        IpcP2pMessage ipcP2pMessager_MessageReceived(IpcP2pMessage msg, string data)
        {
            if (P2pMessageRecieved != null)
                return P2pMessageRecieved(msg, data);
            return IpcP2pMessage.Bork;
        }

        public IpcP2pMessage SendP2pMessage(string p2pServerName, IpcP2pMessage msg, string data)
        {
            // grab the remote object.
            string ipcP2pMessagerUrl = string.Format("ipc://{0}/{1}", p2pServerName, IpcP2pMessagerUri);
            IpcP2pMessager service = (IpcP2pMessager)Activator.GetObject(typeof(IpcP2pMessager), ipcP2pMessagerUrl);
            // send message
            if (service != null)
                try
                {
                    return service.SendMessage(msg, data);
                }
                catch { }
            return IpcP2pMessage.Bork;
        }
    }
}
