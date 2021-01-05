using STDLib.Commands;
using STDLib.JBVProtocol;
using STDLib.JBVProtocol.Devices;
using STDLib.Misc;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConServer
{
    class Program
    {

        static DPS50XX dps = new DPS50XX();
        static void Main(string[] args)
        {
            Settings.Load();
            ConnectionServer server = new ConnectionServer();
            

            server.Client.FrameRecieved += Client_FrameRecieved;

            dps.JBVClient = server.Client;

            BaseCommand.Register("Devices", (args) =>
            {
                Frame f = Frame.RequestSID(SoftwareID.Unknown);
                server.Client.SendFrame(f);
            });


            BaseCommand.Register("Led", (args) =>
            {
                dps.SetLED(args[1] == "1");
            });

            BaseCommand.Do();
        }

        private static void Client_FrameRecieved(object sender, Frame e)
        {
            switch ((CommandList)e.CommandID)
            {
                case CommandList.ReplySID:
                    SoftwareID id = (SoftwareID)BitConverter.ToUInt32(e.Data, 0);
                    LogRecievedCommand(e, id.ToString());
                    if(id == SoftwareID.DPS50xx)
                    {
                        dps.ID = e.TxID;                        
                    }


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
