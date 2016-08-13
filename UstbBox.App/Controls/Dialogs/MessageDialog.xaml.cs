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

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UstbBox.App.Controls.Dialogs
{
    public sealed partial class MessageDialog : ContentDialog
    {
        public MessageDialog(string content, string title = null)
        {
            this.InitializeComponent();
            this.Title = string.IsNullOrWhiteSpace(title) ? "提示" : title;
            this.Content = content ?? string.Empty;
        }
    }
}
