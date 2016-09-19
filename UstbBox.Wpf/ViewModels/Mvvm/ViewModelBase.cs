namespace UstbBox.Wpf.ViewModels.Mvvm
{
    using System;
    using System.Reactive.Disposables;
    using UstbBox.Wpf.Navigation;

    public class ViewModelBase : INavigationAware
    {
        private bool disposedValue = false;

        protected CompositeDisposable DisposableGroup { get; private set; }

        public virtual void Initialize(object parameter)
        {
            this.DisposableGroup = new CompositeDisposable();
        }

        public virtual void OnNavigatedFrom()
        {
        }

        public virtual void OnNavigatedTo(object parameter)
        {
        }

        public virtual bool Redirect(object parameter)
        {
            return false;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.DisposableGroup.Dispose();
                }

                this.disposedValue = true;
            }
        }

        protected void AddToDisposableGroup(params IDisposable[] disposables)
        {
            foreach (var item in disposables)
            {
                this.DisposableGroup.Add(item);
            }
        }
    }
}
