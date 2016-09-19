namespace UstbBox.Wpf.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Models;

    public class CredentialKindToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var kind = value as CredentialKind;
            if (kind != null)
            {
                return kind.Name + "\n使用本账号的网站有\n" + string.Join("\n", kind.Websites)
                       + (string.IsNullOrWhiteSpace(kind.DefaultPasswordInfomation)
                              ? string.Empty
                              : $"\n{kind.DefaultPasswordInfomation}");
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
