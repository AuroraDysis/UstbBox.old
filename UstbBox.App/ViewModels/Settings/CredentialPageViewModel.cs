using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.App.ViewModels.Settings
{
    using System.Reactive.Linq;

    using Windows.ApplicationModel;
    using Windows.Security.Credentials;
    using Windows.UI.Xaml.Controls;

    using Microsoft.Practices.ServiceLocation;

    using Reactive.Bindings;
    using Reactive.Bindings.Extensions;

    using Template10.Utils;

    using UstbBox.App.ViewModels.Models;
    using UstbBox.Services.CredentialServices;

    public class CredentialPageViewModel : DisposableViewModelBase
    {
        public CredentialPageViewModel()
        {
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            this.Credentials = new ReactiveCollection<CredentialViewModel>().AddTo(this.DisposableGroup);
            this.Refresh();

            this.CommandAdd = new ReactiveCommand().AddTo(this.DisposableGroup).AddTo(this.DisposableGroup);
            this.CommandAdd.Subscribe(_ => this.Add());

            this.CommandRefresh = new ReactiveCommand().AddTo(this.DisposableGroup);
            this.CommandRefresh.Subscribe(_ => this.Refresh());

            this.CommandEdit = new ReactiveCommand<IList<object>>().AddTo(this.DisposableGroup);
            this.CommandEdit.Subscribe(list =>
            {
                var vm = list.Cast<CredentialViewModel>().FirstOrDefault();
                if (vm != null)
                {
                    DialogManager.ShowCredential(vm.Kind.Id).Subscribe(_ => this.Refresh());
                }
            });

            this.CommandDelete = new ReactiveCommand<IList<object>>().AddTo(this.DisposableGroup);
            this.CommandDelete.Subscribe(
                list =>
                    {
                        list.Cast<CredentialViewModel>()
                            .ForEach(x => this.CredentialService.RemoveCredential(x.Credential));
                        this.Refresh();
                    });

            this.SelectionMode = new ReactiveProperty<ListViewSelectionMode>(ListViewSelectionMode.Single).AddTo(this.DisposableGroup);
        }

        public ReactiveCommand CommandAdd { get; }

        public ReactiveCommand CommandRefresh { get; }

        public ReactiveCommand<IList<object>> CommandEdit { get; }

        public ReactiveCommand<IList<object>> CommandDelete { get; }

        public ReactiveProperty<ListViewSelectionMode> SelectionMode { get; set; }

        public ReactiveCollection<CredentialViewModel> Credentials { get; }

        public ICredentialService CredentialService { get; set; } =
            ServiceLocator.Current.GetInstance<ICredentialService>();

        public void ChangeSelectionMode()
        {
            this.SelectionMode.Value = this.SelectionMode.Value == ListViewSelectionMode.Single
                ? ListViewSelectionMode.Multiple
                : ListViewSelectionMode.Single;
        }

        private void Add()
        {
            DialogManager.ShowCredential().Subscribe(_ => this.Refresh());
        }

        private void Refresh()
        {
            this.Credentials.ClearOnScheduler();
            var credentials =
                this.CredentialService.RetrieveAll()
                    .Select(AutoMapper.Mapper.Map<PasswordCredential, CredentialViewModel>);
            this.Credentials.AddRangeOnScheduler(credentials);
        }
    }
}
