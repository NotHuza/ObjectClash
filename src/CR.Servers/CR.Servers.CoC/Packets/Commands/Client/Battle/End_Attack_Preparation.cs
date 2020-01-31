namespace CR.Servers.CoC.Packets.Commands.Client.Battle
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class End_Attack_Preparation : Command
    {
        public End_Attack_Preparation(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        public End_Attack_Preparation()
        {
        }

        internal override int Type
        {
            get
            {
                return 702;
            }
        }
    }
}
