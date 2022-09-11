namespace Iterum.BaseSystems
{
    public class NetworkStats
    {
        public int ReceivedPacketsTotal;
        public int ReceivedBytesTotal;
        
        public int SentPacketsTotal;
        public int SentBytesTotal;
        
        public int SentPacketsPerSecond;
        public int SentBytesPerSecond;
        
        public int ReceivedPacketsPerSecond;
        public int ReceivedBytesPerSecond;
        
        public double RTT;
        
        public double Ping => RTT / 2;
    }
}