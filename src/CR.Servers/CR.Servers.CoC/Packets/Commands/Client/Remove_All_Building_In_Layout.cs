namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Remove_All_Building_In_Layout : Command
    {
        public Remove_All_Building_In_Layout(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 569;
            }
        }

        internal override void Decode()
        {
            base.Decode();
            this.Reader.ReadInt32();
            this.Reader.ReadByte();
        }
    }
}