namespace CR.Servers.CoC.Packets.Messages.Server.Home
{
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;
    using CR.Servers.Logic.Enums;

    internal class OwnHomeDataMessage : Message
    {
        public OwnHomeDataMessage(Device Device) : base(Device)
        {
            Device.GameMode.Level.Tick();
        }

        internal override short Type
        {
            get
            {
                return 24101;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt(0);
            this.Data.AddInt(-1);
            this.Data.AddInt(TimeUtils.UnixUtcNow);
            this.Device.Account.Home.Encode(this.Data);
            this.Device.Account.Player.Encode(this.Data);
            this.Data.AddInt(this.Device.State == State.WAR_EMODE ? 1 : 0);
            this.Data.AddInt(1);
            this.Data.AddInt(0);

            this.Data.AddLong(1462629754000);
            this.Data.AddLong(1462629754000);
            this.Data.AddLong(1462629754000);
            this.Data.AddInt(0);
        }
    }
}