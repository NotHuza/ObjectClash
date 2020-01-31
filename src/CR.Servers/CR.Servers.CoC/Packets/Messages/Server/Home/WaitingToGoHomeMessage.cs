namespace CR.Servers.CoC.Packets.Messages.Server.Home
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;

    internal class WaitingToGoHomeMessage : Message
    {
        internal int EstimatedTime;

        internal WaitingToGoHomeMessage(Device device, int estimatedTime) : base(device)
        {
            this.EstimatedTime = estimatedTime;
        }

        internal override short Type
        {
            get
            {
                return 24112;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt(this.EstimatedTime);
        }
    }
}