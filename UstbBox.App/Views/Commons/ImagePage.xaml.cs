using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace UstbBox.App.Views.Commons
{
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    using Microsoft.Graphics.Canvas;
    using Microsoft.Graphics.Canvas.UI.Xaml;

    using Reactive.Bindings.Extensions;

    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImagePage : Page
    {
        private CanvasVirtualBitmap virtualBitmap;

        public ImagePage()
        {
            this.InitializeComponent();

            this.ViewModel.Image.Where(x => x != null)
                .Do(_ => Busy.SetBusy(true, "Loading..."))
                .ObserveOn(TaskPoolScheduler.Default)
                .SelectMany(x => StorageFile.GetFileFromPathAsync(x.Path))
                .SelectMany(x => x.OpenReadAsync())
                .SelectMany(
                    x =>
                    CanvasVirtualBitmap.LoadAsync(
                        this.ImageVirtualControl.Device, 
                        x, 
                        CanvasVirtualBitmapOptions.CacheOnDemand))
                .ObserveOnUIDispatcher()
                .Subscribe(
                    bitmap =>
                        {
                            this.virtualBitmap = bitmap;
                            var size = this.virtualBitmap.Size;
                            this.ImageVirtualControl.Width = size.Width;
                            this.ImageVirtualControl.Height = size.Height;
                            this.ImageVirtualControl.Invalidate();
                            Busy.SetBusy(false);
                        });
        }

        private void ImageVirtualControlRegionsInvalidated(CanvasVirtualControl sender, CanvasRegionsInvalidatedEventArgs args)
        {
            foreach (var region in args.InvalidatedRegions)
            {
                using (var ds = this.ImageVirtualControl.CreateDrawingSession(region))
                {
                    if (this.virtualBitmap != null)
                    {
                        ds.DrawImage(this.virtualBitmap, region, region);
                    }
                }
            }
        }
    }
}
