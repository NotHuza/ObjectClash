namespace CR.Servers.CoC.Packets.Messages.Server.Battle
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;

    internal class LiveReplayMessage : Message
    {
        internal int ClientTick;

        internal List<Command> Commands;
        internal string ReplayJSON;

        public LiveReplayMessage(Device Device, string replayJSON, List<Command> commands, int clientTick) : base(Device)
        {
            this.ClientTick = clientTick;
            this.ReplayJSON = replayJSON;
            this.Commands = commands;
        }

        internal override short Type
        {
            get
            {
                return 24118;
            }
        }

        internal override void Encode()
        {
            this.Data.AddString(null);
            this.Data.AddCompressed(this.ReplayJSON, false);
            this.Data.AddInt(this.ClientTick);
            this.Data.AddInt(this.Commands.Count);

            for (int i = 0; i < this.Commands.Count; i++)
            {
                this.Data.AddInt(this.Commands[i].Type);
                this.Commands[i].Encode(this.Data);
            }

            this.Data.AddInt(1);
        }
    }
}