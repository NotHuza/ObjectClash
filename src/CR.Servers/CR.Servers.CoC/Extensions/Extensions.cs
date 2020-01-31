namespace CR.Servers.CoC.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using CR.Servers.CoC.Logic.Enums;

    internal static class Extension
    {
        internal static ulong Pair(int a, int b)
        {
            return ((ulong) a << 32) | (uint) b;
        }

        internal static ulong Pair(uint a, uint b)
        {
            return ((ulong) a << 32) | b;
        }

        internal static uint HIDWORD(ulong Value)
        {
            return (uint) (Value >> 32);
        }

        internal static uint LODWORD(ulong Value)
        {
            return (uint) (Value >> 32);
        }

        internal static bool IsEquals<T>(this T[] Array1, T[] Array2)
        {
            if (Array1.Length == Array2.Length)
            {
                return !Array1.Where((t, i) => !t.Equals(Array2[i])).Any();
            }

            return false;
        }

        internal static bool Superior(this Role a, Role b)
        {
            if (a == Role.Leader && b != a)
            {
                return false;
            }

            if (a == Role.CoLeader && b != Role.Leader)
            {
                return false;
            }

            if (a == Role.Elder && (b == a || b == Role.Member))
            {
                return false;
            }

            if (a == Role.Member && b == a)
            {
                return false;
            }

            return true;
        }

        internal static byte[] StringToByteArray(string str)
        {
            byte[] Array = new byte[str.Length];

            for (int i = 0; i < str.Length; i++)
            {
                Array[i] = (byte) str[i];
            }

            return Array;
        }

        public static int ParseConfigInt(string str)
        {
            return int.Parse(Program.Configuration[str]);
        }

        public static bool ParseConfigBoolean(string str)
        {
            return bool.Parse(Program.Configuration[str]);
        }

        public static string ParseConfigString(string str)
        {
            return Program.Configuration[str];
        }

        public static bool IsLike(this string s, string pattern)
        {
            // Characters matched so far
            int matched = 0;

            // Loop through pattern string
            for (int i = 0; i < pattern.Length;)
            {
                // Check for end of string
                if (matched > s.Length)
                {
                    return false;
                }

                // Get next pattern character
                char c = pattern[i++];
                if (c == '[') // Character list
                {
                    // Test for exclude character
                    bool exclude = i < pattern.Length && pattern[i] == '!';
                    if (exclude)
                    {
                        i++;
                    }

                    // Build character list
                    int j = pattern.IndexOf(']', i);
                    if (j < 0)
                    {
                        j = s.Length;
                    }

                    HashSet<char> charList = Extension.CharListToSet(pattern.Substring(i, j - i));
                    i = j + 1;

                    if (charList.Contains(s[matched]) == exclude)
                    {
                        return false;
                    }

                    matched++;
                }
                else if (c == '?') // Any single character
                {
                    matched++;
                }
                else if (c == '#') // Any single digit
                {
                    if (!char.IsDigit(s[matched]))
                    {
                        return false;
                    }

                    matched++;
                }
                else if (c == '*') // Zero or more characters
                {
                    if (i < pattern.Length)
                    {
                        // Matches all characters until
                        // next character in pattern
                        char next = pattern[i];
                        int j = s.IndexOf(next, matched);
                        if (j < 0)
                        {
                            return false;
                        }

                        matched = j;
                    }
                    else
                    {
                        // Matches all remaining characters
                        matched = s.Length;
                        break;
                    }
                }
                else // Exact character
                {
                    if (matched >= s.Length || c != s[matched])
                    {
                        return false;
                    }

                    matched++;
                }
            }

            // Return true if all characters matched
            return matched == s.Length;
        }

        private static HashSet<char> CharListToSet(string charList)
        {
            HashSet<char> set = new HashSet<char>();

            for (int i = 0; i < charList.Length; i++)
            {
                if (i + 1 < charList.Length && charList[i + 1] == '-')
                {
                    // Character range
                    char startChar = charList[i++];
                    i++; // Hyphen
                    char endChar = (char) 0;
                    if (i < charList.Length)
                    {
                        endChar = charList[i++];
                    }

                    for (int j = startChar; j <= endChar; j++)
                    {
                        set.Add((char) j);
                    }
                }
                else
                {
                    set.Add(charList[i]);
                }
            }

            return set;
        }
    }
}