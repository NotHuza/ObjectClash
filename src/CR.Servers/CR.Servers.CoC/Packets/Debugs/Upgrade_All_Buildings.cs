using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR.Servers.CoC.Packets.Debugs
{
    internal class Upgrade_All_Buildings : Debug
    {
      
        public Upgrade_All_Buildings(params string[] Parameters) : base(Parameters)
        {
            // Upgrade_All_Buildings_Debug.
        }

      
     /*   internal override async void Process()
        {
            if (!string.IsNullOrEmpty(this.PlayerTag))
            {
                Player Player = await Resources.Accounts.LoadPlayerAsync(this.PlayerHighID, this.PlayerLowID);

                if (Player != null)
                {
                    while (Player.Level.GameObjectManager.TownHall.UpgradeAvailable)
                    {
                        Player.Level.GameObjectManager.TownHall.StartUpgrade();
                        Player.Level.GameObjectManager.TownHall.FinishConstruction();
                    }

                    foreach (Building Building in Player.Level.GameObjectManager.GameObjects[0][0])
                    {
                        while (Building.UpgradeAvailable)
                        {
                            Building.StartUpgrade();
                            Building.FinishConstruction();
                        }
                    }

                    if (Player.Connected)
                    {
                        new Disconnected_Message(Player.Level.GameMode.Device, 0).Send();
                    }
                }*/
            }
        }
    //}
//}