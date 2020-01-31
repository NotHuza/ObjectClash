namespace CR.Servers.CoC.Logic
{
    using System.Linq;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic.Enums;

    internal class NpcMapSlots : DataSlots
    {
        internal NpcMapSlots(int Capacity = 50) : base(Capacity)
        {
        }

        internal NpcMapSlots()
        {
        }

        internal bool CanAttackNPC(Data Data)
        {
            NpcData Npc = (NpcData) Data;

            /*
            for (int i = 0; i < Npc.MapDependencies.Length; i++)
            {
                Item Progress = this.GetByData(CSV.Tables.Get(Gamefile.Npcs).GetData(Npc.MapDependencies[i]));

                if (Progress == null || Progress.Count <= 0)
                {
                    return false;
                }
            }

            return true;*/

            return Npc.MapDependencies.Select(t => this.GetByData(CSV.Tables.Get(Gamefile.Npcs).GetData(t))).All(Progress => Progress != null && Progress.Count > 0);
        }
    }
}