using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.App
{
    using Windows.Security.Credentials;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    using Template10.Utils;

    using UstbBox.App.ViewModels.Models;
    using UstbBox.Models.Credentials;
    using UstbBox.Services.CredentialServices;
    using UstbBox.Services.EducationSystemServices;
    using UstbBox.Services.ImageServices;
    using UstbBox.Services;

    public class Startups
    {
        public void Run()
        {
            Reactive.Bindings.UIDispatcherScheduler.Initialize();
            this.InitializeNavigationKeys();
            this.RegisterTypes();
            new ServicesBootstrapper().Run();
            this.ConfigAutoMapper();
        }

        private void InitializeNavigationKeys()
        {
            var keys = Template10.Common.BootStrapper.Current.PageKeys<Pages>();
            keys.TryAdd(Pages.MainPage, typeof(Views.MainPage));
            keys.TryAdd(Pages.CredentialPage, typeof(Views.Settings.CredentialPage));
            keys.TryAdd(Pages.SettingsPage, typeof(Views.Settings.SettingsPage));
        }

        private void RegisterTypes()
        {
            var container = new UnityContainer();
            container.RegisterType<IImageService, ImageService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICredentialService, CredentialService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IEducationSystemService, EducationSystemService>(
                new ContainerControlledLifetimeManager());
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
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
    }
}
