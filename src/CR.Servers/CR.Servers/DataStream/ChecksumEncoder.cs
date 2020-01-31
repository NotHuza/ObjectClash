using System;

namespace CR.Servers.DataStream
{
    public class ChecksumEncoder
    {
        private int _BefChecksum;
        private bool _Enabled;

        /// <summary>
        ///     Gets the checksum of this stream.
        /// </summary>
        public int Checksum { get; private set; }

        /// <summary>
        ///     Gets the byte stream instance.
        /// </summary>
        public ByteStream ByteStream { get; private set; }

        /// <summary>
        ///     Gets if this instance is checksum only mode.
        /// </summary>
        public virtual bool IsCheckSumOnlyMode => true;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChecksumEncoder" /> class.
        /// </summary>
        public ChecksumEncoder(ByteStream ByteStream)
        {
            _Enabled = true;
            this.ByteStream = ByteStream;
        }

        /// <summary>
        ///     Writes a byte value.
        /// </summary>
        public virtual void AddByte(byte Value)
        {
            Checksum = Extensions.Extensions.RotateRight(Checksum, 31) + Value + 11;
            ByteStream?.AddByte(Value);
        }

        /// <summary>
        ///     Writes a boolean value.
        /// </summary>
        public virtual void AddBoolean(bool Value)
        {
            Checksum = Extensions.Extensions.RotateRight(Checksum, 31) + (Value ? 13 : 7);
            ByteStream?.AddBoolean(Value);
        }

        /// <summary>
        ///     Writes a short value.
        /// </summary>
        public virtual void AddShort(short Value)
        {
            Checksum = Extensions.Extensions.RotateRight(Checksum, 31) + Value + 19;
            ByteStream?.AddShort(Value);
        }

        /// <summary>
        ///     Writes a ushort value.
        /// </summary>
        public virtual void AddUShort(ushort Value)
        {
            Checksum = Extensions.Extensions.RotateRight(Checksum, 31) + Value + 20;
            ByteStream?.AddUShort(Value);
        }

        /// <summary>
        ///     Writes a int value.
        /// </summary>
        public virtual void AddInt(int Value)
        {
            Checksum = Extensions.Extensions.RotateRight(Checksum, 31) + Value + 9;
            ByteStream?.AddInt(Value);
        }

        /// <summary>
        ///     Writes a uint value.
        /// </summary>
        public virtual void AddUInt(uint Value)
        {
            Checksum = Extensions.Extensions.RotateRight(Checksum, 31) + (int) Value + 21;
            ByteStream?.AddUInt(Value);
        }

        /// <summary>
        ///     Writes a long value.
        /// </summary>
        public virtual void AddLong(long Value)
        {
            Checksum = (int) ((Value >> 32) +
                              Extensions.Extensions.RotateRight(
                                  (int) (Value >> 32) + Extensions.Extensions.RotateRight((int) Value, 31) + 67, 31) +
                              91);
            ByteStream?.AddLong(Value);
        }

        /// <summary>
        ///     Writes a ulong value.
        /// </summary>
        public virtual void AddULong(ulong Value)
        {
            Checksum = (int) ((Value >> 32) +
                              (ulong) Extensions.Extensions.RotateRight(
                                  (int) (uint) (Value >> 32) +
                                  Extensions.Extensions.RotateRight((int) (uint) Value, 31) + 67, 31) + 91);
            ByteStream?.AddULong(Value);
        }

        /// <summary>
        ///     Writes a byte array.
        /// </summary>
        public virtual void AddBytes(byte[] Buffer)
        {
            var ROR = Extensions.Extensions.RotateRight(Checksum, 31);

            if (Buffer != null)
                Checksum = ROR + Buffer.Length + 28;
            else
                Checksum = ROR + 27;

            ByteStream?.AddBytes(Buffer);
        }

        /// <summary>
        ///     Writes a string.
        /// </summary>
        public virtual void AddString(string String)
        {
            var ROR = Extensions.Extensions.RotateRight(Checksum, 31);

            if (String != null)
                Checksum = ROR + String.Length + 28;
            else
                Checksum = ROR + 27;

            ByteStream?.AddString(String);
        }

        /// <summary>
        ///     Writes a string reference.
        /// </summary>
        public virtual void AddStringReference(string String)
        {
            if (String == null)
                throw new ArgumentNullException("String");

            Checksum = Extensions.Extensions.RotateRight(Checksum, 31) + String.Length + 9;
            ByteStream?.AddStringReference(String);
        }

        /// <summary>
        ///     Writes a vint.
        /// </summary>
        public virtual void AddVInt(int Value)
        {
            Checksum = Extensions.Extensions.RotateRight(Checksum, 31) + Value + 33;
            ByteStream?.AddVInt(Value);
        }

        /// <summary>
        ///     Adds range to byte stream.
        /// </summary>
        public virtual void AddRange(byte[] Packet)
        {
            ByteStream?.AddRange(Packet);
        }

        /// <summary>
        ///     Sets if encoder is enabled.
        /// </summary>
        public void EnableCheckSum(bool Value)
        {
            if (!_Enabled || Value)
            {
                if (!_Enabled && Value)
                    Checksum = _BefChecksum;
            }
            else
            {
                _BefChecksum = Checksum;
            }

            _Enabled = Value;
        }

        /// <summary>
        ///     Resets the checksum of this instance.
        /// </summary>
        public void ResetChecksum()
        {
            Checksum = 0;
        }

        /// <summary>
        ///     Sets the bytestream instance.
        /// </summary>
        public void SetByteStream(ByteStream ByteStream)
        {
            this.ByteStream = ByteStream;
        }

        ~ChecksumEncoder()
        {
            ByteStream = null;
        }
    }
}