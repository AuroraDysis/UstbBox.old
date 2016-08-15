using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.App.ViewModels
{
    using System.Reactive.Disposables;
    using Template10.Services.NavigationService;

    public class DisposableViewModelBase : Template10.Mvvm.ViewModelBase, IDisposable
    {
        protected CompositeDisposable DisposableGroup { get; } = new CompositeDisposable();

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            if (!this.DisposableGroup.IsDisposed)
            {
                this.DisposableGroup.Dispose();
            }
            return base.OnNavigatingFromAsync(args);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
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
        #endregion
    }
}
