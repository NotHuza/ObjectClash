namespace CR.Servers.CoC.Packets.Messages.Client.Account
{
    using System;
    using System.Linq;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Enums;
    using CR.Servers.CoC.Packets.Messages.Server.Account;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Logic.Enums;
    using System.Threading.Tasks;

    internal class UnlockAccountMessage : Message
    {
        internal int HighId;
        internal int LowId;
        internal string UnlockCode;
        internal string UserToken;

        public UnlockAccountMessage(Device device, Reader reader) : base(device, reader)
        {
            this.Device.State = State.UNLOCK_ACC;
        }

        internal override short Type
        {
            get
            {
                return 10121;
            }
        }

        internal override void Decode()
        {
            this.HighId = this.Reader.ReadInt32();
            this.LowId = this.Reader.ReadInt32();
            this.UserToken = this.Reader.ReadString();

            this.UnlockCode = this.Reader.ReadString();
        }

        internal override async Task ProcessAsync()
        {
            this.ShowValues();
            if (this.UnlockCode.Length != 12 || string.IsNullOrEmpty(this.UnlockCode))
            {
                //Remove Device
                return;
            }

            if (this.UnlockCode[0] == '/')
            {
                int n;
                if (int.TryParse(this.UnlockCode.Substring(1), out n))
                {
                    if (n == 0)
                    {
                        //Send new player
                        new UnlockAccountOkMessage(this.Device) { Account = (await Resources.Accounts.CreateAccountAsync()).Player }.Send();
                        return;
                    }

                    int HighId = 0;
                    int LowId = 0;

                    this.ToHighAndLow(n, ref HighId, ref LowId);
                    Account Account = await Resources.Accounts.LoadAccountAsync(HighId, LowId);
                    if (Account != null)
                    {
                        Account.Player.Locked = true;
                        new UnlockAccountOkMessage(this.Device) { Account = Account.Player }.Send();
                    }
                    else
                    {
                        new UnlockAccountFailedMessage(this.Device) { Reason = UnlockAccountReason.UnlockError }.Send();
                    }
                }
                else
                {
                    new UnlockAccountFailedMessage(this.Device) { Reason = UnlockAccountReason.UnlockError }.Send();
                }
            }
            else
            {
                Player Account = await Resources.Accounts.LoadPlayerAsync(this.HighId, this.LowId);
                if (Account != null)
                {
                    if (string.Equals(this.UnlockCode, Account.Password, StringComparison.CurrentCultureIgnoreCase))
                    {
                        Account.Locked = false;
                        new UnlockAccountOkMessage(this.Device) { Account = Account }.Send();
                    }
                    else
                    {
                        new UnlockAccountFailedMessage(this.Device) { Reason = UnlockAccountReason.Default }.Send();
                    }
                }
                else
                {
                    new UnlockAccountFailedMessage(this.Device) { Reason = UnlockAccountReason.UnlockError }.Send();
                }
            }
        }

        internal void ToHighAndLow(long l, ref int High, ref int Low)
        {
            Reader bytes = new Reader(BitConverter.GetBytes(l).Reverse().ToArray());

            High = bytes.ReadInt32();
            Low = bytes.ReadInt32();
        }
    }
}