namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Boost_Building : Command
    {
        internal int Count;

        public Boost_Building(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 526;
            }
        }

        internal override void Decode()
        {
            this.Count = this.Reader.ReadInt32();
            for (int i = 0; i < this.Count; i++)
            {
                this.Reader.ReadInt32();
            }

            base.Decode();
        }
    }
}