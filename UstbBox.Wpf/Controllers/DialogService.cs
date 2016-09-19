namespace UstbBox.Wpf.Controllers
{
    using System;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Threading;
    using MaterialDesignThemes.Wpf;
    using Reactive.Bindings;
    using UstbBox.Wpf.Dialogs;
    using UstbBox.Wpf.Extensions;

    public class DialogService
    {
        private readonly string identifier;
        private readonly IScheduler uiScheduler = UIDispatcherScheduler.Default;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

        public DialogService(string hostIdentifier)
        {
            this.identifier = hostIdentifier;
        }

        public static DialogService Instance { get; } = new DialogService("DefaultHost");

        public IObservable<Unit> ShowCredentialDialog()
        {
            return this.ShowDialog(() => new CredentialDialog() { DataContext = new CredentialDialogViewModel() }).IgnoreResult();
        }

        public IObservable<Unit> ShowCredentialDialog(string id)
        {
            return this.ShowDialog(() => new CredentialDialog() { DataContext = new CredentialDialogViewModel(id) }).IgnoreResult();
        }

        private IObservable<object> ShowDialog(Func<object> dialogFactory)
        {
            return this.WaitForSemaphore()
                .Select(_ => dialogFactory())
                .SelectMany(dialog => DialogHost.Show(dialog, this.identifier))
                .Do(x => this.semaphore.Release())
                .PublishLast()
                .PermaRef();
        }

        private IObservable<Unit> WaitForSemaphore()
        {
            return this.semaphore.WaitAsync().ToObservable().ObserveOn(this.uiScheduler);
        }
    }
}
