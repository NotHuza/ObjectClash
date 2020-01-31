namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Messages.Server.Home;
    using CR.Servers.Extensions.Binary;

    internal class Claim_Achievement_Reward : Command
    {
        internal int AchievementId;

        public Claim_Achievement_Reward(Device Device, Reader reader) : base(Device, reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 523;
            }
        }

        internal override void Decode()
        {
            this.AchievementId = this.Reader.ReadInt32();
            base.Decode();
        }

        internal override void Execute()
        {
            Level Level = this.Device.GameMode.Level;
            if (!Level.Player.AchievementProgress.Claim(this.AchievementId))
            {
                new OutOfSyncMessage(this.Device).Send();
            }
        }
    }
}