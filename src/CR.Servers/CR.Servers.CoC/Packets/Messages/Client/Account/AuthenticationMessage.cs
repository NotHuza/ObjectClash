namespace CR.Servers.CoC.Packets.Messages.Client.Account
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Enums;
    using CR.Servers.CoC.Packets.Messages.Server.Account;
    using CR.Servers.CoC.Packets.Messages.Server.Alliances;
    using CR.Servers.CoC.Packets.Messages.Server.Avatar;
    using CR.Servers.CoC.Packets.Messages.Server.Home;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Logic.Enums;
    using System;
    using System.Threading.Tasks;

    internal class AuthenticationMessage : Message
    {
        internal int HighId;

        internal int LocaleId;
        internal int LowId;

        internal int Major, Minor, Revision;

        internal string Token, MasterHash;


        public AuthenticationMessage(Device device, Reader reader) : base(device, reader)
        {
            this.Device.State = State.LOGIN;
        }

        internal override short Type => 10101;

        internal override void Decode()
        {
            this.HighId = this.Reader.ReadInt32();
            this.LowId = this.Reader.ReadInt32();

            this.Token = this.Reader.ReadString();

            this.Major = this.Reader.ReadInt32();
            this.Minor = this.Reader.ReadInt32();
            this.Revision = this.Reader.ReadInt32();

            this.MasterHash = this.Reader.ReadString();

            this.Reader.ReadString();
            this.Device.Info.UDID = this.Reader.ReadString();
            this.Device.Info.OpenUDID = this.Reader.ReadString();
            this.Device.Info.DeviceModel = this.Reader.ReadString();

            this.LocaleId = this.Reader.ReadInt32();
            this.Device.Info.PreferredLanguage = this.Reader.ReadString();

            this.Device.Info.ADID = this.Reader.ReadString();

            this.Device.Info.OSVersion = this.Reader.ReadString();

            this.Device.Info.Android = this.Reader.ReadBoolean();

            this.Reader.ReadString();
            this.Device.Info.AndroidID = this.Reader.ReadString();

            this.Reader.ReadString();

            this.Device.Info.Advertising = this.Reader.ReadBoolean();
            this.Reader.ReadString();

            this.Device.EncryptionSeed = this.Reader.ReadInt32();
            this.Reader.ReadVInt();
            this.Reader.ReadString();
            this.Reader.ReadString();

            this.Device.Info.ClientVersion = this.Reader.ReadString().Split('.');

            this.Reader.ReadString();
            this.Reader.ReadString();

            this.Reader.ReadVInt();
        }

        internal override async Task ProcessAsync()
        {
            if (!this.CheckClient())
            {
                return;
            }

            if (Resources.Closing)
            {
                new AuthenticationFailedMessage(this.Device, LoginFailedReason.UpdateInProgress) { Message = "The server is currently exiting and saving data, and you cannot log-in during this time. Please try again in a few minutes." }.Send();
                return;
            }

            if (this.HighId == 0 && this.LowId == 0 && this.Token == null)
            {
                await this.LoginAsync(await Resources.Accounts.CreateAccountAsync());
            }
            else
            {
                Account account = await Resources.Accounts.LoadAccountAsync(this.HighId, this.LowId);

                if (account != null)
                {
                    if (account.Player != null && account.Home != null)
                    {
                        if (string.Equals(this.Token, account.Player.Token))
                        {
                            if (!account.Player.Locked)
                            {
                                if (!account.Player.Banned)
                                {
                                    if (account.Device != null)
                                    {
                                        if (account.Device.Connected)
                                        {
                                            new DisconnectedMessage(account.Device).Send();
                                        }
                                        else
                                        {
                                            account.Device = null;
                                        }
                                    }

                                    await this.LoginAsync(account);
                                }
                                else
                                {
                                    new AuthenticationFailedMessage(this.Device, LoginFailedReason.Banned).Send();
                                }
                            }
                            else
                            {
                                new AuthenticationFailedMessage(this.Device, LoginFailedReason.Locked).Send();
                            }
                        }
                        else
                        {
                            new AuthenticationFailedMessage(this.Device, LoginFailedReason.Locked).Send();
                        }
                    }
                    else
                    {
                        new AuthenticationFailedMessage(this.Device, LoginFailedReason.Locked).Send();
                    }
                }
                else
                {
                    new AuthenticationFailedMessage(this.Device, LoginFailedReason.Locked).Send();
                }
            }
        }

        internal bool CheckClient()
        {
            if (this.Valid())
            {
                if (this.Device.Info.ClientVersion[0] == Settings.ClientMajorVersion && this.Device.Info.ClientVersion[1] == Settings.ClientMinorVersion)
                {
                    //TODO: Maintenance

                    if (!string.IsNullOrEmpty(Settings.PatchServer))
                    {
                        if (!string.IsNullOrEmpty(Fingerprint.Json) && !string.Equals(this.MasterHash, Fingerprint.Sha))
                        {
                            new AuthenticationFailedMessage(this.Device, LoginFailedReason.Patch).Send();
                            return false;
                        }
                    }

                    return true;
                }

                new AuthenticationFailedMessage(this.Device, LoginFailedReason.Update).Send();
            }
            else
            {
                Resources.Gateway.Disconnect(this.Device.Token.Args);
            }

            return false;
        }

        internal bool Valid()
        {
            if (this.HighId >= 0)
            {
                if (this.LowId >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        internal async Task LoginAsync(Account account)
        {
            account.Device = this.Device;

            this.Device.Account = account;
            this.Device.GameMode.LoadLevel(account.Player.Level);

            this.Device.Info.Locale = this.LocaleId >= 0 ? this.LocaleId : 2000000;

            Player Player = account.Player;

            if (this.Device.UseRC4)
            {
                new SessionKeyMessage(this.Device).Send();
            }

            new AuthenticationOkMessage(this.Device).Send();

            if (account.Battle != null)
            {
                if (!account.Battle.Ended)
                {
                    if (account.Battle.Defender.Home.LowID == account.Home.LowID)
                    {
                        new WaitingToGoHomeMessage(this.Device, account.Battle.RemainingBattleTimeSecs).Send();

                        account.Battle.AddViewer(this.Device);
                        goto sendAllianceMessage;
                    }

                    await account.Battle.EndBattleAsync();
                    account.Battle = null;
                }
                else
                {
                    account.Battle = null;
                }
            }

            new OwnHomeDataMessage(this.Device).Send();
            new AvatarStreamMessage(this.Device).Send();

            sendAllianceMessage:

            if (Player.InAlliance)
            {
                new AllianceDataMessage(this.Device)
                {
                    Alliance = Player.Alliance
                }.Send();

                new AllianceStreamMessage(this.Device)
                {
                    Alliance = Player.Alliance
                }.Send();

                this.Device.GameMode.Level.Player.Alliance.Members.Connected.TryAdd(Player.UserId, Player);
                Player.Alliance.IncrementTotalConnected();
            }

            /*try
            {
                new FriendListMessage(this.Device).Send();
            }
            catch (Exception Exception)
            {
                this.Device.GameMode.Level.Player.Friends.Slots.Clear();
                Logging.Error(Exception.GetType(), "Exception while sending FriendListMessage");
            }*/

            if (this.Device.Chat == null)
            {
                Resources.Chats.Join(this.Device);
            }
        }
    }
}