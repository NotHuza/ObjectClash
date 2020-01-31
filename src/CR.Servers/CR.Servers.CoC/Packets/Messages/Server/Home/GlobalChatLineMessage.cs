namespace CR.Servers.CoC.Packets.Messages.Server.Home
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;

    internal class GlobalChatLineMessage : Message
    {
        internal bool Bot;
        internal int ExpLevel;
        internal int League;

        internal string Message;
        internal string Name;

        internal Player Player;
        internal long UserId;

        public GlobalChatLineMessage(Device Device) : base(Device)
        {
        }

        public GlobalChatLineMessage(Device Device, Player Player) : base(Device)
        {
            this.Player = Player;
            this.Name = this.Player.Name;
            this.ExpLevel = this.Player.ExpLevel;
            this.League = this.Player.League;
            this.UserId = this.Player.UserId;
        }

        internal override short Type
        {
            get
            {
                return 24715;
            }
        }

        internal override void Encode()
        {
            this.Data.AddString(this.Message);
            this.Data.AddString(this.Name);
            this.Data.AddInt(this.ExpLevel);
            this.Data.AddInt(this.League);
            this.Data.AddLong(this.UserId);
            this.Data.AddLong(this.UserId);

            if (!this.Bot && this.Player.InAlliance)
            {
                this.Data.AddBool(true);

                this.Data.AddLong(this.Player.AllianceId);
                this.Data.AddString(this.Player.Alliance.Header.Name);
                this.Data.AddInt(this.Player.Alliance.Header.Badge);
            }
            else
            {
                this.Data.AddBool(false);
            }
        }
    }
}