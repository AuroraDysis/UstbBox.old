using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services.ImageServices
{
    using System.Reactive.Linq;

    using UstbBox.Models.Images;

    public class ImageService : IImageService
    {
        private readonly ImageHelper helper = new ImageHelper();

        public ImageObject GetCampusMap()
        {
            return new ImageObject("校园地图", "http://www.ustb.edu.cn/xxfw/UploadFiles_7320/200905/20090512160457504.jpg");
        }

        public IObservable<IList<ImageObject>> GetSchoolCalendars()
        {
            var images = this.helper.GetTerms().Select(x => new ImageObject(x, this.helper.GetCalendarUrl(x))).ToList();
            var db = AppDatabase.CommonCache();
            var col = db.GetCollection<ImageObject>("SchoolCalendars");
            var confirm = col.FindAll().ToList();
            db.Dispose();

            var unconfirm =
                images.Except(confirm)
                    .ToObservable()
                    .SelectMany(x => this.helper.GetCalendarExist(x.Uri).Select(b => Tuple.Create(b, x)))
                    .Where(x => x.Item1)
                    .Select(x => x.Item2);

            return confirm.ToObservable().Merge(unconfirm).ToList();
        }
    }
}
