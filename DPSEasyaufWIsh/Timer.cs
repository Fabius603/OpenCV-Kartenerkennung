using System.Diagnostics;

namespace DPSEasyaufWish
{
    public class Timer
    {
        public static void Start(out Stopwatch stopwatch)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public static string Stop(Stopwatch stopwatch)
        {
            stopwatch.Stop();
            string time = stopwatch.Elapsed.ToString().Substring(7, 6);
            return time;
        }
    }
}
