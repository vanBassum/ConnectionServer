using STDLib.Saveable;
using System;

namespace ConServer
{
    public sealed class Settings : BaseSettingsV2<Settings>
    {
        public int  ListenerPort { get { return GetPar<int>(1000); } set { SetPar(value); } }
        public string LogFile { get { return GetPar("/data/Log.txt"); } set { SetPar(value); } }
        public int LeaseTime { get { return GetPar<int>(60*60); } set { SetPar(value); } }
        public UInt16 LeaseStartID { get { return GetPar<UInt16>(5); } set { SetPar(value); } }

    }
}
