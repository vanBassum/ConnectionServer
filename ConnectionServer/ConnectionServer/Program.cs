using STDLib.Commands;
using STDLib.JBVProtocol;
using STDLib.JBVProtocol.Commands;
using STDLib.JBVProtocol.DSP50xx.CMD;
using System;
using System.Linq;

namespace ConServer
{
    class Program
    {


        static void Main(string[] args)
        {
            ConnectionServer server = new ConnectionServer();

            server.Client.CommandRecieved += HandleRecievedCommand;

            BaseCommand.Register("Devices", (args) =>
            {
                RequestSID cmd = new RequestSID();
                server.Client.SendCMD(cmd);
            });


            BaseCommand.Register("SetLED", (args) =>
            {
                int devid;
                int val;

                if(int.TryParse(args[1], out devid) && int.TryParse(args[2], out val))
                {
                    SetLED cmd = new SetLED();
                    cmd.RxID = (UInt16)devid;
                    cmd.Led = val != 0;
                    server.Client.SendCMD(cmd);
                }


                
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
