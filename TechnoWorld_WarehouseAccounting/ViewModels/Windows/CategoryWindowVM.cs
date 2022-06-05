using MaterialNotificationLibrary;
using MaterialNotificationLibrary.Enums;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Windows
{
    public class CategoryWindowVM : BaseModalWindowVM
    {
        private string electronicsTypeName;
        private byte[] image;
        public CategoryWindowVM()
        {
            AddCategoryCommand = new RelayCommand(AddCategory);
            SelectImageCommand = new RelayCommand(SelectImage);
            ValidationMessageSetter(CategoryName, nameof(CategoryName));
        }



        public RelayCommand AddCategoryCommand { get; set; }
        public RelayCommand SelectImageCommand { get; set; }
        public byte[] Image { get => image; set { image = value; OnPropertyChanged(); } }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(120, ErrorMessage = "Длина поля «Название категории» слишком большая: максимум {1} символов")]
        public string CategoryName { get => electronicsTypeName; set { electronicsTypeName = value; ValidationMessageSetter(value); } }
        private async void AddCategory(object obj)
        {
            if (Image == null || GetErrorsCount > 0)
            {
                MaterialNotification.Show("Внимание", $"Не все полня заполнены верно или не выбрано изображение!", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
                return;
            }

            var response = await ApiService.Instance.PostRequest("api/Categories", new Category() { Name = CategoryName, Image = Image });
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
        private void SelectImage(object obj)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения | *.png; *.jpg; *.jpeg;";
            if (openFileDialog.ShowDialog() == true)
            {
                if ((new FileInfo(openFileDialog.FileName).Length <= 600000))
                {
                    Image = File.ReadAllBytes(openFileDialog.FileName);
                }
                else
                {
                    MaterialNotification.Show("Внимание", $"Размер файла не должен превышать 600 кб!", MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                }
            }
        }
    }
}
