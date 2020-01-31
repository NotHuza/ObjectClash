namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Edit_Mode_Shown : Command
    {
        public Edit_Mode_Shown(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 544;
            }
        }

        internal override void Execute()
        {
            this.Device.GameMode.Level.EditModeShown = true;
        }
    }
}