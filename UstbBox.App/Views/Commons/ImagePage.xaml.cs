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
    using System.Numerics;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading.Tasks;

    using Microsoft.Graphics.Canvas;
    using Microsoft.Graphics.Canvas.UI.Xaml;

    using Reactive.Bindings.Extensions;

    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI.Composition;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Hosting;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Xaml.Media.Imaging;

    using Robmikh.Util.CompositionImageLoader;

    using Template10.Services.NavigationService;
    using Template10.Common;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImagePage : Page
    {
        private ConnectedAnimation animation;

        public ImagePage()
        {
            this.InitializeComponent();

            this.ViewModel.Image.Where(x => x != null).ObserveOnUIDispatcher().Subscribe(async
                x =>
                    {

                        var visual = ElementCompositionPreview.GetElementVisual(this.ImageScrollViewer);
                        var compositor = visual.Compositor;
                        var imageLoader = ImageLoaderFactory.CreateImageLoader(compositor);
                        var surface = await imageLoader.CreateManagedSurfaceFromUriAsync(new Uri(x.Path));
                       
                        var spriteVisual = compositor.CreateSpriteVisual();
                        spriteVisual.Brush = compositor.CreateSurfaceBrush(surface.Surface);
                        spriteVisual.Size = this.ImageScrollViewer.RenderSize.ToVector2();
                        ElementCompositionPreview.SetElementChildVisual(this.ImageScrollViewer, spriteVisual);

                        this.animation?.TryStart(this.ImageScrollViewer);
                    });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Image");
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("Image", this.ImageScrollViewer);
        }
    }
}
