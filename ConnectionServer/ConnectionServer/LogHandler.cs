using STDLib.JBVProtocol;
using STDLib.Saveable;
using STDLib.Serializers;
using System;
using System.Collections.Generic;
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
        Dictionary<string, LogList<string>> logfiles = new Dictionary<string, LogList<string>>();
        JSON json = new JSON();

        public LogHandler()
        {
            client.FrameRecieved += Client_FrameRecieved;
            
        }

        class L
        {
            public string FileName { get; set; }
            public string Data { get; set; }
        }


        private void Client_FrameRecieved(object sender, Frame e)
        {
            switch ((CMDS)e.CommandID)
            {
                case CMDS.WriteLogString:
                    logList.Add(new LogEntry(Encoding.ASCII.GetString(e.Data)));
                    break;
                case CMDS.WriteLogToFile:
                    L l = json.Deserialize<L>(Encoding.ASCII.GetString(e.Data));
                    if (!logfiles.ContainsKey(l.FileName))
                        logfiles[l.FileName] = new LogList<string>(l.FileName);
                    logfiles[l.FileName].Add(l.Data);
                    break;
            }
        }


        public enum CMDS
        {
            WriteLogString = 1,
            WriteLogToFile = 2,
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
