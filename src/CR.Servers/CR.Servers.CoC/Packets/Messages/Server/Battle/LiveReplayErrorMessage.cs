namespace CR.Servers.CoC.Packets.Messages.Server.Battle
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Enums;
    using CR.Servers.Extensions.List;

    internal class LiveReplayErrorMessage : Message
    {
        internal LiveReplayErrorReason Reason;

        public LiveReplayErrorMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 24117;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt((int) this.Reason);
        }
    }
}