namespace CR.Servers.CoC.Packets.Messages.Client.Battle
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class CancelDuelMatchmakeMessage : Message
    {
        public CancelDuelMatchmakeMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 14103;
            }
        }

        internal override void Process()
        {
            Resources.Duels.Quit(this.Device);
        }
    }
}