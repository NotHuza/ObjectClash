namespace CR.Servers.CoC.Logic
{
    using System;
    using CR.Servers.CoC.Extensions;

    internal class Timer
    {
        internal int EndTime;
        internal int Seconds;
        internal DateTime StartTime;

        internal Timer()
        {
            this.StartTime = new DateTime();
            this.EndTime = 0;
            this.Seconds = 0;
        }

        internal bool Started
        {
            get
            {
                return this.EndTime != 0;
            }
        }


        internal DateTime GetStartTime
        {
            get
            {
                return this.StartTime;
            }
        }

        internal DateTime GetEndTime
        {
            get
            {
                return TimeUtils.FromUnixTimestamp(this.EndTime);
            }
        }


        internal void StartTimer(DateTime time, int durationSeconds)
        {
            this.StartTime = time;
            this.EndTime = (int) TimeUtils.ToUnixTimestamp(time) + durationSeconds;
            this.Seconds = durationSeconds;
        }

        internal void StopTimer()
        {
            this.EndTime = 0;
            this.Seconds = 0;
        }

        internal void FastForward(int seconds)
        {
            this.Seconds -= seconds;
            this.EndTime -= seconds;
        }

        internal void IncreaseTimer(int seconds)
        {
            this.Seconds += seconds;
            this.EndTime += seconds;
        }

        internal int GetRemainingSeconds(DateTime time)
        {
            int result = this.Seconds - (int) time.Subtract(this.StartTime).TotalSeconds;
            if (result <= 0)
            {
                result = 0;
            }

            return result;
        }


        internal int GetRemainingSeconds(DateTime time, bool boost, DateTime boostEndTime = default(DateTime), float multiplier = 0f)
        {
            int result;
            if (!boost)
            {
                result = this.Seconds - (int) time.Subtract(this.StartTime).TotalSeconds;
            }
            else
            {
                if (boostEndTime >= time)
                {
                    result = this.Seconds - (int) (time.Subtract(this.StartTime).TotalSeconds * multiplier);
                }
                else
                {
                    float boostedTime = (float) time.Subtract(this.StartTime).TotalSeconds - (float) (time - boostEndTime).TotalSeconds;
                    float notBoostedTime = (float) time.Subtract(this.StartTime).TotalSeconds - boostedTime;

                    result = this.Seconds - (int) (boostedTime * multiplier + notBoostedTime);
                }
            }

            if (result <= 0)
            {
                result = 0;
            }

            return result;
        }
    }
}