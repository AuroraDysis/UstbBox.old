namespace UstbBox.Wpf.Navigation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    public class NavigationTemplateSelector : DataTemplateSelector
    {
        private readonly ResourceDictionary navigationDictionary;

        private readonly Dictionary<string, DataTemplate> typeDictionary;

        public NavigationTemplateSelector()
        {
            this.navigationDictionary = new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/UstbBox.Wpf;component/Navigation/NavigationTemplates.xaml")
            };

            this.typeDictionary = this.navigationDictionary.Cast<DictionaryEntry>()
                .Where(x => x.Key is Type)
                .ToDictionary(x => (x.Key as Type).FullName, x => x.Value as DataTemplate);
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var wr = item as WeakReference;
            if (wr != null)
            {
                var navItem = wr.Target as INavigationAware;
                if (navItem != null && wr.IsAlive)
                {
                    var type = navItem.GetType().FullName;
                    if (this.typeDictionary.ContainsKey(type))
                    {
                        return this.typeDictionary[type];
                    }
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
