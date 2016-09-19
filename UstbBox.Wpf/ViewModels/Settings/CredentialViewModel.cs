namespace UstbBox.Wpf.ViewModels.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using CredentialManagement;
    using Models;
    using Reactive.Bindings;
    using UstbBox.Wpf.Controllers;
    using UstbBox.Wpf.Services;
    using UstbBox.Wpf.ViewModels.Mvvm;

    public class CredentialViewModel : ViewModelBase
    {
        public ReactiveProperty<List<CredentialVm>> Credentials { get; set; }

        public ReactiveCommand CommandRefresh { get; set; }

        public ReactiveCommand CommandAdd { get; set; }

        public ReactiveCommand<CredentialVm> CommandEdit { get; set; }

        public ReactiveCommand<CredentialVm> CommandDelete { get; set; }

        public override void Initialize(object parameter)
        {
            base.Initialize(parameter);

            this.Credentials = new ReactiveProperty<List<CredentialVm>>();

            this.CommandRefresh = new ReactiveCommand();
            this.CommandRefresh.Subscribe(_ => this.RefreshData());

            this.CommandAdd = new ReactiveCommand();
            this.CommandAdd.Subscribe(_ => DialogService.Instance.ShowCredentialDialog().Subscribe(x => this.RefreshData()));

            this.CommandEdit = new ReactiveCommand<CredentialVm>();
            this.CommandEdit.Subscribe(vm => DialogService.Instance.ShowCredentialDialog(vm.Kind.Id).Subscribe(_ => this.RefreshData()));

            this.CommandDelete = new ReactiveCommand<CredentialVm>();
            this.CommandDelete.Subscribe(vm =>
            {
                CredentialService.Instance.DeleteCredential(vm.Kind.Id);
                this.RefreshData();
            });
        }

        public override void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            this.RefreshData();
        }

        private void RefreshData()
        {
            this.Credentials.Value = CredentialService.Instance.RetrieveAll().Select(x => new CredentialVm(x)).ToList();
        }

        public class CredentialVm
        {
            public CredentialVm(Credential credential)
            {
                this.Username = credential.Username;
                this.Kind = CredentialKind.GetKindById(credential.Target);
                credential.Load();
                this.PasswordLength = credential.SecurePassword.Length;
                this.Websites = string.Join("\n", this.Kind.Websites);
                this.LastWriteTime = credential.LastWriteTime;
            }

            public string Username { get; set; }

            public CredentialKind Kind { get; set; }

            public int PasswordLength { get; set; }

            public string Websites { get; set; }

            public DateTime LastWriteTime { get; set; }
        }
    }
}
