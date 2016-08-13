using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Core.Extensions
{
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    public static class ReactiveExtensionsMixin
    {
        public static IObservable<Unit> ToDefaultUnit<T>(this IObservable<T> observable)
        {
            return observable.Select(_ => Unit.Default);
        }

        public static IObservable<T> PermaRef<T>(this IConnectableObservable<T> connectable)
        {
            connectable.Connect();
            return connectable;
        }
    }
}
