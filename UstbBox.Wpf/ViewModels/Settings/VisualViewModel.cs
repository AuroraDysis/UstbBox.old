namespace UstbBox.Wpf.ViewModels.Settings
{
    using System;
    using System.Reactive.Linq;
    using MaterialDesignColors;
    using MaterialDesignThemes.Wpf;
    using Reactive.Bindings;
    using UstbBox.Wpf.Extensions;
    using UstbBox.Wpf.Services;
    using UstbBox.Wpf.ViewModels.Mvvm;

    public class VisualViewModel : ViewModelBase
    {
        private readonly PaletteHelper paletteHelper = new PaletteHelper();

        public ReadOnlyReactiveCollection<Swatch> Swatches { get; set; }

        public ReactiveProperty<bool> IsDark { get; set; }

        public ReactiveCommand<Swatch> CommandApplyPrimary { get; set; }

        public ReactiveCommand<Swatch> CommandApplyAccent { get; set; }

        public ReactiveCommand CommandReset { get; set; }

        public override void Initialize(object parameter)
        {
            base.Initialize(parameter);

            var service = SettingService.Instance;

            this.Swatches = new SwatchesProvider().Swatches.ToObservable().ToReadOnlyReactiveCollection();

            this.IsDark = new ReactiveProperty<bool>(service.IsDark);
            this.IsDark.DoOnUI(this.paletteHelper.SetLightDark).ObserveOnBackgroundAndSubscribe(b => service.IsDark = b);

            this.CommandApplyAccent = new ReactiveCommand<Swatch>();
            this.CommandApplyAccent.DoOnUI(this.paletteHelper.ReplaceAccentColor).ObserveOnBackgroundAndSubscribe(sw => service.AccentColor = sw.Name);

            this.CommandApplyPrimary = new ReactiveCommand<Swatch>();
            this.CommandApplyPrimary.DoOnUI(this.paletteHelper.ReplacePrimaryColor).ObserveOnBackgroundAndSubscribe(sw => service.PrimaryColor = sw.Name);

            this.CommandReset = new ReactiveCommand();
            this.CommandReset.DoOnBackground(_ => service.ResetSettings()).ObserveOnUIAndSubscribe(_ =>
                {
                    this.paletteHelper.ReplaceAccentColor(service.AccentColor);
                    this.paletteHelper.ReplacePrimaryColor(service.PrimaryColor);
                    this.paletteHelper.SetLightDark(service.IsDark);
                });

            this.AddToDisposableGroup(this.Swatches, this.CommandApplyAccent, this.CommandApplyPrimary, this.CommandReset);
        }
    }
}
