namespace CR.Servers.CoC.Logic
{
    using CR.Servers.CoC.Logic.Battles;

    internal class Account
    {
        internal Battle Battle;
        internal Device Device;
        internal DuelBattle DuelBattle;
        internal Home Home;
        internal Player Player;

        internal int HighId;
        internal int LowId;

        public Account(int HighID, int LowID, Player Player, Home Home)
        {
            this.HighId = HighID;
            this.LowId = LowID;
            this.Player = Player;
            this.Home = Home;
        }

        internal long AccountId
        {
            get
            {
                return ((long) this.HighId << 32) | (uint) this.LowId;
            }
        }
    }
}