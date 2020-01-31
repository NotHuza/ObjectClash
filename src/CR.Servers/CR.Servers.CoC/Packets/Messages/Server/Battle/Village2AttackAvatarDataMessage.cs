namespace CR.Servers.CoC.Packets.Messages.Server.Battle
{
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;
    using CR.Servers.Logic.Enums;

    internal class Village2AttackAvatarDataMessage : Message
    {
        internal Level Enemy;

        public Village2AttackAvatarDataMessage(Device Device) : base(Device)
        {
            this.Device.State = State.IN_1VS1_BATTLE;
        }

        internal override short Type
        {
            get
            {
                return 25023;
            }
        }

        internal override void Encode()
        {
            this.Device.GameMode.Level.Player.Encode(this.Data);

            this.Data.AddLong(this.Enemy.Player.UserId);
            this.Data.AddInt(0);
            this.Data.AddInt(0);
            this.Data.AddInt(0);
            this.Data.AddCompressed(this.Enemy.BattleV2().ToString());
            this.Data.AddCompressed(GameEvents.Events_Json);
            this.Data.AddCompressed("{\"Village2\":{\"TownHallMaxLevel\":8}}");

            this.Device.GameMode.Level.Home.Encode(this.Data);
            this.Data.AddLong(this.Enemy.Player.UserId);
            this.Data.AddInt(TimeUtils.UnixUtcNow);
        }
    }
}