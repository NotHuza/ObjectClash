namespace CR.Servers.CoC.Packets.Debugs.Elite
{
    using System;
    using System.Text;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Messages.Server.Home;
    using CR.Servers.Logic.Enums;

    internal class Fast_Forward : Debug
    {
        internal StringBuilder Help;

        public Fast_Forward(Device Device, params string[] Parameters) : base(Device, Parameters)
        {
        }

        internal override Rank RequiredRank
        {
            get
            {
                return Rank.Elite;
            }
        }

        internal override void Process()
        {
            if (this.Parameters.Length >= 1)
            {
                int Time;
                if (int.TryParse(this.Parameters[0], out Time))
                {
                    if (Time > 0)
                    {
                        this.Device.GameMode.Level.FastForwardTime(Time);

                        if (this.Device.Connected)
                        {
                            new OwnHomeDataMessage(this.Device).Send();
                        }


                        this.SendChatMessage("Command successfuly processed! Total elapsed time " + TimeSpan.FromSeconds(Time));
                    }
                }
                else
                {
                    this.Help = new StringBuilder();
                    this.Help.AppendLine("Time exceed the limit!");
                    this.SendChatMessage(this.Help.ToString());
                }

                //Time bust be at least 1
            }
        }
    }
}