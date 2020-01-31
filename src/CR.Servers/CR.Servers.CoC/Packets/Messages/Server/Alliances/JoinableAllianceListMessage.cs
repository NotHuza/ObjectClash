namespace CR.Servers.CoC.Packets.Messages.Server.Alliances
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.Extensions.List;

    internal class JoinableAllianceListMessage : Message
    {
        internal Alliance[] Alliances;

        public JoinableAllianceListMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 24304;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt(this.Alliances.Length);
            foreach (Alliance Alliance in this.Alliances)
            {
                Alliance.Header.Encode(this.Data);
            }

            this.Data.AddInt(0); //Another list but no one know what is it for
        }
    }
}