namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Extensions.Binary;

    internal class Change_Village_Mode : Command
    {
        internal int State;

        public Change_Village_Mode(Device client, Reader reader) : base(client, reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 591;
            }
        }

        internal override void Decode()
        {
            this.State = this.Reader.ReadInt32();
            base.Decode();
        }

        internal override void Execute()
        {
            Level Level = this.Device.GameMode.Level;

            if (this.State == 0 || this.State == 1)
            {
                Level.Player.Variables.Set((int)Variable.VillageToGoTo, this.State);
            }
            else
            {
                Logging.Error(GetType(), "------------------ State => " + State);
            }
#if DEBUG
            Logging.Info(this.GetType(),
                $"Village Manager : Changing mode to {(Village_Mode)Level.Player.Variables.GetCountByGlobalId((int)Variable.VillageToGoTo)}");
#endif
        }
    }

    public enum Village_Mode
    {
        NORMAL_VILLAGE,
        BUILDER_VILLAGE
    }
}