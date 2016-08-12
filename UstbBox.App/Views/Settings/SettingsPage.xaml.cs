using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UstbBox.App.Views.Settings
{
    using Template10.Services.SerializationService;

    public sealed partial class SettingsPage : Page
    {
        private readonly ISerializationService serializationService;

        public SettingsPage()
        {
            this.InitializeComponent(); 
            this.serializationService = SerializationService.Json;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var index = int.Parse(this.serializationService.Deserialize(e.Parameter?.ToString()).ToString());
            this.myPivot.SelectedIndex = index;
        }
    }
}
