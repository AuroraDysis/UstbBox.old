using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UstbBox.App.Controls.Dialogs
{
    using Windows.Security.Credentials;

    using AutoMapper;

    using Microsoft.Practices.ServiceLocation;

    using Reactive.Bindings;

    using UstbBox.App.ViewModels.Models;
    using UstbBox.Models.Credentials;
    using UstbBox.Services.CredentialServices;

    public sealed partial class CredentialDialog : ContentDialog
    {
        private readonly ICredentialService credentialService = ServiceLocator.Current.GetInstance<ICredentialService>();

        private readonly IEnumerable<CredentialViewModel> credentialViewModels;

        public CredentialDialog()
        {
            this.InitializeComponent();
            this.CrendentialComboBox.ItemsSource =
                this.credentialViewModels =
                from kind in CredentialKind.AllKinds select Mapper.Map<CredentialKind, CredentialViewModel>(kind);
        }

        public CredentialDialog(Guid id) : this()
        {
            this.CrendentialComboBox.SelectedItem = this.credentialViewModels.FirstOrDefault(x => x.Kind.Id == id);
        }

        public bool CanSave(string username, string password, object selectedItem)
        {
            return !string.IsNullOrWhiteSpace(username) && !string.IsNullOrEmpty(password) && selectedItem != null;
        }

        private void SaveCredential()
        {
            var username = this.UsernameTextBox.Text;
            var password = this.PasswordBox.Password;
            var item = (CredentialViewModel)this.CrendentialComboBox.SelectedItem;
            var credential = new PasswordCredential(item.Kind.Id.ToString(), username, password);
            this.credentialService.SaveCredential(credential);
        }

        private void FillCredential()
        {
            CredentialViewModel vm = this.CrendentialComboBox.SelectedItem as CredentialViewModel;
            if (vm?.Credential != null)
            {
                this.UsernameTextBox.Text = vm.Credential.UserName;
                vm.Credential.RetrievePassword();
                this.PasswordBox.Password = vm.Credential.Password;
            }
            else
            {
                this.UsernameTextBox.Text = string.Empty;
                this.PasswordBox.Password = string.Empty;
            }
        }
    }
}
