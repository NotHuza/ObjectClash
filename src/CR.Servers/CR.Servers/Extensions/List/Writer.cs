using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CR.Servers.Library.ZLib;

namespace CR.Servers.Extensions.List
{
    public static class Writer
    {
        public static void AddByte(this List<byte> _Packet, int _Value)
        {
            _Packet.Add((byte)_Value);
        }

        public static void AddInt(this List<byte> _Packet, int _Value)
        {
            _Packet.AddRange(BitConverter.GetBytes(_Value).Reverse());
        }

        public static void AddIntEndian(this List<byte> _Packet, int _Value)
        {
            _Packet.AddRange(BitConverter.GetBytes(_Value));
        }

        public static void AddInt(this List<byte> _Packet, int _Value, int _Skip)
        {
            _Packet.AddRange(BitConverter.GetBytes(_Value).Reverse().Skip(_Skip));
        }

        public static void AddInt24(this List<byte> _Packet, int _Value)
        {
            _Packet.AddRange(BitConverter.GetBytes(_Value).Reverse().Skip(1));
        }

        public static void AddLong(this List<byte> _Packet, long _Value)
        {
            _Packet.AddRange(BitConverter.GetBytes(_Value).Reverse());
        }
        public static void AddLongEndian(this List<byte> _Packet, long _Value)
        {
            _Packet.AddRange(BitConverter.GetBytes(_Value));
        }

        /*public static void AddLong(this List<byte> _Packet, long _Value, int _Skip)
        {
            _Packet.AddRange(BitConverter.GetBytes(_Value).Reverse().Skip(_Skip));
        }*/

        public static void AddBool(this List<byte> _Packet, bool _Value)
        {
            _Packet.Add(_Value ? (byte)1 : (byte)0);
        }

        public static void AddBools(this List<byte> Packet, params bool[] Booleans)
        {
            byte boolean = 0;

            for (var i = 0; i < Booleans.Length; i++)
            {
                bool Bool = Booleans[i];

                if (Bool)
                {
                    boolean |= (byte)(1 << i);
                }
            }

            Packet.Add(boolean);
        }

        public static void AddString(this List<byte> _Packet, string _Value)
        {
            if (_Value == null)
            {
                _Packet.AddInt(-1);
            }
            else
            {
                byte[] _Buffer = Encoding.UTF8.GetBytes(_Value);

                _Packet.AddInt(_Buffer.Length);
                _Packet.AddRange(_Buffer);
            }
        }

        public static void AddVInt(this List<byte> Packet, int Value)
        {
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
                                Packet.Add((byte)(Value & 0x3F | 0x80));
                                Packet.Add((byte)((Value >> 6) & 0x7F | 0x80));
                                Packet.Add((byte)((Value >> 13) & 0x7F | 0x80));
                                Packet.Add((byte)((Value >> 20) & 0x7F | 0x80));
                                Packet.Add((byte)((Value >> 27) & 0xF));

                                return;
                            }

                            Packet.Add((byte)(Value & 0x3F | 0x80));
                            Packet.Add((byte)((Value >> 6) & 0x7F | 0x80));
                            Packet.Add((byte)((Value >> 13) & 0x7F | 0x80));
                            Packet.Add((byte)((Value >> 20) & 0x7F));

                            return;
                        }

                        Packet.Add((byte)(Value & 0x3F | 0x80));
                        Packet.Add((byte)((Value >> 6) & 0x7F | 0x80));
                        Packet.Add((byte)((Value >> 13) & 0x7F));

                        return;
                    }

                    Packet.Add((byte)(Value & 0x3F | 0x80));
                    Packet.Add((byte)((Value >> 6) & 0x7F));

                    return;
                }

                Packet.Add((byte)(Value & 0x3F));
            }
            else
            {
                if (Value <= -64)
                {
                    if (Value <= -0x2000)
                    {
                        if (Value <= -0x100000)
                        {
                            if (Value <= -0x8000000)
                            {
                                Packet.Add((byte)(Value & 0x3F | 0xC0));
                                Packet.Add((byte)((Value >> 6) & 0x7F | 0x80));
                                Packet.Add((byte)((Value >> 13) & 0x7F | 0x80));
                                Packet.Add((byte)((Value >> 20) & 0x7F | 0x80));
                                Packet.Add((byte)((Value >> 27) & 0xF));

                                return;
                            }

                            Packet.Add((byte)(Value & 0x3F | 0xC0));
                            Packet.Add((byte)((Value >> 6) & 0x7F | 0x80));
                            Packet.Add((byte)((Value >> 13) & 0x7F | 0x80));
                            Packet.Add((byte)((Value >> 20) & 0x7F));

                            return;
                        }

                        Packet.Add((byte)(Value & 0x3F | 0xC0));
                        Packet.Add((byte)((Value >> 6) & 0x7F | 0x80));
                        Packet.Add((byte)((Value >> 13) & 0x7F));

                        return;
                    }

                    Packet.Add((byte)(Value & 0x3F | 0xC0));
                    Packet.Add((byte)((Value >> 6) & 0x7F));
                }
                else
                {
                    Packet.Add((byte)(Value & 0x3F | 0x40));
                }
            }
        }

        public static void AddVInt(this List<byte> _Packet, uint _Value)
        {
            _Packet.AddVInt((int)_Value);
        }

        public static void AddVInt(this List<byte> _Packet, int _Value, int _Prefix)
        {
            _Packet.AddVInt(_Prefix);
            _Packet.AddVInt(_Value);
        }

        public static void AddUShort(this List<byte> _Packet, ushort _Value)
        {
            _Packet.AddRange(BitConverter.GetBytes(_Value).Reverse());
        }

        public static void AddShort(this List<byte> _Packet, short _Value)
        {
            _Packet.AddRange(BitConverter.GetBytes(_Value).Reverse());
        }

        public static void AddUInt24(this List<byte> _Packet, uint _Value)
        {
            _Packet.AddRange(BitConverter.GetBytes(_Value).Reverse().Skip(1));
        }

       public static void AddCompressed(this List<byte> _Packet, string _Value, bool addbool = true)
        {
            if (addbool)
                _Packet.AddBool(true);

            if (_Value == null)
                _Packet.AddInt(-1);
            else
            {
                byte[] Compressed = ZlibStream.CompressString(_Value);

                _Packet.AddInt(Compressed.Length + 4);
                _Packet.AddIntEndian(_Value.Length);
                _Packet.AddRange(Compressed);
            }
        }

        public static void AddHexa(this List<byte> _Packet, string _Value)
        {
            _Packet.AddRange(_Value.HexaToBytes());
        }

        public static byte[] HexaToBytes(this string _Value)
        {
            string _Tmp = _Value.Contains("-") ? _Value.Replace("-", string.Empty) : _Value.Replace(" ", string.Empty);
            return Enumerable.Range(0, _Tmp.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(_Tmp.Substring(x, 2), 16)).ToArray();
        }

        /*internal static void AddData(this List<byte> _Writer, Data _Data)
        {
            int Reference = _Data.Id;
            int RowIndex = _Data.GetId();

            _Writer.AddInt(Reference);
            _Writer.AddInt(RowIndex);
        }

        internal static void AddDataSlots(this List<byte> _Packet, List<Slot> data)
        {
            _Packet.AddInt(data.Count);
            foreach (var dataSlot in data)
            {
                _Packet.AddInt(dataSlot.Data);
                _Packet.AddInt(dataSlot.Count);
            }
        }*/

        public static void AddByteArray(this List<byte> _Packet, byte[] data)
        {
            if (data == null)
                _Packet.AddInt(-1);
            else
            {
                _Packet.AddInt(data.Length);
                _Packet.AddRange(data);
            }
        }

        /*internal static void Shuffle<T>(this IList<T> List)
        {
            int c = List.Count;

            while (c > 1)
            {
                c--;

                int r = Resources.Random.Next(c + 1);

                T Value = List[r];
                List[r] = List[c];
                List[c] = Value;
            }
        }*/
    }
}
