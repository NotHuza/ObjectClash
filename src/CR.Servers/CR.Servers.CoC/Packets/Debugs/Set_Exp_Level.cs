namespace CR.Servers.CoC.Packets.Debugs
{
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Messages.Server.Home;
    using CR.Servers.Logic.Enums;

    internal class Set_Exp_Level : Debug
    {
        internal int Level;

        public Set_Exp_Level(Device Device, params string[] Parameters) : base(Device, Parameters)
        {
            // Set_Exp_Level
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
                if (int.TryParse(this.Parameters[0], out this.Level))
                {
                    if (this.Level > 0)
                    {
                        if (this.Level < 501)
                        {
                            Player.ExpLevel = this.Level;
                            new OwnHomeDataMessage(this.Device).Send();
                        }
                        else
                        {
                            this.SendChatMessage("Level maximum is 500!");
                            return;
                        }
                    }
                    else
                    {
                        this.SendChatMessage("Level minimum is 1!");
                        return;
                    }
                }
                else
                {
                    this.SendChatMessage("Invalid parameters! Usage: /setlevel <LEVEL>");
                }
            }
            else
            {
                this.SendChatMessage("Missing parameters! Usage: /setlevel <LEVEL>");
            }
        }
    }
}