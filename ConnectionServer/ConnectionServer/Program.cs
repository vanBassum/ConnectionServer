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

            JBVClient client = new JBVClient();

            DummyConnection con1 = new DummyConnection();
            DummyConnection con2 = new DummyConnection();
            DummyConnection.CoupleConnections(con1, con2);
            server.router.AddConnection(con2);
            System.Threading.Thread.Sleep(100);
            client.SetConnection(con1);
            


            Device.Client = client;

            
            BaseCommand.Register("Devices", ()=>{
                var v = Device.GetDevices(1000);
                foreach (Device d in v)
                    Console.WriteLine($"{d.ID} \t{d.SoftwareID.ToString()}");
            });

            BaseCommand.Register("LedOn", () => {
                var v = Device.GetDevices<DPS50xx>(1000);
                foreach (DPS50xx d in v)
                    d.SetLED(true);
                    
            });



            BaseCommand.Do();
        }

        
    }

}
