namespace CR.Servers.CoC.Logic
{
    using CR.Servers.CoC.Logic.Enums;

    internal class VariableSlots : DataSlots
    {
        internal VariableSlots(int Capacity = 30) : base(Capacity)
        {
            // ResourceSlots.
        }

        internal int Village2BarrackLevel
        {
            get
            {
                return this.GetCountByGlobalId((int) Variable.Village2BarrackLevel);
            }
            set
            {
                this.Set(Variable.Village2BarrackLevel, value);
            }
        }


        internal void Initialize()
        {
            this.Set(Variable.AccountBound, 0);
            this.Set(Variable.BeenInArrangedWar, 0);
            this.Set(Variable.ChallengeLayoutIsWar, 0);
            this.Set(Variable.ChallengeStarted, 0);
            this.Set(Variable.EventUseTroop, 0);
            this.Set(Variable.FILL_ME, 0);
            this.Set(Variable.FriendListLastOpened, 0);
            this.Set(Variable.LootLimitCooldown, 0);
            this.Set(Variable.LootLimitFreeSpeedUp, 0);
            this.Set(Variable.LootLimitTimerEndSubTick, 0);
            this.Set(Variable.LootLimitTimerEndTimestamp, 0);
            this.Set(Variable.LootLimitWinCount, 0);
            this.Set(Variable.SeenBuilderMenu, 0);
            this.Set(Variable.StarBonusCooldown, 0);
            this.Set(Variable.StarBonusCounter, 0);
            this.Set(Variable.StarBonusTimerEndSubTick, 0);
            this.Set(Variable.StarBonusTimerEndTimestep, 0);
            this.Set(Variable.StarBonusTimesCollected, 0);
            this.Set(Variable.VillageToGoTo, 0);
            this.Set(Variable.Village2BarrackLevel, 0);
        }

        internal void Set(Variable Resource, int Count)
        {
            this.Set((int) Resource, Count);
        }
    }
}