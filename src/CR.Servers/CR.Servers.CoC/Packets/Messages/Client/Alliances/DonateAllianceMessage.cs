namespace CR.Servers.CoC.Packets.Messages.Client.Alliances
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.CoC.Packets.Commands.Server;
    using CR.Servers.Extensions.Binary;

    internal class DonateAllianceMessage : Message
    {
        internal long StreamId;
        internal Data Unit;

        internal int UnitType;
        internal bool UseDiamonds;

        public DonateAllianceMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 14310;
            }
        }

        internal override void Decode()
        {
            this.UnitType = this.Reader.ReadInt32();
            this.Unit = this.Reader.ReadData();
            this.StreamId = this.Reader.ReadInt64();
            this.UseDiamonds = this.Reader.ReadBoolean();
        }

        internal override void Process()
        {
            Player Player = this.Device.GameMode.Level.Player;

            if (Player.InAlliance)
            {
                Alliance Alliance = Player.Alliance;
                StreamEntry Stream = Alliance.Streams.Get(this.StreamId);

                if (Stream != null)
                {
                    if (Stream is DonateStreamEntry)
                    {
                        DonateStreamEntry DonationStream = (DonateStreamEntry)Stream;
                        Player Target = Resources.Accounts.LoadAccount(Stream.SenderHighId, Stream.SenderLowId)?.Player;
                        if (Target != null)
                        {
                            if (Target.InAlliance)
                            {
                                if (Target.AllianceId == Player.AllianceId)
                                {
                                    if (this.UnitType == 1)
                                    {
                                        if (this.Unit is SpellData)
                                        {
                                            SpellData SpellData = (SpellData)this.Unit;
                                            if (DonationStream.MaxSpell >= DonationStream.UsedSpell + SpellData.HousingSpace)
                                            {
                                                if (this.UseDiamonds)
                                                {
                                                    int Cost = SpellData.DonateCost;
                                                    if (Player.HasEnoughDiamonds(Cost))
                                                    {
                                                        Player.UseDiamonds(Cost);
                                                    }
                                                    else
                                                    {
                                                        Logging.Error(this.GetType(), "Unable to donate unit. The player use diamonds to donate but doesn't have enough diamonds!");
                                                        return;
                                                    }
                                                }

                                                int UnitLevel = Player.GetUnitUpgradeLevel(SpellData);
                                                UnitItem Unit = new UnitItem(SpellData.GlobalId, 1, UnitLevel);

                                                DonationStream.New = false;

                                                Target.AllianceUnits.Add(Unit);
                                                Unit.DonatorId = Player.UserId;
                                                DonationStream.Units.Add(Unit, Player.UserId);

                                                int HousingSpace = SpellData.HousingSpace;
                                                DonationStream.UsedSpell += HousingSpace;
                                                Target.CastleUsedSpellCapacity += HousingSpace;

                                                Player.Donation += HousingSpace;
                                                Target.DonationReceived += HousingSpace;
                                                Player.AddExperience(HousingSpace);

                                                this.Device.GameMode.CommandManager.AddCommand(new Donate_Unit_Callback(this.Device) { StreamId = this.StreamId, UnitType = this.UnitType, UnitId = SpellData.GlobalId, UseDiamonds = this.UseDiamonds });

                                                if (Target.Connected)
                                                {
                                                    Target.Level.GameMode.CommandManager.AddCommand(new Alliance_Unit_Received(Target.Level.GameMode.Device) { Donator = Player.Name, UnitType = this.UnitType, UnitId = SpellData.GlobalId, Level = UnitLevel });
                                                }

                                                Alliance.Streams.Update(Stream);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (this.Unit is CharacterData)
                                        {
                                            CharacterData CharacterData = (CharacterData)this.Unit;
                                            if (DonationStream.MaxTroop >= DonationStream.UsedTroop + CharacterData.HousingSpace)
                                            {
                                                if (this.UseDiamonds)
                                                {
                                                    int Cost = CharacterData.DonateCost;
                                                    if (Player.HasEnoughDiamonds(Cost))
                                                    {
                                                        Player.UseDiamonds(Cost);
                                                    }
                                                    else
                                                    {
                                                        Logging.Error(this.GetType(), "Unable to donate unit. The player use diamonds to donate but doesn't have enough diamonds!");
                                                        return;
                                                    }
                                                }

                                                int UnitLevel = Player.GetUnitUpgradeLevel(CharacterData);
                                                UnitItem Unit = new UnitItem(CharacterData.GlobalId, 1, UnitLevel);

                                                DonationStream.New = false;

                                                Target.AllianceUnits.Add(Unit);
                                                Unit.DonatorId = Player.UserId;
                                                DonationStream.Units.Add(Unit, Player.UserId);

                                                int HousingSpace = CharacterData.HousingSpace;
                                                DonationStream.UsedTroop += HousingSpace;
                                                Target.CastleUsedCapacity += HousingSpace;

                                                Player.Donation += HousingSpace;
                                                Target.DonationReceived += HousingSpace;
                                                Player.AddExperience(HousingSpace);

                                                this.Device.GameMode.CommandManager.AddCommand(new Donate_Unit_Callback(this.Device) { StreamId = this.StreamId, UnitType = this.UnitType, UnitId = CharacterData.GlobalId, UseDiamonds = this.UseDiamonds });

                                                if (Target.Connected)
                                                {
                                                    Target.Level.GameMode.CommandManager.AddCommand(new Alliance_Unit_Received(Target.Level.GameMode.Device) { Donator = Player.Name, UnitType = this.UnitType, UnitId = CharacterData.GlobalId, Level = UnitLevel });
                                                }

                                                Alliance.Streams.Update(Stream);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}