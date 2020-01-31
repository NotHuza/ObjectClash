namespace CR.Servers.CoC.Packets.Stream.Scrambler
{
    internal class RC4Scrambler
    {
        private const int MATRIX_A = -1727483681;
        private const int UPPER_MASK = -2147483648;
        private const int LOWER_MASK = 0x7fffffff;
        private const int TEMPERING_MASK_B = -1658038656;
        private const int TEMPERING_MASK_C = -272236544;

        private readonly int[] _seeds = new int[624];
        private int _ix;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RC4Scrambler" /> class.
        /// </summary>
        public RC4Scrambler(int seed)
        {
            for (int i = 0; i < this._seeds.Length; i++)
            {
                this._seeds[i] = seed;
                seed = 1812433253 * ((seed ^ (seed >> 30)) + 1);
            }
        }

        /// <summary>
        ///     Gets the next random int value.
        /// </summary>
        public int NextInt()
        {
            if (this._ix == 0)
            {
                for (int i = 1, j = 0; i <= this._seeds.Length; i++, j++)
                {
                    int v4 = (this._seeds[i % this._seeds.Length] & RC4Scrambler.LOWER_MASK) + (this._seeds[j] & RC4Scrambler.UPPER_MASK);
                    int v6 = (v4 >> 1) ^ this._seeds[(i + 396) % this._seeds.Length];

                    if ((v4 & 1) == 1)
                    {
                        v6 ^= RC4Scrambler.MATRIX_A;
                    }

                    this._seeds[j] = v6;
                }
            }

            int val = this._seeds[this._ix];

            val ^= (val >> 11) ^ (((val ^ (val >> 11)) << 7) & RC4Scrambler.TEMPERING_MASK_B);
            val = ((val ^ ((val << 15) & RC4Scrambler.TEMPERING_MASK_C)) >> 18) ^ val ^ ((val << 15) & RC4Scrambler.TEMPERING_MASK_C);

            this._ix = (this._ix + 1) % this._seeds.Length;

            return val;
        }
    }
}