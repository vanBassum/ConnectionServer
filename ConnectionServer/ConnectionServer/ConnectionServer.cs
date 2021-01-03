using STDLib.Ethernet;
using STDLib.JBVProtocol;

namespace ConServer
{

    public class ConnectionServer
    {
        public JBVClient Client { get; }
        Router router;
        TcpSocketListener listener;
        LeaseHandler leaseHandler;
        LogHandler logHandler;


        public ConnectionServer()
        {
            Client = new JBVClient(SoftwareID.ConnectionServer);
            router = new Router();
            leaseHandler = new LeaseHandler();
            logHandler = new LogHandler();




            DummyConnection cRouter1 = new DummyConnection();
            DummyConnection cRouter2 = new DummyConnection();
            DummyConnection cRouter3 = new DummyConnection();
            router.AddConnection(cRouter1);
            router.AddConnection(cRouter2);
            router.AddConnection(cRouter3);


            DummyConnection.CoupleConnections(cRouter1, Client.Connection);
            DummyConnection.CoupleConnections(cRouter2, leaseHandler.Connection);
            DummyConnection.CoupleConnections(cRouter3, logHandler.Connection);


            listener = new TcpSocketListener();
            listener.OnClientAccept += (sender, client) => router.AddConnection(new TCPConnection(client));
            listener.BeginListening(1000);
        }
    }
}
