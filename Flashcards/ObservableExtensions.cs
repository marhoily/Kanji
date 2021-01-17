using System;
using System.Reactive.Linq;

namespace WpfApp1
{
    public static class ObservableExtensions
    {
        internal static IObservable<T> ThrottleFirst<T>(this IObservable<T> source, TimeSpan delay, System.Reactive.Concurrency.IScheduler scheduler)
        {
            return source.Publish(o => o.Take(1)
                .Concat(o.IgnoreElements().TakeUntil(Observable.Return(default(T)).Delay(delay, scheduler)))
                .Repeat()
                .TakeUntil(o.IgnoreElements().Concat(Observable.Return(default(T)))));
        }
    }

}
