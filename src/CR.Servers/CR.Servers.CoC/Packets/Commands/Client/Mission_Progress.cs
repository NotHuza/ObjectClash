namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Extensions.Game;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Mission_Progress : Command
    {
        internal int Mission_ID;

        public Mission_Progress(Device Client, Reader Reader) : base(Client, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 519;
            }
        }

        internal override void Decode()
        {
            this.Mission_ID = this.Reader.ReadInt32();
            base.Decode();
        }

        internal override void Execute()
        {
            if (this.Device.Account.Player.Mission_Finish(this.Mission_ID))
            {
                // Missions Mission = CSV.Tables.Get(Gamefile.Missions).GetDataWithID(Mission_ID) as Missions;
            }
        }
    }
}