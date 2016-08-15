using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.App.ViewModels.Commons
{
    using Reactive.Bindings;

    using UstbBox.Models.Images;
    using Windows.UI.Xaml.Navigation;

    using Microsoft.Practices.ServiceLocation;

    using UstbBox.Services.ImageServices;

    public class ImagePageViewModel : DisposableViewModelBase
    {
        public ImagePageViewModel()
        {
            this.Image = new ReactiveProperty<ImageObject>();
        }

        public ReactiveProperty<ImageObject> Image { get; set; }

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
                        Views.Busy.SetBusy(true, "Downloading...");
                        service.GetCampusMap().Subscribe(
                            image =>
                                {
                                    Views.Busy.SetBusy(false);
                                    image.Name = "校园地图";
                                    this.Image.Value = image;
                                },
                            ex =>
                                {
                                    Views.Busy.SetBusy(false);
                                });
                        break;
                }
            }
        }
    }
}
