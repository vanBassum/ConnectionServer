using STDLib.Commands;
using STDLib.JBVProtocol;
using STDLib.Misc;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConServer
{
    class Program
    {


        static void Main(string[] args)
        {
            Settings.Load();
            ConnectionServer server = new ConnectionServer();


            server.Client.FrameRecieved += Client_FrameRecieved;

            BaseCommand.Register("Devices", (args) =>
            {
                Frame f = Frame.RequestSID(SoftwareID.Unknown);
                server.Client.SendFrame(f);
            });

            
            BaseCommand.Register("Log", (args) =>
            {
                Frame f = new Frame();
                f.RxID = 1;
                f.CommandID = (UInt32)LogHandler.CMDS.WriteLogString;
                f.SetData(Encoding.ASCII.GetBytes("hallo"));
                server.Client.SendFrame(f);
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

        private static void Client_FrameRecieved(object sender, Frame e)
        {
            switch ((CommandList)e.CommandID)
            {
                case CommandList.ReplySID:
                    SoftwareID id = (SoftwareID)BitConverter.ToUInt32(e.Data, 0);
                    LogRecievedCommand(e, id.ToString());
                    break;
            }



            
        }

        static void LogRecievedCommand(Frame cmd, string message = "")
        {
            if(message == "")
                Logger.LOGI($"{cmd.TxID}");
            else
                Logger.LOGI($"{cmd.TxID}: {message}");
        }
        
    }

}
