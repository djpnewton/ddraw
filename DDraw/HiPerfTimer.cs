// originally sourced from http://www.eggheadcafe.com/articles/20021111.asp
using System;
using System.Runtime.InteropServices;

namespace DDraw
{
    public class HiPerfTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long startTime, stopTime;
        private long freq;

        public HiPerfTimer()
        {
            startTime = 0;
            stopTime  = 0;
            freq = 0;

            if (QueryPerformanceFrequency(out freq) == false)
            {
                throw new Exception("high-performance counter not supported");
            }
        }

        // Start the timer
        public long Start()
        {
            QueryPerformanceCounter(out startTime);
            return startTime;
        }

        // Stop the timer
        public long Stop()
        {
            QueryPerformanceCounter(out stopTime);
            return stopTime;
        }

        // Returns the duration of the timer (in seconds)
        public double Duration
        {
            get { return (double)(stopTime - startTime) / (double)freq; }
        }


        public long Frequency 
        {
            get
            {
                QueryPerformanceFrequency(out freq);
                return freq;
            }
        }
    }
}
