using CR.Servers.CoC.Core.Network;
using CR.Servers.CoC.Logic;
using CR.Servers.CoC.Packets.Messages.Server.Battle;
using CR.Servers.Extensions.Binary;

namespace CR.Servers.CoC.Packets.Messages.Client.Battle
{
    internal class Search_Battle : Message
    {
        internal override short Type => 14123;

        public Search_Battle(Device Device, Reader Reader) : base(Device, Reader)
        {
            
        }

        internal override void Process()
        {
            new Battle_Fail(Device).Send(); //No idea how to reply yet
        }
    }
}
