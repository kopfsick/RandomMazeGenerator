using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace RandomMazeGenerator.WPF
{

    public class MultiBoolToThicknessConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var individualThickness = System.Convert.ToInt32(parameter);

            if(values.Any(v => !(v is bool))) return Binding.DoNothing;
            
            return new Thickness(((bool)values[0]) ? individualThickness : 0,
                                 ((bool)values[1]) ? individualThickness : 0,
                                 ((bool)values[2]) ? individualThickness : 0,
                                 ((bool)values[3]) ? individualThickness : 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
