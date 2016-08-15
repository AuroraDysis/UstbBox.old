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

namespace UstbBox.App.Views.Generals
{
    using Template10.Controls;
    using Template10.Services.NavigationService;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GeneralsNavigationPage : Page
    {
        public GeneralsNavigationPage()
        {
            this.InitializeComponent();
        }

        private void ListViewItemClick(object sender, ItemClickEventArgs e)
        {
            var info = e.ClickedItem as HamburgerButtonInfo;
            if (info != null)
            {
                NavigationService.GetForFrame(this.Frame).Navigate(info.PageType, info.PageParameter);
            }
        }
    }
}
