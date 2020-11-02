using STDLib.Commands;
using STDLib.JBVProtocol;
using STDLib.JBVProtocol.Commands;
using STDLib.JBVProtocol.Devices;
using STDLib.JBVProtocol.DSP50xx;
using STDLib.JBVProtocol.DSP50xx.CMD;
using System;
using System.Linq;
using System.Threading;

namespace ConServer
{
    class Program
    {


        static void Main(string[] args)
        {
            ConnectionServer server = new ConnectionServer();

            Device.Init(server.Client);


            server.Client.CommandRecieved += HandleRecievedCommand;

            BaseCommand.Register("Devices", (args) =>
            {
                RequestSID cmd = new RequestSID();
                server.Client.SendCMD(cmd);
            });


            BaseCommand.Register("Boeh", (args) =>
            {
                Device.OnDeviceFound += Device_OnDeviceFound;
                Device.SearchDevices();
                
            });



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

        private static void Device_OnDeviceFound(object sender, Device e)
        {
            Logger.LOGI($"Device found{e.GetType().Name}");
            if(e is DPS50xx dev)
            {
                dev.SetLED(true);
            }
        }

        private static void HandleRecievedCommand(object sender, Command e)
        {
            switch (e)
            {
                case ReplySID cmd:
                    LogRecievedCommand(cmd, $"{cmd.SID}");
                    break;

                default:
                    LogRecievedCommand(e);
                    break;
            }
        }

        static void LogRecievedCommand(Command cmd, string message = "")
        {
            if(message == "")
                Logger.LOGI($"{cmd.TxID}, {cmd.GetType().Name}");
            else
                Logger.LOGI($"{cmd.TxID}, {cmd.GetType().Name}: {message}");
        }

    }

}
