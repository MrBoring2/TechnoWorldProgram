using MaterialNotificationLibrary;
using MaterialNotificationLibrary.Enums;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_API.Models;
using TechnoWorld_WarehouseAccounting.Services;
using TechnoWorld_WarehouseAccounting.ViewModels.Windows;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_Helpers.Models;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages
{
    public class EmployeesManagementPageVM : ListEntitiesPageVM<Employee, FilteredEmployees>
    {
        private ObservableCollection<SortParameter> sortParameters;
        private ObservableCollection<ItemWithTitle<Post>> posts { get; set; }
        private ItemWithTitle<Post> selectedPost;
        private Visibility editAddVisibility;
        public event EventHandler onElectronicSelected;
        public EmployeesManagementPageVM() : base(15)
        {
            Initialize();
            LoadData();
            ApiService.Instance.GetHubConnection.On<string>("UpdateEmployees", async (deliveries) =>
            {
                await GetWithFilter();
            });
        }

        public RelayCommand OpenEmployeeWindowCommand { get; set; }
        public RelayCommand OpenEditEmployeeWindowCommand { get; set; }
        public override ObservableCollection<SortParameter> SortParameters { get => sortParameters; set { sortParameters = value; OnPropertyChanged(); } }
        public ObservableCollection<ItemWithTitle<Post>> Posts
        {
            get => posts;
            set
            {
                posts = value;
                OnPropertyChanged();
            }
        }

        public ItemWithTitle<Post> SelectedPost
        {
            get => selectedPost;
            set
            {
                selectedPost = value;
                OnPropertyChanged();
                GetWithFilter();
            }
        }
        public Visibility EditAddVisibility { get => editAddVisibility; set { editAddVisibility = value; OnPropertyChanged(); } }
        protected override string UrlApi => "api/Employees/Filter";

        protected override object FilterParam
        {
            get
            {
                return new
                {
                    search = Search,
                    postId = SelectedPost == null || SelectedPost.Item == null ? 0 : SelectedPost.Item.PostId,
                    sortParameter = SelectedSort.Property,
                    isAscending = SelectedSort.IsAcsending,
                    currentPage = Paginator == null ? 1 : Paginator.SelectedPageNumber,
                    itemsPerPage = ItemsPerPage
                };
            }
        }

        protected async override void AfterSetSelectedSort(SortParameter value)
        {
            await GetWithFilter();
        }

        protected async override void AfterSetSearch(string value)
        {
            await GetWithFilter();
        }

        private void Initialize()
        {

            OpenEmployeeWindowCommand = new RelayCommand(OpenEmployeeWindow);
            OpenEditEmployeeWindowCommand = new RelayCommand(OpenEditEmployeeWindow);
            SortParameters = new ObservableCollection<SortParameter>
            {
                new SortParameter("ФИО", "FullName"),
                new SortParameter("Почта", "Email")
            };

            OnPropertyChanged(nameof(Search));
            OnPropertyChanged(nameof(SelectedSort));
        }


        private async void LoadData()
        {
            await LoadPosts();
            await GetWithFilter();
        }


        private async Task LoadPosts()
        {
            var request = await ApiService.Instance.GetRequest("api/Posts");
            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Posts = new ObservableCollection<ItemWithTitle<Post>>(JsonConvert.DeserializeObject<List<Post>>(request.Content).Select(p => new ItemWithTitle<Post>(p, p.Name)));
                Posts.Insert(0, new ItemWithTitle<Post>(null, "Все"));
                selectedPost = Posts.FirstOrDefault();
                OnPropertyChanged(nameof(SelectedPost));
            }
        }



        private async void OpenEmployeeWindow(object obj)
        {
            EmployeeWindowVM employeeWindowVM = new EmployeeWindowVM();
            await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(employeeWindowVM));
            if (employeeWindowVM.DialogResult == true)
            {
                MaterialNotification.Show("Оповещение", $"Сотрудник {employeeWindowVM.Login} упешно добавлен", MaterialNotificationButton.Ok, MaterialNotificationImage.Susccess);
            }
        }
        private async void OpenEditEmployeeWindow(object obj)
        {
            EmployeeWindowVM employeeWindowVM;
            if (SelectedEntity == null)
            {
                MaterialNotification.Show("Внимание", $"Сначала выберите сотрудника!", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
            }
            else
            {
                employeeWindowVM = new EmployeeWindowVM(SelectedEntity);

                await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(employeeWindowVM));

                if (employeeWindowVM.DialogResult == true)
                {
                    
                }
            }

        }

    }
}
