namespace UstbBox.Wpf.Navigation
{
    using System;
    using MaterialDesignThemes.Wpf;

    public class NavigationItem
    {
        public string ItemName { get; set; }

        public PackIconKind IconKind { get; set; }

        public Type ViewModelType { get; set; }

        public object Parameter { get; set; }

        public string Introduction { get; set; }
    }
}
