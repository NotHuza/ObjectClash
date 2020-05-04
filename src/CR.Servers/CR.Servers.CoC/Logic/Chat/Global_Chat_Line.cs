using CR.Servers.CoC.Packets.Messages.Server.Leaderboard;
using System;

namespace CR.Servers.CoC.Logic.Chat
{
    internal class Global_Chat_Line//some code for global chat work progress
    {
        private Device _device;
        private Player _player;

        public Global_Chat_Line(Device device, Player player)
        {
            this._device = device;
            this._player = player;
        }
 
        public string Message { get; set; }
        public string Name { get; set; }


        public Device Device => _device;
        public Player Player => _player;


        internal void Send()
        {
            //if (Device.Connected)
            //Device.EnqueueOutgoingMessage(this);
            //if (Player.Connected);
            new Global_Clans(Device, Device.GameMode.Level.Player) { Message = Message, Name = Player.Name }.Send();
        }
    }
}