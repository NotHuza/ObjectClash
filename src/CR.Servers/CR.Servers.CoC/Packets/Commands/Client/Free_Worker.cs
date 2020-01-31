namespace CR.Servers.CoC.Packets.Commands.Client
{
    using System;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Free_Worker : Command
    {
        internal Command Command;
        internal bool EmbedCommand;
        internal int VillageWorker;

        public Free_Worker(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 521;
            }
        }

        internal override void Decode()
        {
            base.Decode();
            this.VillageWorker = this.Reader.ReadInt32();
            this.EmbedCommand = this.Reader.ReadBoolean();

            if (this.EmbedCommand)
            {
                int CommandID = this.Reader.ReadInt32();

                if (Factory.Commands.ContainsKey(CommandID))
                {
                    this.Command = Factory.CreateCommand(CommandID, this.Device, this.Reader);

                    if (this.Command != null)
                    {
                        this.Command.Decode();
                    }
                }
                else
                {
                    Logging.Error(this.GetType(), "Embded Command is not handled! (" + CommandID + ")");
                    this.Reader.BaseStream.Position = this.Reader.BaseStream.Position - (4 + 1 + 4 + 4);
                    this.Log();
                }
            }
        }

        internal override void Execute()
        {
            Level Level = this.Device.GameMode.Level;

            if (this.VillageWorker == 0)
            {
                if (Level.WorkerManager.FreeWorkers == 0)
                {
                    Level.WorkerManager.FinishTaskOfOneWorker();
                }
                else
                {
                    Logging.Error(this.GetType(), "Free worker called even when there is free worker!");
                }
            }
            else
            {
                if (Level.WorkerManagerV2.FreeWorkers == 0)
                {
                    Level.WorkerManagerV2.FinishTaskOfOneWorker();
                }
                else
                {
                    Logging.Error(this.GetType(), "Free worker called even when there is free worker!");
                }
            }

            if (this.Command != null)
            {
                try
                {
                    if (!this.Command.IsServerCommand)
                    {
                        this.Command.Execute();
                        /*
                        Logging.Info(this.GetType(), "Embedded Command is handled! (" + this.Command.Type + ")");
                        */
                    }
                    else
                    {
                        Logging.Error(this.GetType(), "Unable to execute server command as embedded command! (" + this.Command.Type + ")");
                    }
                }
                catch (Exception Exception)
                {
                    Logging.Error(this.GetType(),
                        "An error has been throwed when the process of command type " + this.Command.Type + ". " +
                        Exception);
                }
            }
        }
    }
}