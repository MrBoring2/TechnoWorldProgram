using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TechnoWorld_Terminal.Resources.Converters
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var image = new BitmapImage();
            using (var ms = new MemoryStream(value as byte[]))
            {
                image.BeginInit();

                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CacheOption = BitmapCacheOption.None;
                image.DecodePixelHeight = 80;
                image.StreamSource = ms;
                image.EndInit();
            }

            return image;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

