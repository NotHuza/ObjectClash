namespace CR.Servers.CoC.Packets.Messages.Client.API
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class BindGamecenterAccountMessage : Message
    {
        internal string BundleId;

        internal bool Force;
        internal string GameCenterId;
        internal byte[] Salt;

        internal byte[] Signature;
        internal byte[] Timestamp;
        internal string Url;

        public BindGamecenterAccountMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 14212;
            }
        }

        internal override void Decode()
        {
            this.Force = this.Reader.ReadBoolean();

            this.GameCenterId = this.Reader.ReadString();
            this.Url = this.Reader.ReadString();
            this.BundleId = this.Reader.ReadString();

            this.Signature = this.Reader.ReadArray();
            this.Salt = this.Reader.ReadArray();
            this.Timestamp = this.Reader.ReadArray();
        }

        internal override void Process()
        {
            /*Player Bounded = await Resources.Players.FindPlayer(T => !(T.HighID == this.Device.GameMode.Level.Player.HighID && T.LowID == this.Device.GameMode.Level.Player.LowID) && T.GameCenterID == this.GameCenterId);

            if (Bounded != null)
            {
                new GameCenter_Account_Already_Bound_Message(this.Device, Bounded).Send();
                return;
            }
            
            Mongo.Players.UpdateOne(T => T.HighId == this.Device.GameMode.Level.Player.HighID && T.LowId == this.Device.GameMode.Level.Player.LowID, Builders<Players>.Update.Set(T => T.GameCenterId, this.GameCenterId));
      
            new GameCenter_Account_Bound_Message(this.Device, 1).Send();*/
        }
    }
}