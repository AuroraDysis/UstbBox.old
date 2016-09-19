namespace UstbBox.Wpf.Extensions
{
    using System;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Reactive.Bindings;

    public static class ObservableExtensions
    {
        private static IScheduler uiSchduler = UIDispatcherScheduler.Default;

        private static IScheduler taskPoolSchduler = TaskPoolScheduler.Default;

        public static IObservable<T> DoOnUI<T>(this IObservable<T> source, Action<T> action)
        {
            return source.ObserveOn(uiSchduler).Do(action);
        }

        public static IObservable<T> DoOnBackground<T>(this IObservable<T> source, Action<T> action)
        {
            return source.ObserveOn(taskPoolSchduler).Do(action);
        }

        public static IDisposable ObserveOnUIAndSubscribe<T>(this IObservable<T> source, Action<T> action)
        {
            return source.ObserveOn(uiSchduler).Subscribe(action);
        }

        public static IDisposable ObserveOnBackgroundAndSubscribe<T>(this IObservable<T> source, Action<T> action)
        {
            return source.ObserveOn(taskPoolSchduler).Subscribe(action);
        }

        public static IObservable<T> PermaRef<T>(this IConnectableObservable<T> @this)
        {
            @this.Connect();
            return @this;
        }

        public static IObservable<Unit> IgnoreResult<T>(this IObservable<T> @this)
        {
            return @this.Select(_ => Unit.Default);
        }
    }
}
