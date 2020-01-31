namespace CR.Servers.CoC.Packets.Debugs.Elite
{
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.CoC.Packets.Messages.Server.Home;
    using CR.Servers.Logic.Enums;

    internal class Add_Spells : Debug
    {
        public Add_Spells(Device Device, params string[] Parameters) : base(Device, Parameters)
        {
            // Add_Spells
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
            foreach (SpellData Data in CSV.Tables.Get(Gamefile.Spells).Datas)
            {
                if (!Data.DisableProduction)
                {
                    if (Data.VillageType == 0)
                    {
                        this.Device.GameMode.Level.Player.Spells.Add(Data, 500);
                    }
                }
            }

            if (this.Device.Connected)
            {
                new OwnHomeDataMessage(this.Device).Send();
            }
        }
    }
}