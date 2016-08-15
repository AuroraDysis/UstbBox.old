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
    using Windows.UI.Core;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Xaml.Media.Imaging;

    using Template10.Services.NavigationService;
    using Template10.Common;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImagePage : Page
    {
        private CanvasVirtualBitmap virtualBitmap;

        private readonly SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();

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
                        },
                    ex =>
                        {
                            Busy.SetBusy(false);
                        });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Image");

            if (animation != null)
            {
                this.ImageVirtualControl.Opacity = 0;

                // Wait for image opened. In future Insider Preview releases, this won't be necessary.
                this.ImageVirtualControl.Loaded += (_1, _2) =>
                {
                    this.ImageVirtualControl.Opacity = 1;
                };
                animation.TryStart(this.ImageVirtualControl);
            }


            BootStrapper.BackRequested += this.BootStrapper_BackRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            BootStrapper.BackRequested -= this.BootStrapper_BackRequested;
        }

        private void BootStrapper_BackRequested(object sender, HandledEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("Image", this.ImageVirtualControl);
            
            e.Handled = true;

            this.ViewModel.NavigationService.GoBack(new SuppressNavigationTransitionInfo());
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
