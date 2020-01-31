namespace CR.Servers.CoC.Packets.Commands.Client
{
    using System.Collections.Generic;
    using System.Text;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Map;
    using CR.Servers.Extensions.Binary;

    internal class Move_Building : Command
    {
        internal int Id;

        internal int X;
        internal int Y;

        public Move_Building(Device Client, Reader Reader) : base(Client, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 501;
            }
        }

        internal override void Decode()
        {
            this.X = this.Reader.ReadInt32();
            this.Y = this.Reader.ReadInt32();
            this.Id = this.Reader.ReadInt32();

            base.Decode();
        }

        internal override void Execute()
        {
            GameObject GameObject = this.Device.GameMode.Level.GameObjectManager.Filter.GetGameObjectByPreciseId(this.Id);

            if (GameObject != null)
            {
                if (GameObject.Type != 3)
                {
                    GameObject.SetPositionXY(this.X, this.Y);
                }
            }
            else
            {
                StringBuilder Error = new StringBuilder();
                GameObject Precise = this.Device.GameMode.Level.GameObjectManager.Filter.GetGameObjectByPreciseId(this.Id);
                if (Precise != null)
                {
                    Error.AppendLine($"Building Id :  {this.Id}");
                    Error.AppendLine($"Building Name :  {Precise.Data.Name}");
                    Error.AppendLine($"Building New X :  {this.X}");
                    Error.AppendLine($"Building New Y :  {this.Y}");
                    Error.AppendLine($"Building still null with precise id : {false}");

                    Error.AppendLine($"Player Id :  {this.Device.GameMode.Level.Player.UserId}");
                    Error.AppendLine($"Player current village :  {this.Device.GameMode.Level.GameObjectManager.Map}");
                    Error.AppendLine($"Player town hall level :  {this.Device.GameMode.Level.Player.TownHallLevel}");
                    Error.AppendLine($"Player town hall2 level :  {this.Device.GameMode.Level.Player.TownHallLevel2}");
                }
                else
                {
                    Error.AppendLine($"Precise building : null");

                    Error.AppendLine($"Building Id :  {this.Id}");
                    Error.AppendLine($"Building New X :  {this.X}");
                    Error.AppendLine($"Building New Y :  {this.Y}");

                    Error.AppendLine($"Player Id :  {this.Device.GameMode.Level.Player.UserId}");
                    Error.AppendLine($"Player current village :  {this.Device.GameMode.Level.GameObjectManager.Map}");
                    Error.AppendLine($"Player town hall level :  {this.Device.GameMode.Level.Player.TownHallLevel}");
                    Error.AppendLine($"Player town hall2 level :  {this.Device.GameMode.Level.Player.TownHallLevel2}");
                }

                Resources.Logger.Debug(Error);
            }
        }
    }
}