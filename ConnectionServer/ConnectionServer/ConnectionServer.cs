using STDLib.Ethernet;
using STDLib.JBVProtocol;
using STDLib.JBVProtocol.Connections;
using STDLib.JBVProtocol.IO;
using STDLib.Misc;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ConnectionServer
{
    public class ConnectionServer
    {
        readonly LeaseServer iDServer;
        readonly Router router;
        readonly TcpSocketListener listener;



        public ConnectionServer()
        {
            Settings.Load(true);

            DummyConnection con_ID_Router = new DummyConnection();
            DummyConnection con_Router_ID = new DummyConnection();
            DummyConnection.CoupleConnections(con_ID_Router, con_Router_ID);

            iDServer = new LeaseServer(con_ID_Router);

            router = new Router();
            router.AddConnection(con_Router_ID);
            listener = new TcpSocketListener();
            listener.OnClientAccept += Listener_OnClientAccept;
            listener.BeginListening(Settings.ListenerPort);

        }

        ~ConnectionServer()
        {
            Settings.Save();
        }

        private void Listener_OnClientAccept(object sender, TcpSocketClient e)
        {
            TCPConnection con = new TCPConnection(e);
            router.AddConnection(con);
            Console.WriteLine("Client accepted");
        }
    }

}
