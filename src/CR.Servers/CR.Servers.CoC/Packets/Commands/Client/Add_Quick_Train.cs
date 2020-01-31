namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Add_Quick_Train : Command
    {
        internal int Count;

        internal int Database;

        public Add_Quick_Train(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 558;
            }
        }

        internal override void Decode()
        {
            this.Database = this.Reader.ReadInt32();
            this.Count = this.Reader.ReadInt32();

            for (int i = 0; i < this.Count; i++)
            {
                this.Reader.ReadInt32(); //Global Id
                this.Reader.ReadInt32(); //Count
            }

            base.Decode();
        }
    }
}