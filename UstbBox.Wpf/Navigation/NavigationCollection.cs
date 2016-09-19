namespace UstbBox.Wpf.Navigation
{
    using System.Collections.ObjectModel;
    using System.Windows.Markup;

    [ContentProperty(nameof(Items))]
    public class NavigationCollection
    {
        public string GroupName { get; set; }

        public ItemsCollection Items { get; set; } = new ItemsCollection();

        public class ItemsCollection : ObservableCollection<NavigationItem>
        {
        }
    }
}
