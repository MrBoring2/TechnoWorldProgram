﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MaterialNotificationLibrary;
using MaterialNotificationLibrary.Enums;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_Helpers.ValidationRules;
using WPF_VM_Abstractions;
using WPF_Helpers.Services;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Windows
{
    public class EmployeeWindowVM : BaseModalWindowVM
    {
        private bool isAdd;
        private bool isNewPasswordChecked;
        private bool isNewLoginChecked;
        private string newPassword;
        private string newLogin;
        private string checkPassword;
        private Visibility isEditVisibility;
        private string selectedGender;
        private ObservableCollection<Post> posts;
        private Employee CurrentEmployee { get; set; }
        public EmployeeWindowVM()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            RemoveCommand = new RelayCommand(Remove);
            CurrentEmployee = new Employee();
            IsAdd = true;
            Genders = new List<string>
            {
                "Мужской",
                "Женский"
            };
            Gender = Genders.FirstOrDefault();
            IsNewPasswordChecked = false;
            NewPassword = string.Empty;
            IsEditVisibility = Visibility.Collapsed;

            Task.Run(() => Initialize());
            Task.Run(() => LoadData());

            ValidationMessageSetter(FullName, nameof(FullName));
            ValidationMessageSetter(Passport, nameof(Passport));
            ValidationNotRequiredMessageSetter(IsNewPasswordChecked, NewPassword, nameof(NewPassword));
            ValidationMessageSetter(Email, nameof(Email));
            ValidationMessageSetter(DateOfBirth, nameof(DateOfBirth));
            ValidationMessageSetter(Password, nameof(Password));
            ValidationMessageSetter(Login, nameof(Login));
            ValidationMessageSetter(PhoneNumber, nameof(PhoneNumber));
        }


        public EmployeeWindowVM(Employee employee) : this()
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
        public Visibility IsEditVisibility { get => isEditVisibility; set { isEditVisibility = value; OnPropertyChanged(); } }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand RemoveCommand { get; set; }
        public ObservableCollection<Post> Posts
        {
            get { return posts; }
            set { posts = value; OnPropertyChanged(); }
        }
        public List<string> Genders { get; set; }
        public bool IsAdd { get => isAdd; set { isAdd = value; OnPropertyChanged(); } }
        public string Gender { get => CurrentEmployee.Gender; set { CurrentEmployee.Gender = value; OnPropertyChanged(); } }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(120, ErrorMessage = "Длина поля ФИО слишком большая: максимум {1} символов")]
        public string FullName { get => CurrentEmployee.FullName; set { CurrentEmployee.FullName = value; ValidationMessageSetter(value); } }
        public Post Post { get => CurrentEmployee.Post; set { CurrentEmployee.Post = value; CurrentEmployee.PostId = value.PostId; OnPropertyChanged(); SetRole(); } }
        public bool IsNewPasswordChecked
        {
            get => isNewPasswordChecked;
            set { isNewPasswordChecked = value; OnPropertyChanged(); ValidationNotRequiredMessageSetter(IsNewPasswordChecked, NewPassword, nameof(NewPassword)); }
        }
        public bool IsNewLoginChecked { get => isNewLoginChecked; set { isNewLoginChecked = value; ValidationNotRequiredMessageSetter(IsNewLoginChecked, NewLogin, nameof(NewLogin)); } }
        private void SetRole()
        {
            switch (Post.PostId)
            {
                case 1:
                    {
                        CurrentEmployee.RoleId = 1;
                    }
                    break;
                case 2:
                    {
                        CurrentEmployee.RoleId = 2;
                    }
                    break;
                case 3:
                    {
                        CurrentEmployee.RoleId = 3;
                    }
                    break;
                case 4:
                    {
                        CurrentEmployee.RoleId = 4;
                    }
                    break;
                default:
                    CurrentEmployee.RoleId = 0;
                    break;
            }
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Длина поля Пасспорт должна быть {1} символов")]
        [RegularExpression(@"\d{4}\s\d{6}", ErrorMessage = "Поле Паспорт должно быть в виде [Серия Номер] (0000 000000)")]
        public string Passport { get => CurrentEmployee.Passport; set { CurrentEmployee.Passport = value; ValidationMessageSetter(value); } }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [PhoneNumberValidation(ErrorMessage = "Поле Номер телефона неверно заполнено")]
        public string PhoneNumber { get => CurrentEmployee.PhoneNumber; set { CurrentEmployee.PhoneNumber = value; ValidationMessageSetter(value); } }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Длина поля Email должна быть от {2} до {1} символов")]
        [EmailValidation(ErrorMessage = "Введён некорректный адрес электронной почты")]
        public string Email { get => CurrentEmployee.Email; set { CurrentEmployee.Email = value; ValidationMessageSetter(value); } }

        [DateOfBirthValidation(ErrorMessage = "Возраст сотрудника должен быть от 18 до 80 лет")]
        public DateTime DateOfBirth
        {
            get => CurrentEmployee.DateOfBirth == DateTime.MinValue ? DateTime.Now.Date : CurrentEmployee.DateOfBirth;
            set { CurrentEmployee.DateOfBirth = value; ValidationMessageSetter(value); }
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Длина поля Логин должна быть от {2} до {1} символов")]
        public string Login { get => CurrentEmployee.Login; set { CurrentEmployee.Login = value; ValidationMessageSetter(value); } }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Длина поля Пароль должна быть от {2} до {1} символов")]
        public string Password
        {
            get => CurrentEmployee.Password;
            set { CurrentEmployee.Password = value; ValidationMessageSetter(value); }
        }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Длина поля Новый пароль должна быть от {2} до {1} символов")]
        public string NewPassword
        {
            get => newPassword;
            set { newPassword = value; ValidationNotRequiredMessageSetter(IsNewPasswordChecked, value); }
        }
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Длина поля Подтвердите пароль должна быть от {2} до {1} символов")]
        public string CheckPassword
        {
            get => checkPassword;
            set { checkPassword = value; ValidationNotRequiredMessageSetter(IsNewLoginChecked, value); }
        }
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Длина поля Новый логин должна быть от {2} до {1} символов")]
        public string NewLogin
        {
            get => newLogin;
            set { newLogin = value; ValidationNotRequiredMessageSetter(IsNewLoginChecked, value); }
        }

        private async Task Initialize()
        {

        }
        private async Task InitializeFields(Employee employee)
        {
            IsAdd = false;
            IsEditVisibility = Visibility.Visible;
            CurrentEmployee = (Employee)employee.Clone();
            OnPropertyChanged(nameof(ElectrnicsType));
            OnPropertyChanged(nameof(Manufacturer));
            OnPropertyChanged(nameof(Category));
            ValidationMessageSetter(FullName, nameof(FullName));
            ValidationMessageSetter(Passport, nameof(Passport));
            ValidationNotRequiredMessageSetter(IsNewPasswordChecked, NewPassword, nameof(NewPassword));
            ValidationMessageSetter(Email, nameof(Email));
            ValidationMessageSetter(DateOfBirth, nameof(DateOfBirth));
            ValidationMessageSetter(Password, nameof(Password));
            ValidationMessageSetter(Login, nameof(Login));
            ValidationMessageSetter(PhoneNumber, nameof(PhoneNumber));
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
                Posts = new ObservableCollection<Post>(JsonConvert.DeserializeObject<List<Post>>(response.Content).OrderBy(p => p.Name));
                if (Post == null)
                {
                    Post = Posts.FirstOrDefault();
                }
            }
        }
        public void TextChanged(object sender, TextChangedEventArgs e)
        {
            //if ((sender as TextBox).Text == "")
            //{
            //    e.Handled = true;
            //    (sender as TextBox).Text = "0";
            //}
        }

        private async void Save(object obj)
        {
            if (GetErrorsCount > 0)
            {
                MaterialNotification.Show("Внимание", "Не все поля заполены верно!", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
                return;
            }
            else
            {
                if (IsNewLoginChecked)
                {
                    if (MD5EncoderService.EncodePassword(Login, CheckPassword) == Password)
                    {
                        Login = NewLogin;
                        Password = MD5EncoderService.EncodePassword(Login, CheckPassword);
                    }
                    else
                    {
                        MaterialNotification.Show("Ошибка", "Введённый пароль для изменения логина неверный!", MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(NewPassword))
                {
                    Password = MD5EncoderService.EncodePassword(Login, NewPassword);
                }

                if (isAdd)
                {
                    Password = MD5EncoderService.EncodePassword(Login, Password);
                    var response = await ApiService.Instance.PostRequest("api/Employees", CurrentEmployee);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        DialogResult = true;
                    }
                    else
                    {
                        MaterialNotification.Show("Ошибка", response.Content, MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                    }
                }
                else
                {
                    var response = await ApiService.Instance.PutRequest("api/Employees", CurrentEmployee.EmployeeId, CurrentEmployee);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MaterialNotification.Show("Оповещение", $"Сотрудник {Login} упешно изменён", MaterialNotificationButton.Ok, MaterialNotificationImage.Susccess);
                        DialogResult = true;
                    }
                    else
                    {
                        MaterialNotification.Show("Ошибка", response.Content, MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                    }

                }
            }
        }
        private async void Remove(object obj)
        {
            var result = MaterialNotification.Show("Подтверждение", $"Удалить сотрудника {CurrentEmployee.Login}?", MaterialNotificationButton.YesNo, MaterialNotificationImage.Question);
            if (result == MaterialNotificationResult.Yes)
            {
                var response = await ApiService.Instance.DeleteRequest("api/Employees", CurrentEmployee.EmployeeId);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MaterialNotification.Show("Оповещение", $"Сотрудник {CurrentEmployee.Login} упешно удалён.", MaterialNotificationButton.Ok, MaterialNotificationImage.Susccess);
                    DialogResult = true;
                }
                else
                {
                    MaterialNotification.Show("Произошла ошибка при попытке удаления", $"{JsonConvert.DeserializeObject<string>(response.Content)}", MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                }
            }
        }

        private void Cancel(object obj)
        {
            DialogResult = false;
        }
    }
}
