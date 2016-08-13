using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.SettingsService;
using Windows.UI.Xaml;

namespace UstbBox.App.ViewModels.Settings
{
    using UstbBox.App.Services.SettingsServices;

    public class SettingsPageViewModel : DisposableViewModelBase
    {
        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();
    }

    public class SettingsPartViewModel : DisposableViewModelBase
    {
        SettingsService settings;

        public SettingsPartViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
            }
            else
            {
                this.settings = Services.SettingsServices.SettingsService.Instance;
            }
        }

        public bool UseShellBackButton
        {
            get { return this.settings.UseShellBackButton; }
            set {
                this.settings.UseShellBackButton = value; this.RaisePropertyChanged(); }
        }

        public bool UseLightThemeButton
        {
            get { return this.settings.AppTheme.Equals(ApplicationTheme.Light); }
            set {
                this.settings.AppTheme = value ? ApplicationTheme.Light : ApplicationTheme.Dark; this.RaisePropertyChanged(); }
        }

        private string busyText = "Please wait...";
        public string BusyText
        {
            get { return this.busyText; }
            set
            {
                this.Set(ref this.busyText, value);
                this.showBusyCommand.RaiseCanExecuteChanged();
            }
        }

        DelegateCommand showBusyCommand;
        public DelegateCommand ShowBusyCommand
            => this.showBusyCommand ?? (this.showBusyCommand = new DelegateCommand(async () =>
            {
                Views.Busy.SetBusy(true, this.busyText);
                await Task.Delay(5000);
                Views.Busy.SetBusy(false);
            }, () => !string.IsNullOrEmpty(this.BusyText)));
    }

    public class AboutPartViewModel : DisposableViewModelBase
    {
        public Uri Logo => Windows.ApplicationModel.Package.Current.Logo;

        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        public string Version
        {
            get
            {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        public Uri RateMe => new Uri("http://aka.ms/template10");
    }
}

