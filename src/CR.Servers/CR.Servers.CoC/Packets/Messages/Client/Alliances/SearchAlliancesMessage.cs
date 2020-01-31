namespace CR.Servers.CoC.Packets.Messages.Client.Alliances
{
    using System.Linq;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Messages.Server.Alliances;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Logic.Enums;
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;

    internal class SearchAlliancesMessage : Message
    {
        internal int ClanLocation;
        internal int ExpLevels;
        internal int MaximumMembers;
        internal int MinimumMembers;
        internal bool OnlyCanJoin;
        internal int RequiredDuelScore;
        internal int RequiredScore;

        internal string TextSearch;
        internal int Unknown1;
        internal int WarFrequency;

        public SearchAlliancesMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 14324;
            }
        }

        internal override void Decode()
        {
            this.TextSearch = this.Reader.ReadString();
            this.WarFrequency = this.Reader.ReadInt32();
            this.ClanLocation = this.Reader.ReadInt32();
            this.MinimumMembers = this.Reader.ReadInt32();
            this.MaximumMembers = this.Reader.ReadInt32();
            this.RequiredScore = this.Reader.ReadInt32();
            this.RequiredDuelScore = this.Reader.ReadInt32();
            this.OnlyCanJoin = this.Reader.ReadBoolean();
            this.Unknown1 = this.Reader.ReadInt32();
            this.ExpLevels = this.Reader.ReadInt32();
        }

        internal override void Process()
        {
            new AllianceListMessage(this.Device)
            {
                TextSearch = this.TextSearch,
                Alliances = Resources.Clans.GetAllClans().Where(T =>
                        LikeOperator.LikeString(T.Header.Name, this.TextSearch, CompareMethod.Text) &&
                        T.Header.NumberOfMembers >= this.MinimumMembers &&
                        T.Header.NumberOfMembers <= this.MaximumMembers &&
                        T.Header.ExpLevel >= this.ExpLevels &&
                        T.Header.Type <= (this.OnlyCanJoin ? Hiring.INVITE : Hiring.CLOSED) && //TODO: Check required trophy stuff
                        T.Header.Origin == (this.ClanLocation == 0 ? T.Header.Origin : this.ClanLocation) &&
                        T.Header.WarFrequency == (this.WarFrequency == 0 ? T.Header.WarFrequency : this.WarFrequency) &&
                        T.Header.Score >= this.RequiredScore &&
                        T.Header.DuelScore >= this.RequiredDuelScore)
                    .Take(64).OrderByDescending(T => T.Header.Score).ToArray()
            }.Send();
        }
    }
}