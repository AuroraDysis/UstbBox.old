using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace UstbBox.App.ViewModels.Commons
{
    using Reactive.Bindings;

    using UstbBox.Models.Teach;

    public class WebPageViewModel : DisposableViewModelBase
    {
        public WebPageViewModel()
        {
            this.Title = new ReactiveProperty<string>();
            this.Uri = new ReactiveProperty<Uri>();
        }

        public ReactiveProperty<string> Title { get; set; }

        public ReactiveProperty<Uri> Uri { get; set; }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await base.OnNavigatedToAsync(parameter, mode, state);
            var item = parameter as TeachNewsItem;
            if (item != null)
            {
                this.Title.Value = item.Name;
                this.Uri.Value = new Uri(item.Link);
            }
        }
    }
}
