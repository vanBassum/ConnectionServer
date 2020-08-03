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
        readonly Router router;
        readonly TcpSocketListener listener;
        readonly Client client;


        public ConnectionServer()
        {
            Settings.Save("Settings.json");
            Logger.SetFile(Settings.LogFile, true);
            router = new Router(Settings.RouterID);
            listener = new TcpSocketListener();
            listener.OnClientAccept += Listener_OnClientAccept;
            listener.BeginListening(Settings.ListenerPort);
            client = new Client(Settings.ClientID);

            DummyConnection con1 = new DummyConnection();
            DummyConnection con2 = new DummyConnection();
            DummyConnection.CoupleConnections(con1, con2);
            router.AddConnection(con1);
            client.SetConnection(con2);
            client.OnMessageRecieved += Client_OnMessageRecieved;
            client.OnBroadcastRecieved += Client_OnBroadcastRecieved;
        }

        private void Client_OnBroadcastRecieved(object sender, Message e)
        {
            //throw new NotImplementedException();
        }

        private void Client_OnMessageRecieved(object sender, Message e)
        {
            switch(e.Payload[0])
            {
                case 0:
                    Logger.WriteLine(Regex.Escape(Encoding.ASCII.GetString(e.Payload, 1, e.Payload.Length -1)));
                    break;
            }
            
            //throw new NotImplementedException();
        }

        ~ConnectionServer()
        {
            Settings.Load("Settings.json");
        }

        private void Listener_OnClientAccept(object sender, TcpSocketClient e)
        {
            TCPConnection con = new TCPConnection(e);
            router.AddConnection(con);
            Logger.WriteLine("Accept connection");
        }
    }

}
