namespace CR.Servers.CoC.Packets.Commands.Client
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Commands.Client.List;
    using CR.Servers.Extensions.Binary;

    internal class Remove_Units : Command
    {
        internal List<UnitToRemove> UnitsToRemove;

        public Remove_Units(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 550;
            }
        }

        internal override void Decode()
        {
            int UnitCount = this.Reader.ReadInt32();
            this.UnitsToRemove = new List<UnitToRemove>(UnitCount);

            for (int i = 0; i < UnitCount; i++)
            {
                this.Reader.ReadInt32();
                this.UnitsToRemove.Add(new UnitToRemove
                {
                    Type = this.Reader.ReadInt32(),
                    Id = this.Reader.ReadInt32(),
                    Count = this.Reader.ReadInt32()
                });
                this.Reader.ReadInt32();
            }

            base.Decode();
        }

        internal override void Execute()
        {
            Level Level = this.Device.GameMode.Level;
            foreach (UnitToRemove UnitToRemove in this.UnitsToRemove)
            {
                if (UnitToRemove.Type == 0)
                {
                    Item Unit = Level.Player.Units.Find(T => T.Data == UnitToRemove.Id);
                    if (Unit != null)
                    {
                        Unit.Count -= UnitToRemove.Count;
                    }
                }
                else if (UnitToRemove.Type == 1)
                {
                    Item Spell = Level.Player.Spells.Find(T => T.Data == UnitToRemove.Id);
                    if (Spell != null)
                    {
                        Spell.Count -= UnitToRemove.Count;
                    }
                }
                else
                {
                    Logging.Error(this.GetType(), "Unable to remove units. Unknown unit type!");
                }
            }
        }
    }
}