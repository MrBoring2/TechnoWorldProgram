using MaterialNotificationLibrary;
using MaterialNotificationLibrary.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Windows
{
    public class AddManufacturerWindowVM : BaseModalWindowVM
    {
        private string manufacturerName;
        public AddManufacturerWindowVM()
        {
            AddManufacturerCommand = new RelayCommand(AddManufacturer);
            ValidationMessageSetter(ManufacturerName, nameof(ManufacturerName));
        }


        public RelayCommand AddManufacturerCommand { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(120, ErrorMessage = "Длина поля Название производителя слишком большая: максимум {1} символов")]
        public string ManufacturerName { get => manufacturerName; set { manufacturerName = value; ValidationMessageSetter(value); } }
        private async void AddManufacturer(object obj)
        {
            var response = await ApiService.Instance.PostRequest("api/Manufacturers", new Manufacturer() { Name = ManufacturerName });
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                MaterialNotification.Show("Успешно", "Производитель успешно добавлен.", MaterialNotificationButton.Ok, MaterialNotificationImage.Susccess);
                DialogResult = true;
            }
            else
            {
                MaterialNotification.Show("Ошибка", $"{JsonConvert.DeserializeObject<string>(response.Content)}", MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
            }
        }
    }
}
