namespace UstbBox.App
{
    using System.Threading.Tasks;

    using Template10.Common;
    using Template10.Controls;

    using UstbBox.App.Services.SettingsServices;

    using Windows.ApplicationModel.Activation;
    using Windows.Security.Credentials;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    using Template10.Utils;

    using UstbBox.App.ViewModels.Models;
    using UstbBox.Models.Credentials;
    using UstbBox.Services.CredentialServices;
    using UstbBox.Services.EducationSystemServices;

    [Bindable]
    public sealed partial class App : BootStrapper
    {
        public App()
        {
            this.InitializeComponent();

            this.SplashFactory = (e) => new Views.Splash(e);

            var settings = SettingsService.Instance;

            this.RequestedTheme = settings.AppTheme;
            this.CacheMaxDuration = settings.CacheMaxDuration;
            this.ShowShellBackButton = settings.UseShellBackButton;
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            this.InitializeNavigationKeys();
            this.RegisterTypes();
            this.ConfigAutoMapper();
            Reactive.Bindings.UIDispatcherScheduler.Initialize();
            UsualCommands.Initialize();

            // ReSharper disable once TryCastAndCheckForNull.0
            if (Window.Current.Content as ModalDialog == null)
            {
                // create a new frame 
                var nav = this.NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                // create modal root
                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true, 
                    Content = new Views.Shell(nav), 
                    ModalContent = new Views.Busy(), 
                };
            }

            await Task.CompletedTask;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // long-running startup tasks go here
            this.NavigationService.Navigate(Pages.MainPage);
            await Task.CompletedTask;
        }

        private void InitializeNavigationKeys()
        {
            var keys = this.PageKeys<Pages>();
            keys.TryAdd(Pages.MainPage, typeof(Views.MainPage));
            keys.TryAdd(Pages.CredentialPage, typeof(Views.Settings.CredentialPage));
            keys.TryAdd(Pages.SettingsPage, typeof(Views.Settings.SettingsPage));
        }

        private void ConfigAutoMapper()
        {
            AutoMapper.Mapper.Initialize(
                cfg =>
                    {
                        cfg.CreateMap<PasswordCredential, CredentialViewModel>()
                            .ForMember(vm => vm.Kind, x => x.MapFrom(c => CredentialKind.Get(c.Resource)))
                            .ForMember(vm => vm.Credential, x => x.MapFrom(c => c));
                        cfg.CreateMap<CredentialKind, CredentialViewModel>()
                            .ForMember(vm => vm.Kind, x => x.MapFrom(c => c))
                            .ForMember(
                                vm => vm.Credential,
                                x =>
                                x.MapFrom(
                                    c =>
                                    ServiceLocator.Current.GetInstance<ICredentialService>()
                                        .GetCredential(c.Id.ToString())))
                            .ForMember(
                                vm => vm.Prompt,
                                x =>
                                x.MapFrom(
                                    c =>
                                    "使用本账号的网站有\n" + string.Join("\n", c.Websites)
                                    + (string.IsNullOrWhiteSpace(c.DefaultPasswordInfomation)
                                           ? ""
                                           : $"\n{c.DefaultPasswordInfomation}")));
                    });
        }

        private void RegisterTypes()
        {
            var container = new UnityContainer();
            container.RegisterType<ICredentialService, CredentialService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IEducationSystemService, EducationSystemService>(
                new ContainerControlledLifetimeManager());
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
        }
    }
}

