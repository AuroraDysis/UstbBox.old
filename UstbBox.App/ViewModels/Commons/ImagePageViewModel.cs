﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.App.ViewModels.Commons
{
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Animation;

    using Reactive.Bindings;

    using UstbBox.Models.Images;
    using Windows.UI.Xaml.Navigation;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Toolkit.Uwp.UI;

    using UstbBox.Services.ImageServices;

    public class ImagePageViewModel : DisposableViewModelBase
    {
        public ImagePageViewModel()
        {
            this.Image = new ReactiveProperty<ImageObject>();
            this.CommandOpen = new ReactiveCommand();
            this.CommandOpen.Subscribe(
                async _ =>
                    {
                        var filename = ImageCache.GetCacheFileName(new Uri(this.Image.Value.Uri));
                        var storageFile =
                            await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(filename));
                        await Windows.System.Launcher.LaunchFileAsync(storageFile);
                    });
        }

        public ReactiveProperty<ImageObject> Image { get; set; }

        public ReactiveCommand CommandOpen { get; set; }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await base.OnNavigatedToAsync(parameter, mode, state).ConfigureAwait(false);
            if (parameter is string)
            {
                var str = (string)parameter;
                switch (str)
                {
                    case "CampusMap":
                        var service = ServiceLocator.Current.GetInstance<IImageService>();
                        this.Image.Value = service.GetCampusMap();
                        break;
                }
            }
            else if (parameter is ImageObject)
            {
                this.Image.Value = (ImageObject)parameter;
            }
        }
    }
}
