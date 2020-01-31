namespace CR.Servers.CoC.Packets.Messages.Server.Battle
{
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;
    using CR.Servers.Logic.Enums;

    internal class EnemyHomeDataMessage : Message
    {
        internal Level Enemy;
        internal Player Player;
        internal bool NextButton;

        public EnemyHomeDataMessage(Device Device, Level Enemy) : base(Device)
        {
            this.Enemy = Enemy;
            this.Enemy.Tick();

            this.Player = this.Device.GameMode.Level.Player;
            this.Device.GameMode.Level.Tick();
        }

        public EnemyHomeDataMessage(Device Device) : base(Device)
        {
            this.Player = this.Device.GameMode.Level.Player;
            this.Device.GameMode.Level.Tick();
        }

        internal override short Type
        {
            get
            {
                return 24107;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt(0);
            this.Data.AddInt(-1);

            this.Data.AddInt(TimeUtils.UnixUtcNow);

            this.Enemy.Home.Encode(this.Data);
            this.Enemy.Player.Encode(this.Data);

            this.Device.GameMode.Level.Player.Encode(this.Data);

            this.Data.AddInt(this.NextButton ? 3 : 2);
            this.Data.AddInt(0);
            this.Data.AddByte(0);
        }

        internal override void Process()
        {
            this.Device.State = State.IN_PC_BATTLE;
        }
    }
}