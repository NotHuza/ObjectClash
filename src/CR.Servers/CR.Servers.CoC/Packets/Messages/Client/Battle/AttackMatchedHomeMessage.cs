namespace CR.Servers.CoC.Packets.Messages.Client.Battle
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Battles;
    using CR.Servers.CoC.Packets.Commands.Client.Battle;
    using CR.Servers.CoC.Packets.Messages.Server.Battle;
    using CR.Servers.CoC.Packets.Messages.Server.Home;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Logic.Enums;

    internal class AttackMatchedHomeMessage : Message
    {
        public AttackMatchedHomeMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 14123;
            }
        }

        internal override void Process()
        {
            // SHIT METHOD

            new OwnHomeDataMessage(this.Device).Send();
            new AvailableServerCommandMessage(this.Device)
            {
                Command = new Search_Opponent(this.Device, null)
            }.Send();
        }
    }
}