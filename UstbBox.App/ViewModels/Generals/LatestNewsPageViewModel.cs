using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace UstbBox.App.ViewModels.Generals
{
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
            this.CommandRefresh.Subscribe(
                _ =>
                    {
                        var news = this.LatestNews;
                        this.LatestNews = null;
                        news?.Dispose();
                        this.LatestNews = this.TeachService.GetLatestNews().ToReadOnlyReactiveCollection().AddTo(this.DisposableGroup);
                    });
            this.LatestNews =
                this.TeachService.GetLatestNews().ToReadOnlyReactiveCollection().AddTo(this.DisposableGroup);
        }

        public ITeachService TeachService { get; set; } = ServiceLocator.Current.GetInstance<ITeachService>();

        public ReactiveCommand CommandRefresh { get; set; }

        private ReadOnlyReactiveCollection<TeachNewsItem> latestNews = default(ReadOnlyReactiveCollection<TeachNewsItem>);

        public ReadOnlyReactiveCollection<TeachNewsItem> LatestNews
        {
            get
            {
                return this.latestNews;
            }

            set
            {
                this.Set(ref this.latestNews, value);
            }
        }

        public void OpenNewsLink(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as TeachNewsItem;
            if (item == null)
            {
                return;
            }
            
        }
    }
}
