using STDLib.Saveable;
using System;

namespace ConnectionServer
{
    public sealed class Settings : BaseSettings<Settings>
    {
        public static int  ListenerPort { get { return GetPar<int>(1000); } set { SetPar(value); } }
        public static string LogFile { get { return GetPar("/data/Log.txt"); } set { SetPar(value); } }
    }
}
