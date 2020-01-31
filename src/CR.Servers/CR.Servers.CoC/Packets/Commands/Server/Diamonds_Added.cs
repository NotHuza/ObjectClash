namespace CR.Servers.CoC.Packets.Commands.Server
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Extensions.List;

    internal class Diamonds_Added : ServerCommand
    {
        internal bool AlliangeGift;

        internal int Count;

        public Diamonds_Added(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        public Diamonds_Added(Device Device) : base(Device)
        {
        }

        internal override int Type
        {
            get
            {
                return 7;
            }
        }

        //internal BillingPackageData BillingPackageData;
        //internal GemBundleData GemBundleData;

        internal override void Decode()
        {
            this.AlliangeGift = this.Reader.ReadBoolean();

            this.Count = this.Reader.ReadInt32();

            this.Reader.ReadInt32();
            this.Reader.ReadInt32();
            /*this.BillingPackageData = Reader.ReadData<BillingPackageData>();
            this.GemBundleData = Reader.ReadData<GemBundleData>();*/

            this.Reader.ReadBoolean();

            this.Reader.ReadString(); //TransactionID

            base.Decode();
        }

        internal override void Encode(List<byte> Data)
        {
            Data.AddBool(this.AlliangeGift);

            Data.AddInt(this.Count);
            Data.AddInt(0);
            Data.AddInt(0);
            /*this.Data.AddData(this.BillingPackageData);
            this.Data.AddData(this.GemBundleData);*/
            Data.AddBool(this.AlliangeGift);

            Data.AddString(null); // TransactionID

            base.Encode(Data);
        }

        internal override void Execute()
        {
            this.Device.GameMode.Level.Player.AddDiamonds(this.Count);
        }
    }
}