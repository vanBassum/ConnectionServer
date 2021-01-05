using STDLib.JBVProtocol;
using STDLib.Misc;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;


namespace ConServer
{
    public class LeaseHandler
    {
        public IConnection Connection { get; private set; }
        SoftwareID softwareID = SoftwareID.LeaseServer;
        Lease lease = new Lease();
        ConcurrentDictionary<Guid, Lease> leases = new ConcurrentDictionary<Guid, Lease>();
        Framing framing = new Framing();
        
        Task task;
        BlockingCollection<Frame> pendingFrames = new BlockingCollection<Frame>();

        public LeaseHandler()
        {
            lease.ID = 0;                       //The leaseserver is always at id 0.
            lease.Expire = DateTime.MaxValue;   //The leaseserver's lease never expires.
            leases[lease.Key] = lease;

            framing.OnFrameCollected += (sender, frame) => pendingFrames.Add(frame);

            task = new Task(Work);
            task.Start();

            SetConnection(new DummyConnection());
        }

        public void SetConnection(IConnection con)
        {
            Connection = con;
            Connection.OnDataRecieved += (sender, data) => framing.Unstuff(data);
        }

        void Work()
        {
            while (true)
            {
                Frame frame = pendingFrames.Take();
                if (frame != null)
                {
                    CommandList cmd = (CommandList)frame.CommandID;
                    switch (cmd)
                    {
                        case CommandList.RequestLease:
                            HandleRequestLease(frame);
                            break;
                        case CommandList.RequestID:
                            UInt16 id = BitConverter.ToUInt16(frame.Data, 0);
                            if (id == this.lease.ID)
                            {
                                Frame f = Frame.ReplyID(this.lease.ID);
                                SendFrame(f);
                            }
                            break;
                        case CommandList.RequestSID:
                            SoftwareID sid = (SoftwareID)BitConverter.ToUInt32(frame.Data, 0);
                            if (sid == SoftwareID.Unknown || sid == softwareID)
                            {
                                Frame f = Frame.ReplySID(this.softwareID);
                                SendFrame(f);
                            }
                            break;
                        case CommandList.ReplyID:
                            break;
                        case CommandList.ReplyLease:
                            break;
                        default:
                            Logger.LOGW($"Command not handled '{frame.Options.ToString()}', '{cmd.ToString()}'");
                            break;
                    }
                }
            }
        }


        public void HandleRequestLease(Frame rx)
        {
            Guid key = new Guid(rx.Data);
            Lease lease = null;
            lock (leases)
            {
                if (leases.TryGetValue(key, out lease))
                {
                    //Extend lease
                    lease.Expire = DateTime.Now.AddMinutes(10);
                }
                else
                {
                    lease = new Lease();
                    lease.Expire = DateTime.Now.AddMinutes(10);
                    lease.Key = key;

                    //TODO: Reuse expired leases.
                    //TODO: Optimize this!
                    UInt16 id = Settings.Items.LeaseStartID;
                    while (leases.Any(a => a.Value.ID == id))
                        id++;
                    lease.ID = id;

                    leases[lease.Key] = lease;
                }
            }
            ReplyLease(lease);
            Logger.LOGI($"New lease accepted {lease.ToString()}");
        }


        void ReplyLease(Lease lease)
        {
            Frame f = Frame.ReplyLease(lease);
            SendFrame(f);
        }


        public void SendFrame(Frame frame)
        {
            frame.TxID = lease.ID;
            if (Connection != null)
                Connection.SendData(framing.Stuff(frame));
            else
                Logger.LOGE("No connection");
        }

    }

}