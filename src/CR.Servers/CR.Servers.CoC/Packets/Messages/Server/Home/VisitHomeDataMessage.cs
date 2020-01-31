namespace CR.Servers.CoC.Packets.Messages.Server.Home
{
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;
    using CR.Servers.Logic.Enums;

    internal class VisitHomeDataMessage : Message
    {
        internal Level Visit;

        internal VisitHomeDataMessage(Device Device, Level Player) : base(Device)
        {
            this.Visit = Player;
            this.Visit.Tick();
            this.Device.State = State.VISIT;
        }

        internal override short Type
        {
            get
            {
                return 24113;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt(0);
            this.Data.AddInt(TimeUtils.UnixUtcNow);
            this.Visit.Home.Encode(this.Data);
            this.Visit.Player.Encode(this.Data);
            this.Data.AddInt(0);
            this.Data.AddByte(1);
            this.Device.GameMode.Level.Player.Encode(this.Data);
        }
    }
}