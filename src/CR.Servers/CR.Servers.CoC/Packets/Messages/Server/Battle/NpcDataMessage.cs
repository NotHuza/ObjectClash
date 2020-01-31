namespace CR.Servers.CoC.Packets.Messages.Server.Battle
{
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;
    using CR.Servers.Logic.Enums;

    internal class NpcDataMessage : Message
    {
        internal int Npc_ID;

        internal Home NpcHome;

        internal bool Village2;

        public NpcDataMessage(Device Device) : base(Device)
        {
            Device.State = State.IN_NPC_BATTLE;
        }

        internal override short Type
        {
            get
            {
                return 24133;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt(0);
            this.Data.AddInt(TimeUtils.UnixUtcNow);
            this.NpcHome.Encode(this.Data);
            this.Device.GameMode.Level.Player.Encode(this.Data);

            this.Data.AddInt(this.Npc_ID);

            this.Data.AddBool(this.Village2);
        }
    }
}