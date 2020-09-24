using STDLib.Commands;
using STDLib.JBVProtocol;
using STDLib.JBVProtocol.Connections;
using STDLib.JBVProtocol.Devices;
using System;
using System.Linq;

namespace ConnectionServer
{
    class Program
    {


        static void Main(string[] args)
        {
            ConnectionServer server = new ConnectionServer();

            JBVClient client = new JBVClient(SoftwareID.TestApp);

            DummyConnection con1 = new DummyConnection();
            DummyConnection con2 = new DummyConnection();
            DummyConnection.CoupleConnections(con1, con2);
            server.router.AddConnection(con2);
            System.Threading.Thread.Sleep(100);
            client.SetConnection(con1);
            


            Device.Client = client;

            BaseCommand.Register("bla", ()=>{
                var v = Device.GetDevices<STDLib.JBVProtocol.Devices.Router>(1000);
            });

            BaseCommand.Do();
        }

        
    }

}
