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

        public IObservable<ImageObject> GetCampusMap()
        {
            return this.helper.DownloadImage(
                "CampusMap",
                "CampusMap",
                "CampusMap.jpg",
                "http://www.ustb.edu.cn/xxfw/UploadFiles_7320/200905/20090512160457504.jpg");
        }

        public IObservable<ImageObject> GetSchoolCalendars()
        {
            return
                this.helper.GetTerms()
                    .ToObservable()
                    .SelectMany(
                        x =>
                        this.helper.DownloadImage(x, "SchoolCalendars", x + ".jpg", this.helper.GetCalendarUrl(x))
                            .Catch(Observable.Return<ImageObject>(null)))
                    .Where(x => x != null);
        }
    }
}
