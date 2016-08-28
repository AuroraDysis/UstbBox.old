using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace UstbBox.App.ViewModels.Generals
{
    using System.Collections.Specialized;
    using System.Reactive.Linq;

    using Windows.UI.Xaml.Controls;

    using Microsoft.Practices.ServiceLocation;

    using Reactive.Bindings;
    using Reactive.Bindings.Extensions;
    using UstbBox.Models.Teach;
    using UstbBox.Services.TeachServices;

    public class LatestNewsPageViewModel : DisposableViewModelBase
    {
        public LatestNewsPageViewModel()
        {
            this.CommandRefresh = new ReactiveCommand().AddTo(this.DisposableGroup);
            this.CommandRefresh.Subscribe(_ => this.GetLatestNews());
            this.GetLatestNews();
        }

        public ITeachService TeachService { get; set; } = ServiceLocator.Current.GetInstance<ITeachService>();

        public ReactiveCommand CommandRefresh { get; set; }

        List<TeachNewsItem> latestNews = default(List<TeachNewsItem>);
        public List<TeachNewsItem> LatestNews { get { return this.latestNews; } set { Set(ref this.latestNews, value); } }

        public void OpenNewsLink(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as TeachNewsItem;
            if (item == null)
            {
                return;
            }
            this.NavigationService.Navigate(Pages.WebPage, item);
        }

        private void GetLatestNews()
        {
            this.TeachService.GetLatestNews().ToList().Subscribe(list => this.LatestNews = list.ToList()).AddTo(this.DisposableGroup); ;
        }
    }
}
