//#define Extra

using CR.Servers.CoC.Core.Network;
using CR.Servers.CoC.Packets.Messages.Server.Account;
using CR.Servers.CoC.Packets.Messages.Server.Home;

namespace CR.Servers.CoC.Packets.Messages.Client.Home
{
    using System;
    using System.Collections.Generic;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;
    using System.Threading.Tasks;

    internal class EndClientTurnMessage : Message
    {
        internal int Checksum;
        internal int CommandCount;

        internal List<Command> Commands;
        internal int SubTick;

        public EndClientTurnMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
            // End_Client_Turn.
        }

        internal override short Type
        {
            get
            {
                return 14102;
            }
        }

        internal override void Decode()
        {
            this.SubTick = this.Reader.ReadInt32();
            this.Checksum = this.Reader.ReadInt32();
            this.CommandCount = this.Reader.ReadInt32();

            if (this.CommandCount <= 1028)
            {
                if (this.CommandCount > 0)
                {
                    this.Commands = new List<Command>(this.CommandCount);

                    for (int i = 0; i < this.CommandCount; i++)
                    {
                        int CommandID = this.Reader.ReadInt32();
                        Command Command = Factory.CreateCommand(CommandID, this.Device, this.Reader);

                        if (Command != null)
                        {
                            Command.Decode();

                            this.Commands.Add(Command);
                        }
                        else
                        {
                            Logging.Info(this.GetType(), "Command is null! (" + CommandID + ")");
                            this.CommandCount = this.Commands.Count;
                            break;
                        }
                    }
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Command count is too high! (" + this.CommandCount + ")");
            }
        }

        internal override async Task ProcessAsync()
        {
            this.Device.GameMode.Time.SubTick = this.SubTick;
            this.Device.GameMode.Level.Tick();

#if Extra
            //Logging.Info(this.GetType(), "Client Time : MS:" + this.Device.GameMode.Time.TotalMS + "  SECS:" + this.Device.GameMode.Time.TotalSecs + ".");
            //Logging.Info(this.GetType(), "Client Subtick : " + this.SubTick + ".");
#endif
            if (this.CommandCount > 0)
            {
                for (int i = 0; i < this.CommandCount; i++)
                {
                    Command Command = this.Commands[i];

                    if (Command.IsServerCommand)
                    {
                        ServerCommand ServerCommand = (ServerCommand)Command;
                        ServerCommand OriginalCommand;
                        if (this.Device.GameMode.CommandManager.ServerCommands.TryGetValue(ServerCommand.Id, out OriginalCommand))
                        {
                            this.Device.GameMode.CommandManager.ServerCommands.Remove(ServerCommand.Id);
                        }
                        else
                        {
                            this.Reader.BaseStream.Position = 0;
                            this.Log();
                            Logging.Error(this.GetType(), this.Device, "Execute command failed! Server Command " + Command.Type + " is not available.");
                        }
                    }

                    if (Command.ExecuteSubTick <= this.SubTick)
                    {
                        try
                        { Command.Execute(); }
                        catch (Exception Exception)
                        {
                            Logging.Error(Exception.GetType(),
                                $"Exception while executing a command {Command.Type}. " + Exception.Message +
                                Environment.NewLine + Exception.StackTrace);
                        }

                        try
                        { await Command.ExecuteAsync(); }
                        catch (Exception Exception)
                        {
                            Logging.Error(Exception.GetType(),
                                $"Exception while executing a command {Command.Type} async. " + Exception.Message +
                                Environment.NewLine + Exception.StackTrace);
                        }

#if COMMAND_DEBUG
                        Device.Account.Player.Debug.Record(Command);
#endif
                    }
                    else
                    {
                        Logging.Error(this.GetType(), this.Device, "Execute command failed! Command should have been executed already. (type=" + Command.Type + ", command_tick=" + Command.ExecuteSubTick + ", server_tick=" + this.SubTick + ")");
                    }
                }
            }

            if (this.Device.Account.Battle != null)
            {
                if (!this.Device.Account.Battle.Ended)
                {
                    if (this.Device.Account.Home == this.Device.Account.Battle.Attacker.Home)
                    {
                        this.Device.Account.Battle.HandleCommands(this.SubTick, this.Commands);
                    }
                }
                else
                {
                    this.Device.Account.Battle = null;
                }
            }
        }
    }
}