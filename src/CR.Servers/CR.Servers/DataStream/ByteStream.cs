using System;
using System.Text;

namespace CR.Servers.DataStream
{
    public class ByteStream : ChecksumEncoder
    {
        protected int _Offset;
        protected byte[] Buffer;

        protected int BooleanOffset;
        protected int BooleanAdditionalValue;

        /// <summary>
        ///     Gets the offset of stream.
        /// </summary>
        public int Offset => _Offset;

        /// <summary>
        ///     Gets the offset of stream.
        /// </summary>
        public int Length => Buffer.Length;

        /// <summary>
        ///     Gets if this instance is checksum only mode.
        /// </summary>
        public override bool IsCheckSumOnlyMode => false;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ByteStream" /> class.
        /// </summary>
        public ByteStream() : base(null)
        {
            Buffer = new byte[32];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ByteStream" /> class.
        /// </summary>
        public ByteStream(int Size) : base(null)
        {
            Buffer = new byte[Size];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ByteStream" /> class.
        /// </summary>
        public ByteStream(byte[] Buffer) : base(null)
        {
            this.Buffer = Buffer;
        }

        /// <summary>
        ///     Adds a byte value.
        /// </summary>
        public override void AddByte(byte Value)
        {
            EnsureCapacity(1);
            Add(Value);
        }

        /// <summary>
        ///     Adds a boolean value.
        /// </summary>
        public override void AddBoolean(bool Value)
        {
            if (BooleanOffset == 0)
            {
                EnsureCapacity(1);
                Add(0);
            }

            if (Value)
                Buffer[Offset - 1] |= (byte) (1 << BooleanOffset);

            BooleanOffset = (BooleanOffset + 1) & 7;
        }

        /// <summary>
        ///     Adds a short value.
        /// </summary>
        public override void AddShort(short Value)
        {
            EnsureCapacity(2);

            Add((byte) (Value >> 8));
            Add((byte) Value);
        }

        /// <summary>
        ///     Adds a ushort value.
        /// </summary>
        public override void AddUShort(ushort Value)
        {
            EnsureCapacity(2);

            Add((byte) (Value >> 8));
            Add((byte) Value);
        }

        /// <summary>
        ///     Adds a int value.
        /// </summary>
        public override void AddInt(int Value)
        {
            EnsureCapacity(4);

            Add((byte) (Value >> 24));
            Add((byte) (Value >> 16));
            Add((byte) (Value >> 8));
            Add((byte) Value);
        }

        /// <summary>
        ///     Adds a uint value.
        /// </summary>
        public override void AddUInt(uint Value)
        {
            EnsureCapacity(4);

            Add((byte) (Value >> 24));
            Add((byte) (Value >> 16));
            Add((byte) (Value >> 8));
            Add((byte) Value);
        }

        /// <summary>
        ///     Adds a long value.
        /// </summary>
        public override void AddLong(long Value)
        {
            EnsureCapacity(8);

            Add((byte) (Value >> 56));
            Add((byte) (Value >> 48));
            Add((byte) (Value >> 40));
            Add((byte) (Value >> 32));
            Add((byte) (Value >> 24));
            Add((byte) (Value >> 16));
            Add((byte) (Value >> 8));
            Add((byte) Value);
        }

        /// <summary>
        ///     Adds a long value.
        /// </summary>
        public void AddLong(int hi, int lo)
        {
            AddInt(hi);
            AddInt(lo);
        }

        /// <summary>
        ///     Adds a ulong value.
        /// </summary>
        public override void AddULong(ulong Value)
        {
            EnsureCapacity(8);

            Add((byte) (Value >> 56));
            Add((byte) (Value >> 48));
            Add((byte) (Value >> 40));
            Add((byte) (Value >> 32));
            Add((byte) (Value >> 24));
            Add((byte) (Value >> 16));
            Add((byte) (Value >> 8));
            Add((byte) Value);
        }

        /// <summary>
        ///     Adds a byte array.
        /// </summary>
        public override void AddBytes(byte[] Buffer)
        {
            if (Buffer != null)
            {
                var Length = Buffer.Length;

                if (Length > 0)
                {
                    EnsureCapacity(Length + 4);

                    AddInt(Length);
                    AddRange(Buffer, Length);
                }
                else
                {
                    AddInt(0);
                }
            }
            else
            {
                AddInt(-1);
            }
        }

        /// <summary>
        ///     Adds a string.
        /// </summary>
        public override void AddString(string String)
        {
            if (String != null)
            {
                var Length = String.Length;

                if (Length > 0)
                {
                    EnsureCapacity(Length + 4);

                    AddInt(Length);
                    AddRange(Encoding.UTF8.GetBytes(String), Length);
                }
                else
                {
                    AddInt(0);
                }
            }
            else
            {
                AddInt(-1);
            }
        }

        /// <summary>
        ///     Adds a string reference.
        /// </summary>
        public override void AddStringReference(string String)
        {
            if (String == null)
                throw new ArgumentNullException("ByteStream::AddStringReference() - String cannot be NULL.");

            var Length = String.Length;

            if (Length > 0)
            {
                EnsureCapacity(Length + 4);

                AddInt(Length);
                AddRange(Encoding.UTF8.GetBytes(String), Length);
            }
            else
            {
                AddInt(0);
            }
        }

        /// <summary>
        ///     Adds a vint.
        /// </summary>
        public override void AddVInt(int Value)
        {
            EnsureCapacity(5);

            if (Value >= 0)
            {
                if (Value >= 64)
                {
                    if (Value >= 0x2000)
                    {
                        if (Value >= 0x100000)
                        {
                            if (Value >= 0x8000000)
                            {
                                Add((byte) ((Value & 0x3F) | 0x80));
                                Add((byte) (((Value >> 6) & 0x7F) | 0x80));
                                Add((byte) (((Value >> 13) & 0x7F) | 0x80));
                                Add((byte) (((Value >> 20) & 0x7F) | 0x80));
                                Add((byte) ((Value >> 27) & 0xF));

                                return;
                            }

                            Add((byte) ((Value & 0x3F) | 0x80));
                            Add((byte) (((Value >> 6) & 0x7F) | 0x80));
                            Add((byte) (((Value >> 13) & 0x7F) | 0x80));
                            Add((byte) ((Value >> 20) & 0x7F));

                            return;
                        }

                        Add((byte) ((Value & 0x3F) | 0x80));
                        Add((byte) (((Value >> 6) & 0x7F) | 0x80));
                        Add((byte) ((Value >> 13) & 0x7F));

                        return;
                    }

                    Add((byte) ((Value & 0x3F) | 0x80));
                    Add((byte) ((Value >> 6) & 0x7F));

                    return;
                }

                Add((byte) (Value & 0x3F));
            }
            else
            {
                if (Value <= -0x40)
                {
                    if (Value <= -0x2000)
                    {
                        if (Value <= -0x100000)
                        {
                            if (Value <= -0x8000000)
                            {
                                Add((byte) ((Value & 0x3F) | 0xC0));
                                Add((byte) (((Value >> 6) & 0x7F) | 0x80));
                                Add((byte) (((Value >> 13) & 0x7F) | 0x80));
                                Add((byte) (((Value >> 20) & 0x7F) | 0x80));
                                Add((byte) ((Value >> 27) & 0xF));

                                return;
                            }

                            Add((byte) ((Value & 0x3F) | 0xC0));
                            Add((byte) (((Value >> 6) & 0x7F) | 0x80));
                            Add((byte) (((Value >> 13) & 0x7F) | 0x80));
                            Add((byte) ((Value >> 20) & 0x7F));

                            return;
                        }

                        Add((byte) ((Value & 0x3F) | 0xC0));
                        Add((byte) (((Value >> 6) & 0x7F) | 0x80));
                        Add((byte) ((Value >> 13) & 0x7F));

                        return;
                    }

                    Add((byte) ((Value & 0x3F) | 0xC0));
                    Add((byte) ((Value >> 6) & 0x7F));
                }
                else
                {
                    Add((byte) ((Value & 0x3F) | 0x40));
                }
            }
        }

        /// <summary>
        ///     Adds element to buffer.
        /// </summary>
        private void Add(byte Value)
        {
            Buffer[_Offset++] = Value;
        }

        /// <summary>
        ///     Adds element to buffer.
        /// </summary>
        public override void AddRange(byte[] Buffer)
        {
            EnsureCapacity(Buffer.Length);

            Array.Copy(Buffer, 0, this.Buffer, _Offset, Buffer.Length);
            _Offset += Buffer.Length;
        }

        /// <summary>
        ///     Adds element to buffer.
        /// </summary>
        public void AddRange(byte[] Buffer, int Length)
        {
            EnsureCapacity(Buffer.Length);

            Array.Copy(Buffer, 0, this.Buffer, _Offset, Length);
            _Offset += Length;
        }

        /// <summary>
        ///     Ensures the capacity of buffer.
        /// </summary>
        private void EnsureCapacity(int Count)
        {
            if (BooleanOffset > 0)
            {
                BooleanOffset = 0;
                BooleanAdditionalValue = 0;
            }

            if (Buffer.Length < _Offset + Count)
            {
                var NBuffer = new byte[Buffer.Length * 2 > Buffer.Length + Count
                    ? Buffer.Length * 2
                    : Buffer.Length + Count];
                Array.Copy(Buffer, 0, NBuffer, 0, _Offset);
                Buffer = NBuffer;
            }
        }

        /// <summary>
        ///     Reads a byte value.
        /// </summary>
        public byte ReadByte()
        {
            return Read();
        }

        /// <summary>
        ///     Reads a boolean value.
        /// </summary>
        public bool ReadBoolean()
        {
            if (BooleanOffset == 0)
            {
                ++_Offset;
                BooleanAdditionalValue = 0;
            }

            BooleanAdditionalValue += (8 - BooleanOffset) >> 3;
            var Value = ((1 << BooleanOffset) & (Buffer[_Offset - 1] + BooleanAdditionalValue - 1)) != 0;
            BooleanOffset = (BooleanOffset + 1) & 7;

            return Value;
        }

        /// <summary>
        ///     Reads a short value.
        /// </summary>
        public short ReadShort()
        {
            return (short) ((Read() << 8) | Read());
        }

        /// <summary>
        ///     Reads a ushort value.
        /// </summary>
        public ushort ReadUShot()
        {
            return (ushort) ((Read() << 8) | Read());
        }

        /// <summary>
        ///     Reads a int value.
        /// </summary>
        public int ReadInt()
        {
            return (Read() << 24) | (Read() << 16) | (Read() << 8) | Read();
        }

        /// <summary>
        ///     Reads a uint value.
        /// </summary>
        public uint ReadUInt()
        {
            return (uint) ((Read() << 24) | (Read() << 16) | (Read() << 8) | Read());
        }

        /// <summary>
        ///     Reads a long value.
        /// </summary>
        public long ReadLong()
        {
            return (Read() << 56) | (Read() << 48) | (Read() << 40) | (Read() << 32) |
                   (Read() << 24) | (Read() << 16) | (Read() << 8) | Read();
        }

        /// <summary>
        ///     Reads a ulong value.
        /// </summary>
        public ulong ReadULong()
        {
            return (ulong) ((Read() << 56) | (Read() << 48) | (Read() << 40) | (Read() << 32) |
                            (Read() << 24) | (Read() << 16) | (Read() << 8) | Read());
        }

        /// <summary>
        ///     Reads a byte array.
        /// </summary>
        public byte[] ReadBytes()
        {
            var Length = ReadInt();

            if (Length < 0)
            {
                if (Length != -1)
                    throw new Exception("ByteStream::readBytes() - Byte array length is invalid. (" + Length + ")");

                return null;
            }

            return ReadRange(Length);
        }

        /// <summary>
        ///     Reads a byte array reference.
        /// </summary>
        public byte[] ReadBytesReference()
        {
            var Length = ReadInt();

            if (Length < 0)
                throw new Exception(
                    "ByteStream::readBytesReference() - Byte array length cannot be inferior than 0! (" + Length + ")");

            return ReadRange(Length);
        }

        /// <summary>
        ///     Reads a string value.
        /// </summary>
        public string ReadString()
        {
            var Length = ReadInt();

            if (Length < 0)
            {
                if (Length != -1)
                    throw new Exception("ByteStream::readString() - String length is invalid. (" + Length + ")");

                return null;
            }

            return Encoding.UTF8.GetString(ReadRange(Length));
        }

        /// <summary>
        ///     Reads a string reference.
        /// </summary>
        public string ReadStringReference()
        {
            var Length = ReadInt();

            if (Length < 0)
                throw new Exception("ByteStream::readStringReference() - String length cannot be inferior than 0! (" +
                                    Length + ")");

            return Encoding.UTF8.GetString(ReadRange(Length));
        }

        /// <summary>
        ///     Reads a vint.
        /// </summary>
        public int ReadVInt()
        {
            var Byte = ReadByte();
            int Result;

            if ((Byte & 0x40) != 0)
            {
                Result = Byte & 0x3F;

                if ((Byte & 0x80) != 0)
                {
                    Result |= ((Byte = Read()) & 0x7F) << 6;

                    if ((Byte & 0x80) != 0)
                    {
                        Result |= ((Byte = Read()) & 0x7F) << 13;

                        if ((Byte & 0x80) != 0)
                        {
                            Result |= ((Byte = Read()) & 0x7F) << 20;

                            if ((Byte & 0x80) != 0)
                            {
                                Result |= ((Byte = Read()) & 0x7F) << 27;
                                return (int) (Result | 0x80000000);
                            }

                            return (int) (Result | 0xF8000000);
                        }

                        return (int) (Result | 0xFFF00000);
                    }

                    return (int) (Result | 0xFFFFE000);
                }

                return (int) (Result | 0xFFFFFFC0);
            }
            Result = Byte & 0x3F;

            if ((Byte & 0x80) != 0)
            {
                Result |= ((Byte = Read()) & 0x7F) << 6;

                if ((Byte & 0x80) != 0)
                {
                    Result |= ((Byte = Read()) & 0x7F) << 13;

                    if ((Byte & 0x80) != 0)
                    {
                        Result |= ((Byte = Read()) & 0x7F) << 20;

                        if ((Byte & 0x80) != 0)
                            Result |= ((Byte = Read()) & 0x7F) << 27;
                    }
                }
            }

            return Result;
        }

        /// <summary>
        ///     Reads a element of buffer.
        /// </summary>
        private byte Read()
        {
            if (BooleanOffset > 0)
            {
                BooleanOffset = 0;
                BooleanAdditionalValue = 0;
            }

            return Buffer[_Offset++];
        }

        /// <summary>
        ///     Reads a element of buffer.
        /// </summary>
        private byte[] ReadRange(int Count)
        {
            var Array = new byte[Count];
            System.Buffer.BlockCopy(Buffer, Offset, Array, 0, Count);
            _Offset += Count;
            return Array;
        }

        /// <summary>
        ///     Sets the byte array of this instance.
        /// </summary>
        public void SetByteArray(byte[] NBuffer)
        {
            Buffer = null;
            Buffer = NBuffer;

            _Offset = NBuffer.Length;
            BooleanOffset = 0;
            BooleanAdditionalValue = 0;
        }

        /// <summary>
        ///     Sets the offset of stream.
        /// </summary>
        public void SetOffset(int Offset)
        {
            if (BooleanOffset > 0)
            {
                BooleanOffset = 0;
                BooleanAdditionalValue = 0;
            }

            _Offset = Offset;
        }

        /// <summary>
        ///     Converted this instance to byte array.
        /// </summary>
        public byte[] ToArray()
        {
            var bytes = new byte[Offset];
            Array.Copy(Buffer, 0, bytes, 0, Offset);
            return bytes;
        }

        /// <summary>
        ///     Converted this instance to byte array.
        /// </summary>
        public byte[] ToArray(int startOffset)
        {
            var bytes = new byte[Offset - startOffset];
            Array.Copy(Buffer, startOffset, bytes, 0, Offset - startOffset);
            return bytes;
        }

        /// <summary>
        ///     Converted this instance to byte array.
        /// </summary>
        public byte[] ToArray(int startOffset, int length)
        {
            var bytes = new byte[length];
            Array.Copy(Buffer, startOffset, bytes, 0, length);
            return bytes;
        }
    }
}