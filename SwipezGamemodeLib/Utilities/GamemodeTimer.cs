using System.Diagnostics;

namespace SwipezGamemodeLib.Utilities
{
    public class GamemodeTimer
    {
        private Stopwatch internalStopwatch;
        public bool isRunning;
        
        public void Start()
        {
            isRunning = true;
            internalStopwatch = Stopwatch.StartNew();
        }
        
        public void Reset()
        {
            if (internalStopwatch == null)
            {
                return;
            }

            isRunning = false;
            internalStopwatch.Reset();
        }

        public bool IsFinishedInMinutes(int minutes)
        {
            if (internalStopwatch == null)
            {
                return false;
            }

            return internalStopwatch.ElapsedMilliseconds >= minutes * 60 * 1000;
        }
        public bool IsFinishedInSeconds(int seconds)
        {
            if (internalStopwatch == null)
            {
                return false;
            }
            return internalStopwatch.ElapsedMilliseconds >= seconds * 1000;
        }

        public string ConvertToReadableTime()
        {
            // Convert stopwatch ms to 0:00 format
            return internalStopwatch.Elapsed.ToString(@"m\:ss");
        }
    }
}