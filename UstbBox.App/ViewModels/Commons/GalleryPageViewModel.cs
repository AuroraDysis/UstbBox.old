using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace UstbBox.App.ViewModels.Commons
{
    using System.Reactive.Linq;

    using Microsoft.Practices.ServiceLocation;

    using Reactive.Bindings;

    using UstbBox.Models.Images;
    using UstbBox.Services.ImageServices;

    public class GalleryPageViewModel : DisposableViewModelBase
    {
        public GalleryPageViewModel()
        {
            this.GalleryName = new ReactiveProperty<string>();
            this.ImageCollection = new ReactiveProperty<List<ImageObject>>();
        }

        public ReactiveProperty<string> GalleryName { get; set; }

        public ReactiveProperty<List<ImageObject>> ImageCollection { get; set; }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await base.OnNavigatedToAsync(parameter, mode, state).ConfigureAwait(false);
            if (parameter is string)
            {
                var str = (string)parameter;
                switch (str)
                {
                    case "SchoolCalendars":
                        this.GalleryName.Value = "校历";
                        var service = ServiceLocator.Current.GetInstance<IImageService>();
                        Views.Busy.SetBusy(true, "Downloading...");
                        service.GetSchoolCalendars()
                            .ToList()
                            .Subscribe(
                                x => this.ImageCollection.Value = x.OrderByDescending(s => s.Name).ToList(),
                                ex => Views.Busy.SetBusy(false),
                                () => Views.Busy.SetBusy(false));
                        break;
                }
            }
        }
    }
}
