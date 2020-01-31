namespace CR.Servers.CoC.Logic
{
    using System.Collections.Generic;
    using CR.Servers.Extensions.List;

    internal class AchievementSlot : List<int>
    {
        internal Player Player;

        internal new bool Add(int GlobalId)
        {
            if (!this.Contains(GlobalId))
            {
                base.Add(GlobalId);
                return true;
            }

            return false;
        }

        internal void Encode(List<byte> Packet)
        {
            Packet.AddInt(this.Count);

            for (int i = 0; i < this.Count; i++)
            {
                Packet.AddInt(this[i]);
            }
        }
    }
}