using System;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace KatjasFotoTool.View
{
    /// <summary>
    /// Sets the cursor state of the mouse.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Cursors))]
    public class BusyMouseConverter : MarkupExtension, IValueConverter
    {
        public BusyMouseConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value)
                    return Cursors.Wait;
                else
                    return null;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Cursors)
            {
                if (value == Cursors.Wait)
                    return true;
                else
                    return false;
            }

            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return instance;
        }

        private static BusyMouseConverter instance = new BusyMouseConverter();

    }
}
