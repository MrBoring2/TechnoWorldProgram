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
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_Helpers.Models;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages
{
    internal class EmployeesManagementPageVM : ListEntitiesPageVM<Employee, EmployeesFilter>
    {
        private ObservableCollection<SortParameter> sortParameters;
        private ObservableCollection<ItemWithTitle<Role>> roles;
        private ObservableCollection<ItemWithTitle<Post>> posts { get; set; }
        private ItemWithTitle<Role> selectedRole;
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
        public ObservableCollection<ItemWithTitle<Role>> Roles { get => roles; set { roles = value; OnPropertyChanged(); } }
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

        public ItemWithTitle<Role> SelectedRole
        {
            get => selectedRole;
            set
            {
                selectedRole = value;
                OnPropertyChanged();
                GetWithFilter();
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
                    roleId = SelectedRole.Item == null ? 0 : SelectedRole.Item.RoleId,
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

            OpenEmployeeWindowCommand = new RelayCommand(OpenProductWindow);
            OpenEditEmployeeWindowCommand = new RelayCommand(OpenEditProductWindow);
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
            await LoadRoles();
            await LoadPosts();
            await GetWithFilter();
        }


        private async Task LoadRoles()
        {
            var request = await ApiService.Instance.GetRequest("api/Roles");
            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Roles = new ObservableCollection<ItemWithTitle<Role>>(JsonConvert.DeserializeObject<List<Role>>(request.Content).Select(p => new ItemWithTitle<Role>(p, p.Name)));
                Roles.Insert(0, new ItemWithTitle<Role>(null, "Все"));
                selectedRole = Roles.FirstOrDefault();
                OnPropertyChanged(nameof(SelectedRole));
            }
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



        private async void OpenProductWindow(object obj)
        {
            //ProductWindowVM productWindowVM = new ProductWindowVM();
            //await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(productWindowVM));
            //if (productWindowVM.DialogResult == true)
            //{
            //    CustomMessageBox.Show($"Товар {productWindowVM.Model} упешно добавлен", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }
        private async void OpenEditProductWindow(object obj)
        {
            if (EditAddVisibility != Visibility.Collapsed)
            {
                //ProductWindowVM productWindowVM;
                //if (SelectedEntity == null)
                //{
                //    CustomMessageBox.Show($"Сначала выберите товар!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                //}
                //else
                //{
                //    productWindowVM = new ProductWindowVM(SelectedEntity);

                //    await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(productWindowVM));

                //    if (productWindowVM.DialogResult == true)
                //    {
                //        CustomMessageBox.Show($"Товар {productWindowVM.Model} упешно изменён", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                //    }
                //}
            }
        }

    }
}
