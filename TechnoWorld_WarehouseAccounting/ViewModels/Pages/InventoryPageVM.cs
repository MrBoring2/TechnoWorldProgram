using MaterialNotificationLibrary;
using MaterialNotificationLibrary.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_WarehouseAccounting.Models;
using TechnoWorld_WarehouseAccounting.Services;
using TechnoWorld_WarehouseAccounting.ViewModels.Windows;
using TechnoWorld_WarehouseAccounting.Views.Windows;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_VM_Abstractions;
using Excel = Microsoft.Office.Interop.Excel;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages
{
    public class InventoryPageVM : BasePageVM
    {
        private decimal factTotalPrice;
        private decimal buhTotalPrice;
        private ObservableCollection<InventoryModel> inventoryModels;
        private InventoryModel selectedInventoryModel;
        private ObservableCollection<Storage> storages;
        private Storage selectedStorage;
        private Visibility emptyVisibility;
        public InventoryPageVM()
        {
            LoadStorages();
            InventoryModels = new ObservableCollection<InventoryModel>();
            InventoryModels.CollectionChanged += InventoryModels_CollectionChanged;
            AddProductCommand = new RelayCommand(AddProduct);
            RemoveProductCommand = new RelayCommand(RemoveProduct);
            FillTableCommand = new RelayCommand(FillTable);
            SpendCommand = new RelayCommand(Spend);
            ClearTableCommand = new RelayCommand(ClearTable);
            EmptyVisibility = Visibility.Visible;
        }
        public Visibility EmptyVisibility { get => emptyVisibility; set { emptyVisibility = value; OnPropertyChanged(); } }
        private void ClearTable(object obj)
        {
            InventoryModels.Clear();
            FactTotalPrice = 0;
            BuhTotalPrice = 0;
        }

        private async void Spend(object obj)
        {
            if (InventoryModels.Count > 0)
            {
                await Export();
            }
            else
            {
                MaterialNotification.Show("Внимание", "Товров в инвентаризации должно быть больше 0!", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
            }

        }

        private async void AddProduct(object obj)
        {
            var productsWindow = new ProductListWindowVM();
            await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(productsWindow));
            if (productsWindow.DialogResult == true)
            {
                if (InventoryModels.FirstOrDefault(p => p.Electronics.ElectronicsId == productsWindow.SelectedEntity.ElectronicsId) == null)
                {
                    var inventoryModel = new InventoryModel(
                        productsWindow.SelectedEntity,
                        productsWindow.SelectedEntity.ElectronicsToStorages
                                                            .Where(p => p.StorageId == SelectedStorage.StorageId).Sum(p => p.Quantity), productsWindow.SelectedEntity.PurchasePrice);
                    inventoryModel.PropertyChanged += InventoryModel_PropertyChanged;
                    InventoryModels.Add(inventoryModel);
                    FactTotalPrice = InventoryModels.Sum(p => p.FactTotalPrice);
                    BuhTotalPrice = InventoryModels.Sum(p => p.BuhTotalPrice);
                }
                else
                {
                    MaterialNotification.Show("Внимание", "Даный товар уже есть в списке!", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
                }
            }
        }
        private void InventoryModels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            int id = 1;
            foreach (var item in sender as ObservableCollection<InventoryModel>)
            {
                if (item.Id != id)
                {
                    item.Id = id;
                }
                id++;
            }
            if (InventoryModels.Count <= 0)
            {
                EmptyVisibility = Visibility.Visible;
            }
            else
            {
                EmptyVisibility = Visibility.Collapsed;
            }
        }

        private void InventoryModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            FactTotalPrice = InventoryModels.Sum(p => p.FactTotalPrice);
            BuhTotalPrice = InventoryModels.Sum(p => p.BuhTotalPrice);
        }
        private void RemoveProduct(object obj)
        {
            if (SelectedInventoryModel != null)
            {
                FactTotalPrice -= SelectedInventoryModel.FactTotalPrice;
                BuhTotalPrice -= SelectedInventoryModel.BuhTotalPrice;
                InventoryModels.Remove(SelectedInventoryModel);
            }
            else
            {
                MaterialNotification.Show("Внимание", "Сначала выберите товар!", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
            }
        }
        private async void FillTable(object obj)
        {
            var response = await ApiService.Instance.GetRequest("api/Electronics/All");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var electronics = JsonConvert.DeserializeObject<List<Electronic>>(response.Content);
                InventoryModels.Clear();
                foreach (var item in electronics)
                {
                    var inventoryModel =
                   (new InventoryModel(
                        item,
                        item.ElectronicsToStorages.Where(p => p.StorageId == SelectedStorage.StorageId).Sum(p => p.Quantity),
                        item.PurchasePrice));

                    inventoryModel.PropertyChanged += InventoryModel_PropertyChanged;
                    InventoryModels.Add(inventoryModel);
                }
            }
        }
        private async Task LoadStorages()
        {
            await Task.Run(async () =>
            {
                var response = await ApiService.Instance.GetRequest("api/Storages");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Storages = new ObservableCollection<Storage>(JsonConvert.DeserializeObject<List<Storage>>(response.Content));
                    if (SelectedStorage == null)
                    {
                        SelectedStorage = Storages.FirstOrDefault();
                    }
                }
            });
        }
        public RelayCommand BackCommand { get; set; }
        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand RemoveProductCommand { get; set; }
        public RelayCommand FillTableCommand { get; set; }
        public RelayCommand SpendCommand { get; set; }
        public RelayCommand ClearTableCommand { get; set; }
        public ObservableCollection<Storage> Storages { get => storages; set { storages = value; OnPropertyChanged(); } }
        public Storage SelectedStorage { get => selectedStorage; set { selectedStorage = value; OnPropertyChanged(); AfterChangeStorage(); } }

        private void AfterChangeStorage()
        {
            foreach (var item in InventoryModels)
            {
                item.BuhAmount = item.Electronics.ElectronicsToStorages
                    .Where(p => p.StorageId == SelectedStorage.StorageId)
                    .Sum(p => p.Quantity);
            }
        }

        public decimal FactTotalPrice
        {
            get => factTotalPrice;
            set
            {
                factTotalPrice = value;
                OnPropertyChanged();
            }
        }
        public decimal BuhTotalPrice
        {
            get => buhTotalPrice;
            set
            {
                buhTotalPrice = value;
                OnPropertyChanged();
            }
        }
        public InventoryModel SelectedInventoryModel
        {
            get => selectedInventoryModel;
            set
            {
                selectedInventoryModel = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<InventoryModel> InventoryModels
        {
            get => inventoryModels;
            set
            {
                inventoryModels = value;
                OnPropertyChanged();
            }
        }


        private async Task Export()
        {
            await Task.Run(async () =>
            {

                string inventoryNumber = GenerateInventoryNumber();

                var application = new Excel.Application();
                application.SheetsInNewWorkbook = 1;
                Excel.Workbook workbook = application.Workbooks.Add(Type.Missing);

                Excel.Worksheet worksheet = application.Worksheets.Item[1];
                worksheet.Name = "Инвентаризация";
                worksheet.Cells[1][1] = $"Инвентаризация № {inventoryNumber} от {DateTime.Now.ToLocalTime()}";
                worksheet.Cells[1][1].Font.Size = 20;
                worksheet.Cells[1][1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                Excel.Range title = worksheet.Range[worksheet.Cells[1][1], worksheet.Cells[8][1]];
                title.Merge();

                worksheet.Cells[1][3] = $"Склад: {SelectedStorage.Name}";
                worksheet.Cells[1][3].Font.Size = 16;

                worksheet.Cells[1][5] = $"Продукция";
                worksheet.Cells[1][5].Font.Size = 16;
                worksheet.Cells[1][5].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                Excel.Range tableTitle = worksheet.Range[worksheet.Cells[1][5], worksheet.Cells[8][5]];
                tableTitle.Merge();

                int startRowIndex = 7;

                worksheet.Cells[1][startRowIndex] = "№";
                worksheet.Cells[2][startRowIndex] = "Название";
                worksheet.Cells[3][startRowIndex] = "Количество фактическое";
                worksheet.Cells[4][startRowIndex] = "Количество учётное";
                worksheet.Cells[5][startRowIndex] = "Отклонение";
                worksheet.Cells[6][startRowIndex] = "Цена";
                worksheet.Cells[7][startRowIndex] = "Стоимость фактическая";
                worksheet.Cells[8][startRowIndex] = "Стоимость учётная";
                startRowIndex++;

                foreach (var inventoryModel in InventoryModels)
                {
                    worksheet.Cells[1][startRowIndex] = inventoryModel.Id;
                    worksheet.Cells[2][startRowIndex] = inventoryModel.Electronics.Model;
                    worksheet.Cells[3][startRowIndex] = inventoryModel.FactAmount;
                    worksheet.Cells[4][startRowIndex] = inventoryModel.BuhAmount;
                    worksheet.Cells[5][startRowIndex].Formula = $"=C{startRowIndex}-D{startRowIndex}";
                    worksheet.Cells[6][startRowIndex] = inventoryModel.Price;
                    worksheet.Cells[7][startRowIndex].Formula = $"=F{startRowIndex}*C{startRowIndex}";
                    worksheet.Cells[8][startRowIndex].Formula = $"=F{startRowIndex}*D{startRowIndex}";
                    startRowIndex++;
                }
                //startRowIndex += 2;


                worksheet.Cells[5][startRowIndex] = "Сумма фактическая:";
                worksheet.Cells[6][startRowIndex].Formula = $"=SUM(G{2}:G{startRowIndex - 1})";
                worksheet.Cells[7][startRowIndex] = "Сумма учётная:";
                worksheet.Cells[8][startRowIndex].Formula = $"=SUM(H{2}:" + $"H{startRowIndex - 1})";
                worksheet.Cells[5][startRowIndex].Font.Bold = worksheet.Cells[7][startRowIndex].Font.Bold = true;
                Excel.Range rangeBorders = worksheet.Range[worksheet.Cells[1][7], worksheet.Cells[8][startRowIndex - 1]];
                rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle =
                rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle =
                rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle =
                rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle =
                rangeBorders.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle =
                rangeBorders.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;

                worksheet.Columns.AutoFit();

                workbook.SaveAs($@"{AppDomain.CurrentDomain.BaseDirectory}Инвентаризация\Инвентаризация №{inventoryNumber}.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault,
                    Type.Missing, Type.Missing,
                    false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                application.Visible = true;
                await App.Current.Dispatcher.InvokeAsync(() => MaterialNotification.Show("Оповещение", $"Документ Инвентаризация № {inventoryNumber} сохранён в формате Excel в папке «Инвентаризация»", MaterialNotificationButton.Ok, MaterialNotificationImage.Susccess));
            });

        }
        private string GenerateInventoryNumber()
        {
            Random random = new Random();
            string number = "";
            string keys = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < 10; i++)
            {
                number += keys[random.Next(0, keys.Length)];
            }

            //number += $"{DateTime.Now.Day}{DateTime.Now.Month}{DateTime.Now.Year}";

            return number;
        }
    }
}
