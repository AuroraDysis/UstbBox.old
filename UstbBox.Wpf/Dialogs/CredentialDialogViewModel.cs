namespace UstbBox.Wpf.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using CredentialManagement;
    using Models;
    using Reactive.Bindings;
    using UstbBox.Wpf.Services;

    public class CredentialDialogViewModel
    {
        public CredentialDialogViewModel()
        {
            this.IsSaveEnabled = Observable.CombineLatest(this.Username, this.Password, this.SelectedKind, this.GetIsSaveEnabled).ToReactiveProperty();
            this.CommandSave = this.IsSaveEnabled.ToReactiveCommand();
            this.CommandSave.ObserveOn(TaskPoolScheduler.Default).Select(_ => this.GetCredential()).Subscribe(CredentialService.Instance.SaveCredential);
        }

        public CredentialDialogViewModel(string id)
            : this()
        {
            this.SelectedKind.Value = this.CredentialKinds.First(x => x.Id == id);
            var credential = CredentialService.Instance.GetCredential(id);
            if (credential != null)
            {
                this.Username.Value = credential.Username;
                this.Password.Value = credential.Password;
            }
        }

        public IReadOnlyCollection<CredentialKind> CredentialKinds { get; } = CredentialKind.AllKinds;

        public ReactiveProperty<CredentialKind> SelectedKind { get; } = new ReactiveProperty<CredentialKind>();

        public ReactiveProperty<string> Username { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> Password { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<bool> IsSaveEnabled { get; }

        public ReactiveCommand CommandSave { get; }

        public bool GetIsSaveEnabled(string username, string password, CredentialKind selectedKind) => !string.IsNullOrWhiteSpace(username) && !string.IsNullOrEmpty(password) && selectedKind != null;

        private Credential GetCredential() => new Credential(this.Username.Value, this.Password.Value, this.SelectedKind.Value.Id);
    }
}
