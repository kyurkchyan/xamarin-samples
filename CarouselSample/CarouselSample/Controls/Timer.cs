using System;
using System.Threading;
using System.Threading.Tasks;

namespace CarouselSample.Controls
{
    public delegate void TimerCallback(object state);

    public sealed class Timer : CancellationTokenSource, IDisposable
    {
        public Timer(TimerCallback callback, object state, int dueTimeInMilliseconds)
        {
            Task.Delay(dueTimeInMilliseconds, Token).ContinueWith((t, s) =>
            {
                var tuple = (Tuple<TimerCallback, object>) s;
                tuple.Item1(tuple.Item2);
            }, Tuple.Create(callback, state), CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.Default);
        }

        public new void Dispose()
        {
            Cancel();
        }
    }
}