namespace CR.Servers.CoC.Logic
{
    internal class Random
    {
        internal int Seed;

        public Random()
        {
        }

        public Random(int Seed)
        {
            this.Seed = Seed;
        }

        internal int IteratedRandomSeed()
        {
            int Seed = this.Seed;

            if (Seed == 0)
            {
                Seed = -1;
            }

            int v2 = Seed ^ (Seed << 13) ^ ((Seed ^ (this.Seed << 13)) >> 17);
            return (32 * v2) ^ v2;
        }

        internal int Rand(int Max)
        {
            if (Max >= 1)
            {
                this.Seed = this.IteratedRandomSeed();

                if (this.Seed >= 0)
                {
                    return this.Seed % Max;
                }

                return -this.Seed % Max;
            }

            return 0;
        }
    }
}