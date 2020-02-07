namespace CR.Servers.CoC.Packets.Messages.Server.Alliances
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.Extensions.List;
    using CR.Servers.Logic.Enums;
    using CR.Servers.CoC.Packets.Enums;
    using System.Linq;

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

        internal ServiceNode Node
        {
            get
            {
                return ServiceNode.Alliance;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt(this.Alliances.Length);
            foreach (Alliance Alliance in this.Alliances)
            {
                Alliance.Header.Encode(this.Data);
            }

            this.Data.AddInt(0);
        }
    }
}