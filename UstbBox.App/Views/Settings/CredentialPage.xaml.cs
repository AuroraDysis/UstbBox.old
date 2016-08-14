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

namespace UstbBox.App.Views.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CredentialPage : Page
    {
        public CredentialPage()
        {
            this.InitializeComponent();
        }

        private void ListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.DeleteButton1.IsEnabled = this.ListView.SelectedItems.Count > 0;
            this.EditButton1.IsEnabled = this.ListView.SelectedItems.Count == 1;

            this.DeleteButton2.IsEnabled = this.ListView.SelectedItems.Count > 0;
            this.EditButton2.IsEnabled = this.ListView.SelectedItems.Count == 1;
        }
    }
}
