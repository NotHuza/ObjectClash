namespace CR.Servers.CoC.Packets.Messages.Server.Battle
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;

    internal class LiveReplayDataMessage : Message
    {
        internal List<Command> Commands;

        internal int EndSubTick;

        internal int SpectatorTeam1;
        internal int SpectatorTeam2;

        public LiveReplayDataMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 24119;
            }
        }

        internal override void Encode()
        {
            this.Data.AddVInt(this.EndSubTick);
            this.Data.AddVInt(this.SpectatorTeam2);
            this.Data.AddVInt(this.SpectatorTeam1);

            if (this.Commands != null)
            {
                this.Data.AddInt(this.Commands.Count);

                for (int i = 0; i < this.Commands.Count; i++)
                {
                    this.Data.AddInt(this.Commands[i].Type);
                    this.Commands[i].Encode(this.Data);
                }
            }
            else
            {
                this.Data.AddInt(0);
            }
        }
    }
}