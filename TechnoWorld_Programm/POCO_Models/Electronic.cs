using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Windows.Resources;
using System.Windows;

namespace TechnoWorld_Programm.POCO_Models
{
    public class Electronic
    {
        [JsonPropertyName("electronicsId")]
        public int ElectronicsId { get; set; }
        [JsonPropertyName("model")]
        public string Model { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        [JsonPropertyName("discount")]
        public int Discount { get; set; }
        [JsonPropertyName("manufactrurerId")]
        public int ManufactrurerId { get; set; }
        [JsonPropertyName("typeId")]
        public int TypeId { get; set; }
        [JsonPropertyName("image")]
        public byte[] Image { get; set; }
        [JsonPropertyName("manufactrurer")]
        public virtual Manufacturer Manufacturer { get; set; }
        [JsonPropertyName("type")]
        public virtual ElectrnicsType Type { get; set; }
        [JsonPropertyName("harantyMonth")]
        public int HarantyMonth { get; set; }
        [JsonPropertyName("manufacturerСountry")]
        public string ManufacturerCountry { get; set; }
        [JsonPropertyName("color")]
        public string Color { get; set; }
        [JsonPropertyName("weight")]
        public double Weight { get; set; }
        [JsonPropertyName("electronicsToStorages")]
        public virtual ICollection<ElectronicsToStorage> ElectronicsToStorages { get; set; }

        public string ManufacturerName
        {
            get => Manufacturer.Name;
        }
        public int AmountInStorage
        {
            get
            {
                return ElectronicsToStorages.FirstOrDefault().Quantity;
            }
        }
        public byte[] DisplayImage 
        {
            get
            {
                if (Image == null)
                {                               
                    Stream stream = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/logo.jpg", UriKind.RelativeOrAbsolute)).Stream;
                    byte[] img = new byte[stream.Length];
                    using (var br = new BinaryReader(stream))
                    {
                        br.Read(img, 0, img.Length);
                    }
                    return img;
                }
                else return Image;
            }
        }
        public object GetProperty(string property)
        {
            return GetType().GetProperty(property).GetValue(this);
        }
    }
}
