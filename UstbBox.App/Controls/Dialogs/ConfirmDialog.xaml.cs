namespace UstbBox.App.Controls.Dialogs
{
    using Windows.UI.Xaml.Controls;

    public sealed partial class ConfirmDialog : ContentDialog
    {
        public ConfirmDialog(string content, string title = null)
        {
            this.InitializeComponent();
            this.Title = string.IsNullOrWhiteSpace(title) ? "操作确认" : title;
            this.Content = content ?? string.Empty;
        }
    }
}
