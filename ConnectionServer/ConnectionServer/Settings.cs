using STDLib.Saveable;
using System;

namespace ConnectionServer
{
    public sealed class Settings : BaseSettings
    {
        public static int  ListenerPort { get { return GetPar<int>(1000); } set { SetPar(value); } }
        public static UInt16 RouterID { get { return GetPar<UInt16>(0xFFFF); } set { SetPar(value); } }
        public static UInt16 ClientID { get { return GetPar<UInt16>(0xFFFE); } set { SetPar(value); } }
        public static string LogFile { get { return GetPar("Log.txt"); } set { SetPar(value); } }
    }
}
