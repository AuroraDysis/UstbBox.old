namespace UstbBox.Wpf.Navigation
{
    using System;

    public interface INavigationAware : IDisposable
    {
        bool Redirect(object parameter);

        void Initialize(object parameter);

        void OnNavigatedTo(object parameter);

        void OnNavigatedFrom();
    }
}
