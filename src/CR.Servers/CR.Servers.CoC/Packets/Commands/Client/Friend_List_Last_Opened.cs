namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Extensions.Binary;

    internal class Friend_List_Last_Opened : Command
    {
        public Friend_List_Last_Opened(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 579;
            }
        }

        internal override void Execute()
        {
            this.Device.GameMode.Level.Player.Variables.Set(Variable.FriendListLastOpened, TimeUtils.UnixUtcNow);
        }
    }
}