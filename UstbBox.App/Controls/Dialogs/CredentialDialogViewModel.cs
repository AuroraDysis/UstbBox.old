using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.App.Controls.Dialogs
{
    using System.Reactive.Linq;

    using Windows.Security.Credentials;

    using Microsoft.Practices.ServiceLocation;

    using Reactive.Bindings;

    using UstbBox.App.ViewModels.Models;
    using UstbBox.Models.Credentials;
    using UstbBox.Services.CredentialServices;

    public class CredentialDialogViewModel
    {
        public CredentialDialogViewModel()
        {
            this.Username = new ReactiveProperty<string>(string.Empty);
            this.Password = new ReactiveProperty<string>(string.Empty);
            this.CredentialViewModels =
                CredentialKind.AllKinds.Select(AutoMapper.Mapper.Map<CredentialKind, CredentialViewModel>).ToList();
            this.SelectedItem = new ReactiveProperty<CredentialViewModel>();
            this.CanSaveExcute = this.Username.Select(x => !string.IsNullOrWhiteSpace(x))
                .CombineLatest(this.Password.Select(x => !string.IsNullOrEmpty(x)), this.SelectedItem.Select(x => x != null), (x, y, z) => x && y && z)
                .ToReactiveProperty();
            this.SelectedItem.Where(x => x?.Credential != null).Subscribe(
                vm =>
                    {
                        this.Username.Value = vm.Credential.UserName;
                        vm.Credential.RetrievePassword();
                        this.Password.Value = vm.Credential.Password;
                    });
            this.CommandSave = new ReactiveCommand();
            this.CommandSave.Subscribe(_ => this.SaveCredential());
        }

        public ReactiveProperty<string> Username { get; set; }

        public ReactiveProperty<string> Password { get; set; }

        public ReactiveProperty<bool> CanSaveExcute { get; set; }

        public List<CredentialViewModel> CredentialViewModels { get; set; }

        public ReactiveProperty<CredentialViewModel> SelectedItem { get; set; }

        public ReactiveCommand CommandSave { get; set; }

        public ICredentialService CredentialService { get; set; } =
            ServiceLocator.Current.GetInstance<ICredentialService>();

        private void SaveCredential()
        {
            var username = this.Username.Value;
            var password = this.Password.Value;
            var item = this.SelectedItem.Value;
            var credential = new PasswordCredential(item.Kind.Id.ToString(), username, password);
            this.CredentialService.SaveCredential(credential);
        }
    }
}
