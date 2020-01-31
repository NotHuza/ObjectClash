namespace CR.Servers.CoC.Packets.Commands.Client.Battle
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Search_Opponent_V2 : Command
    {
        internal int UnknownByte;

        internal int UnknownInt;

        public Search_Opponent_V2(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 601;
            }
        }

        internal override void Decode()
        {
            this.UnknownInt = this.Reader.ReadInt32();
            this.UnknownByte = this.Reader.ReadByte();
            base.Decode();
        }

        internal override void Execute()
        {
            Resources.Duels.Join(this.Device);
        }
    }
}