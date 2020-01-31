namespace CR.Servers.CoC.Packets.Debugs
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Logic.Enums;

    internal class Print_Account_ID : Debug
    {
        public Print_Account_ID(Device Device, params string[] Parameters) : base(Device, Parameters)
        {
            // Print_Account_ID
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

            this.SendChatMessage("Your User ID: " + Player.UserId.ToString());
        }
    }
}