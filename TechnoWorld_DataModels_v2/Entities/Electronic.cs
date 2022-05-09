using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TechoWorld_DataModels_v2.Abstractions;

namespace TechoWorld_DataModels_v2.Entities
{
    public partial class Electronic : BaseEntity
    {
        public Electronic()
        {
            ElectronicsToDeliveries = new HashSet<ElectronicsToDelivery>();
            ElectronicsToStorages = new HashSet<ElectronicsToStorage>();
            OrderElectronics = new HashSet<OrderElectronic>();
        }

        //public string this[string columnName]
        //{
        //    get
        //    {
        //        string error = string.Empty;
        //        switch (columnName)
        //        {
        //            case "Model":
        //                if (Model.Length == 0)
        //                {
        //                    error = "Поле модель не заполнено.";
        //                }
        //                else if (Model.Length > 150)
        //                {
        //                    error = "Слишком длинное значение.";
        //                }
        //                break;
        //            case "SalePrice":
        //                if (SalePrice <= 0)
        //                {
        //                    error = "Цена продажи должна быль больше 0.";
        //                }
        //                break;
        //            case "PurchasePrice":
        //                if (PurchasePrice <= 0)
        //                {
        //                    error = "Цена закупки не должна быть больше 0.";
        //                }
        //                else if (PurchasePrice > SalePrice)
        //                {
        //                    error = "Цена закупки не олжна быть больше цены продажи";
        //                }
        //                break;
        //            case "Color":
        //                {
        //                    if (Color.Length == 0)
        //                    {
        //                        error = "Поле цвет не заполнено.";
        //                    }
        //                }
        //                break;
        //            case "Weight":
        //                if (Weight <= 0)
        //                {
        //                    error = "Вес должен быть больше 0.";
        //                }
        //                break;
        //            case "ManufacturerCountry":
        //                if (ManufacturerСountry.Length == 0)
        //                {
        //                    error = "Поле страна производитель не заполнено.";
        //                }
        //                break;
        //        }
        //        return error;
        //    }
        //}

        public int ElectronicsId { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public bool IsOfferedForSale { get; set; }
        public int ManufactrurerId { get; set; }
        public int TypeId { get; set; }

        public byte[] Image { get; set; }
        public string ManufacturerСountry { get; set; }
        public string Color { get; set; }
        public double Weight { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public virtual ElectrnicsType Type { get; set; }
        //[JsonIgnore]
        public virtual ICollection<ElectronicsToDelivery> ElectronicsToDeliveries { get; set; }

        public virtual ICollection<ElectronicsToStorage> ElectronicsToStorages { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderElectronic> OrderElectronics { get; set; }
        [NotMapped]
        public string ManufacturerName
        {
            get => Manufacturer?.Name;
        }
        [NotMapped]
        public int AmountInStorage
        {
            get
            {
                return ElectronicsToStorages.Count() > 0 && ElectronicsToStorages != null ? ElectronicsToStorages.Sum(p => p.Quantity) : 0;
            }
        }
        [NotMapped]
        public string AmountnAveryStorage
        {
            get
            {
                var str = "";
                if (ElectronicsToStorages.Count() > 0 && ElectronicsToStorages != null)
                {
                    foreach (var item in ElectronicsToStorages)
                    {
                        if (item != ElectronicsToStorages.LastOrDefault())
                        {
                            str += $"На складе {item.Storage.Name}: {item.Quantity} шт.\n";
                        }
                        else
                        {
                            str += $"На складе {item.Storage.Name}: {item.Quantity} шт.";
                        }
                    }
                }
                else
                {
                    str += "Нет в наличии";
                }
                return str;
            }
        }
        [NotMapped]
        public string Status
        {
            get
            {
                if (IsOfferedForSale)
                {
                    return "Выставлен на продажу";
                }
                else
                {
                    return "Снят с продажи";
                }
            }
        }

        //public string Error => null;
    }
}
