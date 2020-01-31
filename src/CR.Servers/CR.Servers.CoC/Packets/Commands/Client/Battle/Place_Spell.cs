namespace CR.Servers.CoC.Packets.Commands.Client.Battle
{
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;
    using Newtonsoft.Json.Linq;

    internal class Place_Spell : Command
    {
        internal bool IsAllianceSpell;
        internal int Level;
        internal SpellData Spell;

        internal int X;
        internal int Y;

        public Place_Spell(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 704;
            }
        }

        internal override void Decode()
        {
            this.X = this.Reader.ReadInt32();
            this.Y = this.Reader.ReadInt32();
            this.Spell = this.Reader.ReadData<SpellData>();
            this.IsAllianceSpell = this.Reader.ReadBoolean();
            this.Level = this.Reader.ReadInt32();
            base.Decode();
        }

        internal override void Execute()
        {
            if (this.Spell != null)
            {
                Level Level = this.Device.GameMode.Level;

                if (this.IsAllianceSpell)
                {
                    Item Unit = Level.Player.AllianceSpells.GetByData(this.Spell, this.Level);

                    if (Unit != null)
                    {
                        if (Unit.Count > 0)
                        {
                            //Do some logging shit for replay and etc

                            Unit.Count--;
                            Level.Player.CastleUsedSpellCapacity -= this.Spell.HousingSpace;
                        }
                    }
                }
                else
                {
                    Item Unit = Level.Player.Spells.GetByData(this.Spell);

                    if (Unit != null)
                    {
                        if (Unit.Count > 0)
                        {
                            //Do some logging shit for replay and etc

                            Unit.Count--;
                        }
                    }
                }
            }
        }

        internal override JObject Save()
        {
            JObject Json = new JObject
            {
                {"base", this.SaveBase()},
                {"x", this.X},
                {"y", this.Y},
                {"d", this.Spell.GlobalId}
            };

            return Json;
        }
    }
}