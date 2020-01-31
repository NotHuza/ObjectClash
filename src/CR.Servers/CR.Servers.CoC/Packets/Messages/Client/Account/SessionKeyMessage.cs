namespace CR.Servers.CoC.Packets.Messages.Client.Account
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Stream.Scrambler;
    using CR.Servers.Extensions.List;

    internal class SessionKeyMessage : Message
    {
        public byte[] Nonce;

        public SessionKeyMessage(Device device) : base(device)
        {
            this.Nonce = new byte[16];
            Resources.Random.NextBytes(this.Nonce);
        }

        internal override short Type
        {
            get
            {
                return 20000;
            }
        }

        internal override void Encode()
        {
            this.Data.AddByteArray(this.Nonce);
            this.Data.AddInt(1);
        }

        internal override void Process()
        {
            string scrambledNonce = null;
            RC4Scrambler rc4Scrambler = new RC4Scrambler(this.Device.EncryptionSeed);

            byte byte100 = 0;

            for (int i = 0; i < 100; i++)
            {
                byte100 = (byte) rc4Scrambler.NextInt();
            }

            for (int i = 0; i < this.Nonce.Length; i++)
            {
                scrambledNonce += (char) (this.Nonce[i] ^ ((byte) rc4Scrambler.NextInt() & byte100));
            }

            this.Device.InitRC4Encrypters(Factory.RC4Key, scrambledNonce);
        }
    }
}