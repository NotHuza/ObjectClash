namespace CR.Servers.CoC.Packets.Debugs
{
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.CoC.Packets.Messages.Server.Home;
    using CR.Servers.Logic.Enums;

    internal class Max_Resources : Debug
    {
        public Max_Resources(Device Device, params string[] Parameters) : base(Device, Parameters)
        {
            // Max_Resources
        }

        internal override Rank RequiredRank
        {
            get
            {
                return Rank.Player;
            }
        }

        internal override void Process()
        {
            Player Player = this.Device.GameMode.Level.Player;

            Player.Resources.Set(Resource.Diamonds, Extension.ParseConfigInt("Game:StartingResources:Diamonds"));
            Player.Resources.Set(Resource.Gold, Extension.ParseConfigInt("Game:StartingResources:Gold"));
            Player.Resources.Set(Resource.Elixir, Extension.ParseConfigInt("Game:StartingResources:Elixir"));
            Player.Resources.Set(Resource.DarkElixir, Extension.ParseConfigInt("Game:StartingResources:DarkElixir"));
            Player.Resources.Set(Resource.Builder_Gold, Extension.ParseConfigInt("Game:StartingResources:GoldV2"));
            Player.Resources.Set(Resource.Builder_Elixir, Extension.ParseConfigInt("Game:StartingResources:ElixirV2"));

            if (this.Device.Connected)
            {
                new OwnHomeDataMessage(this.Device).Send();
            }
        }
    }
}