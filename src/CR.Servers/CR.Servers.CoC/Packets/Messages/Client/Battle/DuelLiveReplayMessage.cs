namespace CR.Servers.CoC.Packets.Messages.Client.Battle
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Battles;
    using CR.Servers.Extensions.Binary;

    internal class DuelLiveReplayMessage : Message
    {
        internal long LiveId;

        public DuelLiveReplayMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 15110;
            }
        }

        internal override void Decode()
        {
            this.LiveId = this.Reader.ReadInt64();
        }

        internal override void Process()
        {
            DuelBattle duel;
            if (Resources.Duels.DuelBattles.TryGetValue(this.LiveId, out duel))
            {
                Battle battle = duel.GetEnemyBattle(this.Device.GameMode.Level);

                if (battle != null)
                {
                    battle.AddViewer(this.Device);
                }
            }
        }
    }
}