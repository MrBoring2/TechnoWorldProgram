﻿using System;
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
            if (value != null)
            {
                var image = new BitmapImage();
                using (var ms = new MemoryStream(value as byte[]))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.DecodePixelHeight = 80;
                    image.StreamSource = ms;
                    image.EndInit();
                }

                image.Freeze();
                return image;
            }
            else
            {
                try
                {
                    var uri = new Uri("pack://application:,,,/Resources/Images/no-pictures.png");
                    var bitmap = new BitmapImage(uri);
                    using (var ms = new MemoryStream())
                    {
                        var pngEncoder = new PngBitmapEncoder();
                        pngEncoder.Frames.Add(BitmapFrame.Create(bitmap));
                        pngEncoder.Save(ms);
                        return ms.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    throw new NullReferenceException("Отсутствует файл изображения в ресурсах!");
                }
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

