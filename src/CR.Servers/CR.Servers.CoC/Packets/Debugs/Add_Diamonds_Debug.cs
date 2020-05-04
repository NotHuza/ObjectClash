using CR.Servers.CoC.Core;
using CR.Servers.Logic.Enums;
using CR.Servers.CoC.Logic;
using CR.Servers.CoC.Packets.Commands.Server;
using CR.Servers.CoC.Packets;

namespace CR.Servers.CoC.Packets.Debugs
{
 internal class Add_Diamonds_Debug : Debug
    {
        internal int Count;
        private string PlayerTag;


        public Add_Diamonds_Debug(params string[] Parameters) : base(Parameters)
        {
            // Add_Diamonds_Debug.
        }
        internal override Rank RequiredRank
        {
            get
            {
                return Rank.ServerOwner;
            }
        }

        internal override void Decode()
         {
         base.Decode();
         int.TryParse(this.NextParameter, out this.Count);
        }

        internal override async void Process()
        {
            if (this.Count > 0)
            {
                if (!string.IsNullOrEmpty(this.PlayerTag))
                {
                    Player Player = await Resources.Accounts.LoadPlayerAsync(this.PlayerHighID, this.PlayerLowID);

                    if (Player != null)
                    {
                        if (Player.Connected)
                        {
                           // Player.Level.GameMode.CommandManager.AddCommand(new Diamonds_Added(false, this.Count));
                        }
                        else
                        {
                            Player.AddDiamonds(this.Count);
                        }
                    }
                }
            }
        }
    }
}