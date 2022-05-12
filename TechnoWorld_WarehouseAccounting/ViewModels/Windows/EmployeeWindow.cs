using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using TechnoWorld_Notification;
using TechnoWorld_Notification.Enums;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_Helpers.ValidationRules;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Windows
{
    public class EmployeeWindow : BaseModalWindowVM
    {
        private bool isAdd;
        private ObservableCollection<Post> posts;
        private Employee CurrentEmployee { get; set; }
        public EmployeeWindow()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            CurrentEmployee = new Employee();
            IsAdd = true;

            Task.Run(() => Initialize());
            Task.Run(() => LoadData());

            ValidationMessageSetter(FullName, nameof(FullName));
            ValidationMessageSetter(Passport, nameof(Passport));
            ValidationMessageSetter(Email, nameof(Email));
            ValidationMessageSetter(DateOfBirth, nameof(DateOfBirth));
            ValidationMessageSetter(Password, nameof(Password));
            ValidationMessageSetter(Login, nameof(Login));
        }
        public EmployeeWindow(Employee employee) : this()
        {
            Task.Run(() => InitializeFields(employee).Wait());
        }
        public void IsAllowedInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var cursorIndex = (sender as TextBox).CaretIndex;
            var text = (sender as TextBox).Text;
            if (sender is TextBox textBox && !e.Text.All(ch => char.IsDigit(ch)))
            {
                e.Handled = true;
            }
        }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public ObservableCollection<Post> Posts
        {
            get { return posts; }
            set { posts = value; OnPropertyChanged(); }
        }
        public bool IsAdd { get => isAdd; set { isAdd = value; OnPropertyChanged(); } }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(120, ErrorMessage = "Длина поля ФИО слишком большая: максимум {1} символов")]
        public string FullName { get => CurrentEmployee.FullName; set { CurrentEmployee.FullName = value; ValidationMessageSetter(value); } }
        public Post Post { get => CurrentEmployee.Post; set { CurrentEmployee.Post = value; OnPropertyChanged(); } }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Длина поля Пасспорт должна быть {1} символов")]
        [RegularExpression(@"\d{4}\s\d{6}", ErrorMessage = "Поле Паспорт должно быть в виде [Серия Номер] (0000 000000)")]
        public string Passport { get => CurrentEmployee.Passport; set { CurrentEmployee.Passport = value; ValidationMessageSetter(value); } }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Длина поля Email должна быть от {1} до {2} символов")]
        [RegularExpression(@"[a-zA-Z1-9\-\._]+@[a-z1-9]+(.[a-z1-9]+){1,}", ErrorMessage = "Введён неккоректный Email")]

        public string Email { get => CurrentEmployee.Passport; set { CurrentEmployee.Passport = value; ValidationMessageSetter(value); } }
        [DateOfBirthValidation(ErrorMessage = "Нельзя устравивать сотрудника моложе 18 лет")]
        public DateTime DateOfBirth { get => CurrentEmployee.DateOfBirth; set { CurrentEmployee.DateOfBirth = value; OnPropertyChanged(); } }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Длина поля Логин должна быть от {1} до {2} символов")]
        public string Login { get => CurrentEmployee.Passport; set { CurrentEmployee.Passport = value; ValidationMessageSetter(value); } }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Длина поля Пароль должна быть от {1} до {2} символов")]
        public string Password { get => CurrentEmployee.Passport; set { CurrentEmployee.Passport = value; ValidationMessageSetter(value); } }
        private async Task Initialize()
        {

        }
        private async Task InitializeFields(Employee employee)
        {
            IsAdd = false;
            CurrentEmployee = employee;
            OnPropertyChanged(nameof(ElectrnicsType));
            OnPropertyChanged(nameof(Manufacturer));
            OnPropertyChanged(nameof(Category));
            ValidationMessageSetter(FullName, nameof(FullName));
            ValidationMessageSetter(Passport, nameof(Passport));
            ValidationMessageSetter(Email, nameof(Email));
            ValidationMessageSetter(DateOfBirth, nameof(DateOfBirth));
            ValidationMessageSetter(Password, nameof(Password));
            ValidationMessageSetter(Login, nameof(Login));
        }
        private async Task LoadData()
        {
            await LoadPosts();
        }
        private async Task LoadPosts()
        {
            var response = await ApiService.Instance.GetRequest("api/Posts");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Posts = new ObservableCollection<Post>(JsonConvert.DeserializeObject<List<Post>>(response.Content));
                if (Post == null)
                {
                    Post = Posts.FirstOrDefault();
                }
            }
        }
        private async void Save(object obj)
        {
            if (GetErrorsCount > 0)
            {
                MaterialNotification.Show("Внимание", "Не все поля заполены верно!", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
            }
            else
            {
                //if (PurchasePrice > SalePrice)
                //{
                //    MaterialNotification.Show("Внимание", "Цена закупки не должна быть больше цены продажи!", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
                //    return;
                //}

                if (isAdd)
                {
                    var response = await ApiService.Instance.PostRequest("api/Employees", CurrentEmployee);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        DialogResult = true;
                    }
                    else
                    {
                        MaterialNotification.Show("Ошибка", JsonConvert.DeserializeObject<string>(response.Content), MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                    }
                }
                else
                {
                    var response = await ApiService.Instance.PutRequest("api/Employees", CurrentEmployee.EmployeeId, CurrentEmployee);
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        DialogResult = true;
                    }
                    else
                    {
                        MaterialNotification.Show("Ошибка", JsonConvert.DeserializeObject<string>(response.Content), MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                    }

                }
            }
        }

        private void Cancel(object obj)
        {
            DialogResult = false;
        }
    }
}
