using System.ComponentModel;
using System.Linq;
using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UstbBox.App.Views
{
    public sealed partial class Shell : Page
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.myHamburgerMenu;

        public Shell()
        {
            Instance = this;
            this.InitializeComponent();
        }

        public Shell(INavigationService navigationService) : this()
        {
            this.SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            this.myHamburgerMenu.NavigationService = navigationService;
        }
    }
}

