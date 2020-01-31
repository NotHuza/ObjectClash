namespace CR.Servers.CoC.Packets.Debugs
{
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Messages.Server.Home;
    using CR.Servers.Logic.Enums;

    internal class Set_Score : Debug
    {
        internal int Score;

        public Set_Score(Device Device, params string[] Parameters) : base(Device, Parameters)
        {
            // Set_Score
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
            Player Player = this.Device.GameMode.Level.Player;

            if (this.Parameters.Length >= 1)
            {
                if (int.TryParse(this.Parameters[0], out this.Score))
                {
                    if (this.Score > -1)
                    {
                        if (this.Score < 10000)
                        {
                            Player.Score = this.Score;
                            Player.DuelScore = this.Score;
                            new OwnHomeDataMessage(this.Device).Send();
                        }
                        else
                        {
                            this.SendChatMessage("Score maximum is 9999!");
                            return;
                        }
                    }
                    else
                    {
                        this.SendChatMessage("Score count cannot be negative!");
                        return;
                    }
                }
                else
                {
                    this.SendChatMessage("Invalid parameters! Usage: /setscore <AMOUNT>");
                }
            }
            else
            {
                this.SendChatMessage("Missing parameters! Usage: /setscore <AMOUNT>");
            }
        }
    }
}