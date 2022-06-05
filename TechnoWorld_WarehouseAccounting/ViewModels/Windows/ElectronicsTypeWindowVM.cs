using MaterialNotificationLibrary;
using MaterialNotificationLibrary.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class ElectronicsTypeWindowVM : BaseModalWindowVM
    {
        private string electronicsTypeName;
        private ObservableCollection<Category> categories;
        private Category selectedCategory;
        public ElectronicsTypeWindowVM()
        {
            LoadCategories();
            AddElectronicsTypeCommand = new RelayCommand(AddElectronicsType);
            ValidationMessageSetter(ElectronicsTypeName, nameof(ElectronicsTypeName));
        }
        public ObservableCollection<Category> Categories { get => categories; set { categories = value; OnPropertyChanged(); } }
        public Category SelectedCategory { get => selectedCategory; set { selectedCategory = value; OnPropertyChanged(); } }
        private async void LoadCategories()
        {
            var response = await ApiService.Instance.GetRequest("api/Categories");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Categories = new ObservableCollection<Category>(JsonConvert.DeserializeObject<List<Category>>(response.Content));
                SelectedCategory = Categories.FirstOrDefault();
            }
        }

        public RelayCommand AddElectronicsTypeCommand { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(120, ErrorMessage = "Длина поля «Название электронной техники» слишком большая: максимум {1} символов")]
        public string ElectronicsTypeName { get => electronicsTypeName; set { electronicsTypeName = value; ValidationMessageSetter(value); } }
        private async void AddElectronicsType(object obj)
        {
            if (GetErrorsCount > 0)
            {
                MaterialNotification.Show("Внимание", $"Не все полня заполнены верно!", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
                return;
            }
            var response = await ApiService.Instance.PostRequest("api/ElectrnicsTypes", new ElectrnicsType() { CategoryId = SelectedCategory.Id, Name = ElectronicsTypeName });
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                MaterialNotification.Show("Успешно", "Тип электронной техники успешно добавлен.", MaterialNotificationButton.Ok, MaterialNotificationImage.Susccess);
                DialogResult = true;
            }
            else
            {
                MaterialNotification.Show("Ошибка", $"{JsonConvert.DeserializeObject<string>(response.Content)}", MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
            }
        }
    }
}
