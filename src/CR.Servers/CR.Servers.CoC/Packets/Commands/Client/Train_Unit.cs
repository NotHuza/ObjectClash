namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Train_Unit : Command
    {
        internal int BarrackId;
        internal int Count;

        internal Data Unit;
        internal int UnitType;

        public Train_Unit(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 508;
            }
        }

        internal override void Decode()
        {
            this.Reader.ReadInt32();

            this.UnitType = this.Reader.ReadInt32();
            this.Unit = this.Reader.ReadData();
            this.Count = this.Reader.ReadInt32();

            this.BarrackId = this.Reader.ReadInt32();
            base.Decode();
        }

        internal override void Execute()
        {
            Level Level = this.Device.GameMode.Level;
            if (this.Unit != null)
            {
                if (this.UnitType == 0)
                {
                    CharacterData Character = this.Unit as CharacterData;

                    ResourceData TrainingResource = Character?.TrainingResourceData;

                    if (Level.Player.Resources.GetCountByData(TrainingResource) >= Character?.TrainingCost[0] * this.Count)
                    {
                        //if (Level.UnitProductionManager.CanProduce(Character, this.Count))
                        {
                            Level.Player.Units.Add(Character, this.Count);
                            //Level.UnitProductionManager.AddUnit(Character, this.Count);
                            Level.Player.Resources.Remove(TrainingResource, Character.TrainingCost[0] * this.Count);
                        }
                    }
                }
                else
                {
                    SpellData Spell = this.Unit as SpellData;

                    ResourceData TrainingResource = Spell?.TrainingResourceData;

                    if (Level.Player.Resources.GetCountByData(TrainingResource) >= Spell?.TrainingCost[0] * this.Count)
                    {
                        //if (Level.SpellProductionManager.CanProduce(Spell, this.Count))
                        {
                            //Level.SpellProductionManager.AddUnit(Spell, this.Count);

                            Level.Player.Spells.Add(Spell, this.Count);
                            Level.Player.Resources.Remove(TrainingResource, Spell.TrainingCost[0] * this.Count);
                        }
                    }
                }
            }
        }
    }
}