using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.App.ViewModels.Settings
{
    using Windows.ApplicationModel;
    using Windows.Security.Credentials;

    using Reactive.Bindings;
    using Reactive.Bindings.Extensions;

    using UstbBox.App.ViewModels.Models;

    public class CredentialPageViewModel : DisposableViewModelBase
    {
        private readonly PasswordVault passwordValut = new PasswordVault();

        public CredentialPageViewModel()
        {
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            this.Credentials = new ReactiveCollection<CredentialViewModel>();

            this.CommandAdd = new ReactiveCommand().AddTo(this.DisposableGroup);
            this.CommandAdd.Subscribe(_ => this.Add());

            this.CommandRefresh = new ReactiveCommand().AddTo(this.DisposableGroup);
            this.CommandRefresh.Subscribe(_ => this.Refresh());
        }

        public ReactiveCommand CommandAdd { get; }

        public ReactiveCommand CommandRefresh { get; }

        public ReactiveCollection<CredentialViewModel> Credentials { get; }

        private void Add()
        {
        }

        private void Refresh()
        {
            this.Credentials.ClearOnScheduler();
            var credentials =
                this.passwordValut.RetrieveAll()
                    .Select(AutoMapper.Mapper.Map<PasswordCredential, CredentialViewModel>);
            this.Credentials.AddRangeOnScheduler(credentials);
        }
    }
}
