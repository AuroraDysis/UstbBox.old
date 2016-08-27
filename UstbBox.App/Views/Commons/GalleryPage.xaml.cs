using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
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
    using Windows.UI.Composition;
    using Windows.UI.Xaml.Hosting;
    using Windows.UI.Xaml.Media.Animation;

    using UstbBox.App.Controls;
    using Template10.Services.NavigationService;

    using UstbBox.Models.Images;
    using System.Reactive.Linq;

    using Microsoft.Toolkit.Uwp.UI.Controls;

    public sealed partial class GalleryPage : Page
    {
        private static ImageObject navigationImageObject;

        private readonly Compositor compositor;

        private readonly ImplicitAnimationCollection elementImplicitAnimation;

        private long token;

        public GalleryPage()
        {
            this.InitializeComponent();
            this.compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            // Create ImplicitAnimations Collection. 
            this.elementImplicitAnimation = this.compositor.CreateImplicitAnimationCollection();

            // Define trigger and animation that should play when the trigger is triggered. 
            this.elementImplicitAnimation["Offset"] = this.CreateOffsetAnimation();

            if (navigationImageObject != null)
            {
                this.token = this.GridView.RegisterPropertyChangedCallback(ItemsControl.ItemsSourceProperty, this.WhenGoBack);
            }
        }

        public CompositionAnimationGroup CreateOffsetAnimation()
        {
            // Define Offset Animation for the ANimation group
            Vector3KeyFrameAnimation offsetAnimation = this.compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            offsetAnimation.Duration = TimeSpan.FromSeconds(.4);

            // Define Animation Target for this animation to animate using definition. 
            offsetAnimation.Target = "Offset";

            // Define Rotation Animation for Animation Group. 
            ScalarKeyFrameAnimation rotationAnimation = this.compositor.CreateScalarKeyFrameAnimation();
            rotationAnimation.InsertKeyFrame(.5f, 0.160f);
            rotationAnimation.InsertKeyFrame(1f, 0f);
            rotationAnimation.Duration = TimeSpan.FromSeconds(.4);

            // Define Animation Target for this animation to animate using definition. 
            rotationAnimation.Target = "RotationAngle";

            // Add Animations to Animation group. 
            CompositionAnimationGroup animationGroup = this.compositor.CreateAnimationGroup();
            animationGroup.Add(offsetAnimation);
            animationGroup.Add(rotationAnimation);

            return animationGroup;
        }

        private void GridViewContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var elementVisual = ElementCompositionPreview.GetElementVisual(args.ItemContainer);
            elementVisual.ImplicitAnimations = args.InRecycleQueue ? null : this.elementImplicitAnimation;
        }

        private void GridViewItemClick(object sender, ItemClickEventArgs e)
        {
            var container = this.GridView.ContainerFromItem(e.ClickedItem) as GridViewItem;
            if (container != null)
            {
                var root = (FrameworkElement)container.ContentTemplateRoot;
                var image = (UIElement)root.FindName("Image");

                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("Image", image);
            }

            var item = (ImageObject)e.ClickedItem;

            this.Transitions = this.Transitions ?? new TransitionCollection();
            this.Transitions.Add(new ContentThemeTransition());

            this.ViewModel.NavigationService.Navigate(Pages.ImagePage, navigationImageObject = item, new SuppressNavigationTransitionInfo());
        }

        private void WhenGoBack(DependencyObject obj, DependencyProperty property)
        {
            this.GridView.UnregisterPropertyChangedCallback(ItemsControl.ItemsSourceProperty, this.token);
            var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Image");
            if (animation != null)
            {
                var item = this.ViewModel.ImageCollection.Value.FirstOrDefault(
                    s => s.Uri == navigationImageObject.Uri);
                this.GridView.ScrollIntoView(item, ScrollIntoViewAlignment.Default);
                this.GridView.UpdateLayout();
                var container = this.GridView.ContainerFromItem(item) as GridViewItem;
                if (container != null)
                {
                    var root = (FrameworkElement)container.ContentTemplateRoot;
                    var image = (ImageEx)root.FindName("Image");

                    // Wait for image opened. In future Insider Preview releases, this won't be necessary.
                    image.Opacity = 0;
                    image.ImageOpened += (sender_, e_) =>
                        {
                            image.Opacity = 1;
                            animation.TryStart(image);
                        };
                }
                else
                {
                    animation.Cancel();
                }
            }

            navigationImageObject = null;
        }
    }
}
