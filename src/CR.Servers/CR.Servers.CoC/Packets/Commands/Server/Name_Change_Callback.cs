namespace CR.Servers.CoC.Packets.Commands.Server
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Extensions.List;

    internal class Name_Change_Callback : ServerCommand
    {
        internal string AvatarName;
        internal int ChangeNameCount;

        public Name_Change_Callback(Device Client) : base(Client)
        {
        }

        public Name_Change_Callback(Device Client, Reader Reader) : base(Client, Reader)
        {
        }


        internal override int Type
        {
            get
            {
                return 3;
            }
        }

        internal override void Decode()
        {
            this.AvatarName = this.Reader.ReadString();
            this.ChangeNameCount = this.Reader.ReadInt32();
            base.Decode();
        }

        internal override void Encode(List<byte> Data)
        {
            Data.AddString(this.AvatarName);
            Data.AddInt(this.ChangeNameCount);
            base.Encode(Data);
        }

        internal override void Execute()
        {
            Level Level = this.Device.GameMode.Level;
            if (this.AvatarName != null)
            {
                Level.Player.Name = this.AvatarName;
                Level.Player.ChangeNameCount = this.ChangeNameCount;

                if (Level.Player.NameSetByUser)
                {
                    Level.Player.ChangeNameCount++;
                }
                else
                {
                    Level.Player.NameSetByUser = true;
                }
            }
        }
    }
}