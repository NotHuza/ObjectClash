using System;

namespace CR.Servers.Extensions
{
    internal static class Extensions
    {
        /// <summary>
        /// Turn two integer into a single long.
        /// </summary>
        /// <param name="pair">The pair.</param>
#if !NETCORE2_0
        public static long ToInt64(this Tuple<int, int> pair)
#else
        public static long ToInt64(this (int, int) Pair)
#endif
        {
            return (long)pair.Item1 << 32 | (long)(uint)pair.Item2;
        }

        /// <summary>
        /// Rotate the int.
        /// </summary>
        /// <param name="value">The int</param>
        /// <param name="count">Rotation Count</param>
        /// <returns></returns>
        public static int RotateRight(int value, int count)
        {
            return value << count | value >> (32 - count);
        }

        /// <summary>
        /// Get index of the value in array. Return -1 if value is not in array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The Array</param>
        /// <param name="value">The Value</param>
        /// <returns></returns>
        public static int IndexOf<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(value))
                    return i;
            }

            return -1;
        }
    }
}