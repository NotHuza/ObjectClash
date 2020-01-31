namespace CR.Servers.CoC.Packets.Debugs.Elite
{
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.CoC.Packets.Messages.Server.Home;
    using CR.Servers.Logic.Enums;

    internal class Clear_All_Units : Debug
    {
        public Clear_All_Units(Device Device, params string[] Parameters) : base(Device, Parameters)
        {
            // Clear_All_Units
        }

        internal override Rank RequiredRank
        {
            get
            {
                return Rank.Player;
            }
        }

        internal override void Process()
        {
            foreach (CharacterData Data in CSV.Tables.Get(Gamefile.Characters).Datas)
            {
                if (Data.VillageType == 0)
                {
                    this.Device.GameMode.Level.Player.Units.Clear();
                }
            }

            if (this.Device.Connected)
            {
                new OwnHomeDataMessage(this.Device).Send();
            }
        }
    }
}