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
    using Windows.ApplicationModel.DataTransfer;

    public sealed partial class MessageDialog : ContentDialog
    {
        public MessageDialog(string content, string title = null)
        {
            this.InitializeComponent();
            this.Title = string.IsNullOrWhiteSpace(title) ? "提示" : title;
            this.ContentTextBlock.Text = content ?? string.Empty;
        }

        private void CopyTapped(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(this.ContentTextBlock.Text);
            Clipboard.SetContent(dataPackage);
        }
    }
}
