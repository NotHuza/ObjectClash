namespace CR.Servers.CoC.Logic.Manager
{
    using System.Collections.Generic;
    using System.Threading;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Packets;
    using CR.Servers.CoC.Packets.Messages.Server.Home;

    internal class CommandManager
    {
        internal bool ChangeNameOnGoing;

        internal Level Level;
        internal int NextServerCommandId;
        internal Dictionary<int, ServerCommand> ServerCommands;

        public CommandManager()
        {
            this.ServerCommands = new Dictionary<int, ServerCommand>();
        }

        internal void SetLevel(Level Level)
        {
            this.Level = Level;
        }

        internal void AddCommand(ServerCommand Command)
        {
            if (Command.IsServerCommand)
            {
                if (this.Level.GameMode.Connected)
                {
                    this.ServerCommands.Add(Command.Id = Interlocked.Increment(ref this.NextServerCommandId), Command);
                    new AvailableServerCommandMessage(this.Level.GameMode.Device) {Command = Command}.Send();
                }
                else
                {
                    Command.Execute();
                }
            }
            else
            {
                Logging.Info(this.GetType(), "AddCommand() - Command is not a server command.");
            }
        }
    }
}