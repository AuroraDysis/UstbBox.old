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
        public IObservable<ImageObject> DownloadImage(string key, string folderName, string fileName, string url)
        {
            using (var database = AppDatabase.CommonCache())
            {
                var col = database.GetCollection<ImageObject>(folderName);
                var image = col.FindById(key);
                if (image != null) return Observable.Return(image);
            }
            return
                url.GetBytesAsync()
                    .ToObservable()
                    .Select(this.ThrowOnBadImageBuffer)
                    .SelectMany(bytes => this.SaveToPath(key, folderName, fileName, bytes));
        }

        private byte[] ThrowOnBadImageBuffer(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 64)
                throw new Exception("Invalid Image");
            return bytes;
        }

        private IObservable<ImageObject> SaveToPath(string key, string folderName, string fileName, byte[] bytes)
        {
            return
                ApplicationData.Current.LocalFolder.CreateFolderAsync(
                    folderName,
                    CreationCollisionOption.OpenIfExists)
                    .ToObservable()
                    .SelectMany(folder => folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting))
                    .SelectMany(
                        async file =>
                            {
                                using (var stream = await file.OpenStreamForWriteAsync().ConfigureAwait(false))
                                {
                                    await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                                }
                                using (var database = AppDatabase.CommonCache())
                                {
                                    var imageObject = new ImageObject() { Name = key, Path = file.Path };
                                    var col = database.GetCollection<ImageObject>(folderName);
                                    if (col.Exists(x => x.Name == key))
                                    {
                                        col.Update(imageObject);
                                    }
                                    else
                                    {
                                        col.Insert(imageObject);
                                    }
                                    return imageObject;
                                }
                            });
        }

        private IEnumerable<string> GetTerms()
        {
            var year = Enumerable.Range(2013, DateTime.Now.Year - 2012);
            return year.Select(x => $"{x}-{x + 1}").SelectMany(x => Enumerable.Range(1, 2).Select(s => x + "-" + s));
        }
    }
}
