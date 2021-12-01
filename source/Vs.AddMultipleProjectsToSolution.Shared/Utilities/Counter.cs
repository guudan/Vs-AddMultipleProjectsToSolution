using System.Threading;

namespace Vs.AddMultipleProjectsToSolution.Utilities
{
    public class Counter
    {
        private volatile int _Value;

        public int Value => _Value;

        public int Add(int value)
        {
#pragma warning disable 420
            return Interlocked.Add(ref _Value, value);
#pragma warning restore 420
        }
    }
}