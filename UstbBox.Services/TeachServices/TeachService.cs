using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services.TeachServices
{
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;

    using UstbBox.Models.Teach;

    public class TeachService : ITeachService
    {
        private readonly TeachHelper helper = new TeachHelper();

        public IObservable<TeachNewsItem> GetLatestNews()
        {
            var db = AppDatabase.CommonCache();
            var items = db.GetCollection<TeachNewsItem>("TeachNews").FindAll().ToList();
            var network = this.helper.GetLatestNews().ToObservable();
            return network.Select(list => list.Except(items))
                .Do(list => this.helper.SaveNewsItems(list))
                .SelectMany(x => x)
                .Merge(items.ToObservable());
        }
    }
}
