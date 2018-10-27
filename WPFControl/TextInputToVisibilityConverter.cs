using System;
using System.Windows;
using System.Windows.Data;

namespace CSL_Mod_Manager.Controls
{
    public class TextInputToVisibilityConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Test for non-null
            if (values[0] is bool b && values[1] is bool hasFocus)
            {
                var hasText = !b;

                if (hasFocus || hasText)
                {
                    return Visibility.Collapsed;
                }

            }

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
