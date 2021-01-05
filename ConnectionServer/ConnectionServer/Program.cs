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

            
            BaseCommand.Register("Log", (args) =>
            {
                Frame f = new Frame();
                f.RxID = 1;
                f.CommandID = (UInt32)LogHandler.CMDS.WriteLogString;
                f.SetData(Encoding.ASCII.GetBytes("hallo"));
                server.Client.SendFrame(f);
            });


            BaseCommand.Register("Led", (args) =>
            {
                dps.SetLED(args[1] == "1");
            });

            BaseCommand.Register("Send", (args) =>
            {

                byte[] data = new byte[] { 0x01, 0x06, 0x00, 0x09, 0x00, 0x01 };
                AddCRC(ref data);
                dps.SendUART(data);

                
            });


            BaseCommand.Register("Test", (args) =>
            {

                dps.Test((byte)args[1][0]);

            });


            void AddCRC(ref byte[] data)
            {
                
                UInt16 crc = CRC16_2(data, data.Length);
                Array.Resize(ref data, data.Length + 2);

                data[data.Length - 2] = (byte)crc;
                data[data.Length - 1] = (byte)(crc>>8);
            }


            UInt16 CRC16_2(byte[] buf, int len)
            {
                UInt16 crc = 0xFFFF;
                for (int pos = 0; pos < len; pos++)
                {
                    crc ^= (UInt16)buf[pos];    // XOR byte into least sig. byte of crc

                    for (int i = 8; i != 0; i--)
                    {
                        if ((crc & 0x0001) != 0)
                        {      // If the LSB is set
                            crc >>= 1;                    // Shift right and XOR 0xA001
                            crc ^= 0xA001;
                        }
                        else                            // Else LSB is not set
                            crc >>= 1;                    // Just shift right
                    }
                }

                return crc;
            }



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
