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
    using Reactive.Bindings;

    using UstbBox.App.ViewModels.Models;
    using UstbBox.Models.Credentials;
    using UstbBox.Services.CredentialServices;

    public sealed partial class CredentialDialog : ContentDialog
    {
        public CredentialDialog()
        {
            this.InitializeComponent();
        }

        public CredentialDialog(Guid id)
        {
            this.InitializeComponent();
            var vm = (CredentialDialogViewModel)this.DataContext;
            vm.SelectedItem.Value = vm.CredentialViewModels.FirstOrDefault(x => x.Kind.Id == id);
        }
    }
}
