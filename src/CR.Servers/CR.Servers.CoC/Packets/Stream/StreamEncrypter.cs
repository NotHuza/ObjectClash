namespace CR.Servers.CoC.Packets.Stream
{
    internal class StreamEncrypter
    {
        internal int OverheadEncryptionLength
        {
            get
            {
                return 0;
            }
        }

        internal virtual byte[] Decrypt(byte[] input)
        {
            return input;
        }

        internal virtual byte[] Encrypt(byte[] input)
        {
            return input;
        }
    }
}