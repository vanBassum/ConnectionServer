using STDLib.JBVProtocol;
using STDLib.Saveable;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace ConServer
{
    public class LogHandler
    {
        JBVClient client = new JBVClient(SoftwareID.LogHandler, 1);
        public IConnection Connection { get { return client.Connection; } }
        LogList<LogEntry> logList = new LogList<LogEntry>("Loghandler.json");

        public LogHandler()
        {
            client.FrameRecieved += Client_FrameRecieved;
            
        }

        private void Client_FrameRecieved(object sender, Frame e)
        {
            switch ((CMDS)e.CommandID)
            {
                case CMDS.WriteLogString:
                    logList.Add(new LogEntry(Encoding.ASCII.GetString(e.Data)));
                    break;
            }


        }


        public enum CMDS
        {
            WriteLogString = 1,

        }

        class LogEntry
        { 
            public DateTime Timestamp { get; set; }
            public object Data { get; set; }

            public LogEntry()
            {

            }

            public LogEntry(object data)
            {
                Timestamp = DateTime.Now;
                Data = data;
            }
        }
    }
}
