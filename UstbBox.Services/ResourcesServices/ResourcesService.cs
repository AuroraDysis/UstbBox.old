using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services.ResourcesServices
{
    using System.Reactive.Windows.Foundation;

    using Windows.Networking.BackgroundTransfer;

    public class ResourcesService
    {
        public IObservable<IReadOnlyList<DownloadOperation>> DiscoverActiveDownloads()
        {
            return BackgroundDownloader.GetCurrentDownloadsAsync().ToObservable();
        }
    }
}
