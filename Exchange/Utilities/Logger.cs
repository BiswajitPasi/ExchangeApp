
using Exchange.Interfaces;

namespace Exchange.Utilities
{
    public class Logger : ILogger
    {
        public void LogError(string message)
        {
            Console.WriteLine($"[ERROR] {message}");

        }
    }
}
