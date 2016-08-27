using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Flurl.Http;

namespace UstbBox.Services.ImageServices
{
    using System.IO;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Reactive.Threading.Tasks;

    using Windows.Storage;

    using Models.Images;
    using Windows.Storage.Streams;
    using System.Reactive.Windows.Foundation;

    using LiteDB;

    public class ImageHelper
    {
        public List<string> GetTerms()
        {
            var year = Enumerable.Range(2013, DateTime.Now.Year - 2012);
            return
                year.Select(x => $"{x}-{x + 1}")
                    .SelectMany(x => Enumerable.Range(1, 2).Select(s => x + "-" + s))
                    .ToList();
        }

        public IObservable<bool> GetCalendarExist(string url)
        {
            return url.AllowAnyHttpStatus().HeadAsync().ToObservable().Select(x => x.IsSuccessStatusCode);
        }

        public string GetCalendarUrl(string term)
        {
            var sb = new StringBuilder("http://teach.ustb.edu.cn/upload_files/");
            sb.Append(term);
            sb.Append(".jpg");
            return sb.ToString();
        }
    }
}
