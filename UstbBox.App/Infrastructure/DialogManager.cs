using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.App
{
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Reactive.Windows.Foundation;
    using System.Threading;

    using Windows.UI.Xaml.Controls;

    using Reactive.Bindings;

    using UstbBox.Core.Extensions;

    using UstbBox.App.Controls.Dialogs;

    public static class DialogManager
    {
        // ReSharper disable once InconsistentNaming
        private static readonly IScheduler UIScheduler = UIDispatcherScheduler.Default;

        private static readonly object AsyncLock = new object();

        public static IObservable<Unit> ShowMessage(string content, string title = null)
        {
            return ShowContentDialog<Unit>(() => new MessageDialog(content, title), command => Unit.Default);
        }

        public static IObservable<bool> ShowConfirm(string content, string title = null)
        {
            return ShowContentDialog<bool>(
                () => new ConfirmDialog(content, title),
                result => result == ContentDialogResult.Primary);
        }

        private static IObservable<T> ShowContentDialog<T>(
            Func<ContentDialog> dialogFactory,
            Func<ContentDialogResult, T> resultMapper)
        {
            var result = new AsyncSubject<T>();
            TaskPoolScheduler.Default.Schedule(
                () =>
                    {
                        lock (AsyncLock)
                        {
                            UIScheduler.Schedule(
                                () => dialogFactory().ShowAsync().ToObservable().Select(resultMapper).Subscribe(result));
                            result.Wait();
                        }
                    });
            return result;
        }
    }
}
