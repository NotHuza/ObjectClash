namespace CR.Servers.CoC.Packets.Messages.Server.Alliances
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.Extensions.List;

    internal class AllianceListMessage : Message
    {
        internal Alliance[] Alliances;

        internal string TextSearch;

        public AllianceListMessage(Device device) : base(device)
        {
        }

        internal override short Type
        {
            get
            {
                return 24310;
            }
        }

        internal override void Encode()
        {
            this.Data.AddString(this.TextSearch);
            this.Data.AddInt(this.Alliances.Length);

            foreach (Alliance Alliance in this.Alliances)
            {
                Alliance.Header.Encode(this.Data);
            }

            this.Data.AddInt(0); //Another list but no one know what is it for
        }
    }
}