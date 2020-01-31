namespace CR.Servers.CoC.Packets
{
    using System;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.CoC.Packets.Messages.Server.Home;
    using CR.Servers.Logic.Enums;

    internal class Debug : IDisposable
    {
        internal Alliance Alliance;
        internal Device Device;

        internal int ParameterOffset;

        internal string[] Parameters;


        internal Debug(Device Device, params string[] Parameters)
        {
            this.Device = Device;
            this.Parameters = Parameters;
        }

        internal string NextParameter
        {
            get
            {
                return this.ParameterOffset >= this.Parameters.Length ? this.Parameters[this.ParameterOffset++] : null;
            }
        }

        internal virtual Rank RequiredRank
        {
            get
            {
                return Rank.Player;
            }
        }

        public virtual void Dispose()
        {
            this.Parameters = null;
        }

        internal virtual void Process()
        {
            // Process.
        }

        internal void SendChatMessage(string message)
        {
            new GlobalChatLineMessage(this.Device, this.Device.GameMode.Level.Player)
            {
                Message = message,
                Name = "[System] Command Manager",
                ExpLevel = 300,
                League = 22,
                Bot = true
            }.Send();
        }
    }
}