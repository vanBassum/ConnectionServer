using STDLib.Commands;
using STDLib.JBVProtocol;
using System;
using System.Linq;

namespace ConServer
{
    class Program
    {


        static void Main(string[] args)
        {
            ConnectionServer server = new ConnectionServer();



            /*
            BaseCommand.Register("Devices", () => {
                var v = Device.GetDevices(1000);
                foreach (Device d in v)
                    Console.WriteLine($"{d.ID} \t{d.SoftwareID.ToString()}");
            });

            BaseCommand.Register("LedOn", () => {
                var v = Device.GetDevices<DPS50xx>(1000);
                foreach (DPS50xx d in v)
                    d.SetLED(true);

            });
            */

            BaseCommand.Do();
        }

        
    }

}
