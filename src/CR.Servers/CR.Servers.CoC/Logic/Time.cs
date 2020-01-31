namespace CR.Servers.CoC.Logic
{
    internal class Time
    {
        internal int SubTick;

        internal int TotalMS
        {
            get
            {
                return 16 * this.SubTick;
            }
        }

        internal int TotalSecs
        {
            get
            {
                return this.SubTick > 0 ? Math.Max((int) (ulong) (16L * this.SubTick / 1000) + 1, 1) : 0;
            }
        }

        internal static int GetSecondsInTicks(int Seconds)
        {
            return (int) ((((uint) ((int) ((ulong) (1000L * Seconds) >> 32) >> 31) >> 38) + 1000L * Seconds) >> 4);
        }
    }
}