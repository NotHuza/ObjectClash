namespace CR.Servers.CoC.Packets.Messages.Server.Home
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;
    using CR.Servers.Logic.Enums;

    internal class Village2VisitAvatarDataMessage : Message
    {
        internal Level Visit;

        internal Village2VisitAvatarDataMessage(Device Device, Level Player) : base(Device)
        {
            this.Visit = Player;
            this.Visit.Tick();
            this.Device.State = State.VISIT;
        }

        internal override short Type
        {
            get
            {
                return 25020;
            }
        }

        internal override void Encode()
        {
            this.Data.AddLong(this.Visit.Player.UserId);
            this.Visit.Home.Encode(this.Data);
            this.Data.AddBool(true);
            {
                this.Visit.Player.Encode(this.Data);
            }
            this.Data.AddInt(0);
        }
    }
}