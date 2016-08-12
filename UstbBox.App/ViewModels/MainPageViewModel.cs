using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Template10.Mvvm;
using Template10.Services.NavigationService;

using Windows.UI.Xaml.Navigation;

namespace UstbBox.App.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.Value = "Designtime value";
            }
        }

        string value = "Gas";
        public string Value { get { return this.value; } set {
            this.Set(ref this.value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                this.Value = suspensionState[nameof(this.Value)]?.ToString();
            }

            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(this.Value)] = this.Value;
            }

            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void GotoDetailsPage() => this.NavigationService.Navigate(typeof(Views.DetailPage), this.Value);

        public void GotoSettings() => this.NavigationService.Navigate(typeof(Views.Settings.SettingsPage), 0);

        public void GotoPrivacy() => this.NavigationService.Navigate(typeof(Views.Settings.SettingsPage), 1);

        public void GotoAbout() => this.NavigationService.Navigate(typeof(Views.Settings.SettingsPage), 2);

    }
}

