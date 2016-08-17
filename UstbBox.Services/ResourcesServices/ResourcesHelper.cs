using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services.ResourcesServices
{
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Windows.Foundation;
    using Windows.Networking.BackgroundTransfer;
    using Windows.Storage;

    internal class ResourcesHelper
    {
        private StorageFolder defaultCacheFolder;

        public ResourcesHelper(StorageFolder defaultCacheFolder)
        {
            this.defaultCacheFolder = defaultCacheFolder;
        }

        public IObservable<DownloadOperation> CreateDownloadOperation(
            string url,
            string folderName,
            string fileName,
            BackgroundTransferPriority priority)
        {
            var downloader = new BackgroundDownloader();
            return
                from folder in
                    this.defaultCacheFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists)
                    .ToObservable()
                from file in folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting).ToObservable()
                select downloader.CreateDownload(new Uri(url), file);
        }

        //public IObservable<Unit> D

        //public IObservable<Unit> Download(DownloadOperation operation)
        //{
        //    operation.StartAsync().ToObservable().Subscribe()
        //}
    }
}
