namespace CR.Servers.CoC.Packets.Messages.Client.Home
{
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Messages.Server.Home;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Logic.Enums;
    using System;
    using System.Threading.Tasks;

    internal class GoHomeMessage : Message
    {
        internal int Mode;
        static Player Player;

        public GoHomeMessage(Device device, Reader reader) : base(device, reader)
        {
            // Space
        }

        internal override short Type
        {
            get
            {
                return 14101;
            }
        }

        internal override void Decode()
        {
            this.Mode = this.Reader.ReadInt32();
        }

        internal override async Task ProcessAsync()
        {
            if (this.Mode == 1)
            {
                this.Device.State = State.WAR_EMODE;
            }
            else
            {
                Player = this.Device.GameMode.Level.Player;
                if (this.Device.State == State.IN_PC_BATTLE)
                {
                    if (this.Device.Account.Battle != null)
                    {
                        // Player.Score = this.Device.Account.Battle.LostScore;
                        await this.Device.Account.Battle.Trofeislogichlost();
                        await this.Device.Account.Battle.EndBattleAsync();
                        this.Device.Account.Battle = null;//npc
                    }
                    else
                    {
                        await this.Device.Account.Battle.Trofeislogichwin();
                        await this.Device.Account.Battle.EndBattleAsync();
                        //Player.Score = this.Device.Account.Battle.WinScore;
                        this.Device.Account.Battle = null;//npc
          
                        
                    }
                }

                if (this.Device.Account.DuelBattle != null)
                {
                   
                    await this.Device.Account.DuelBattle.GetBattle(this.Device.GameMode.Level)?.EndBattleAsync();
                   // Player.DuelScore = this.Device.Account.Battle.WinDuelScore;
                    this.Device.Account.DuelBattle.SendDuelBattleInfoMessage();
                    this.Device.Account.DuelBattle = null;//npc
                }

                this.Device.State = State.LOGGED;
            }

            new OwnHomeDataMessage(this.Device).Send();
        }
    }
}