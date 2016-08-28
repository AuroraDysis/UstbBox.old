using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.App.Converters
{
    using Windows.UI.Xaml.Data;

    using UstbBox.Models.Teach;

    public class TeachNewsItemOrderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var items = value as IEnumerable<TeachNewsItem>;
            return items?.OrderByDescending(x => x.Date);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
