namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Messages.Server.Home;
    using CR.Servers.Extensions.Binary;

    internal class Rename_Quick_Train : Command
    {
        internal string Name;

        internal int Slot;

        public Rename_Quick_Train(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 586;
            }
        }

        internal override void Decode()
        {
            this.Slot = this.Reader.ReadInt32();
            this.Name = this.Reader.ReadString();
            base.Decode();
        }

        internal override void Execute()
        {
            if (this.Name != null)
            {
                this.Name = Resources.Regex.Replace(this.Name, " ");

                if (this.Name.StartsWith(" "))
                {
                    this.Name = this.Name.Remove(0, 1);
                }

                if (this.Name.Length > 128)
                {
                    this.Name = this.Name.Substring(0, 128);
                }

                this.Device.GameMode.Level.ArmyNames[this.Slot] = this.Name;
            }
            else
            {
                new OutOfSyncMessage(this.Device).Send();
            }
        }
    }
}