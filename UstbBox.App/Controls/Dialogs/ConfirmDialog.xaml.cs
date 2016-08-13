namespace UstbBox.App.Controls.Dialogs
{
    using Windows.ApplicationModel.DataTransfer;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class ConfirmDialog : ContentDialog
    {
        public ConfirmDialog(string content, string title = null)
        {
            this.InitializeComponent();
            this.Title = string.IsNullOrWhiteSpace(title) ? "操作确认" : title;
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
