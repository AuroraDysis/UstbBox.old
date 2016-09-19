namespace UstbBox.Wpf
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Markup;
    using MaterialDesignThemes.Wpf;
    using Reactive.Bindings;
    using UstbBox.Wpf.Controllers;
    using UstbBox.Wpf.Services;
    using UstbBox.Wpf.ViewModels.Generals;

    public class Bootstrapper
    {
        public static void Run(StartupEventArgs startupEventArgs)
        {
            UIDispatcherScheduler.Initialize();

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            CommandManager.RegisterClassCommandBinding(typeof(Shell), new CommandBinding(NavigationCommands.BrowseBack, (sender, args) => NavigationService.Instance.GoBack()));
            CommandManager.RegisterClassCommandBinding(typeof(Shell), new CommandBinding(NavigationCommands.BrowseForward, (sender, args) => NavigationService.Instance.GoForward()));

            LoadVisualSettings();

            NavigationService.Instance.NavigateTo<HomeViewModel>();
        }

        public static void LoadVisualSettings()
        {
            var helper = new PaletteHelper();
            var service = SettingService.Instance;
            helper.ReplaceAccentColor(service.AccentColor);
            helper.ReplacePrimaryColor(service.PrimaryColor);
            helper.SetLightDark(service.IsDark);
        }
    }
}
