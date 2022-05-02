﻿using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_WarehouseAccounting.Common;
using TechnoWorld_WarehouseAccounting.Models;
using TechnoWorld_WarehouseAccounting.Services;
using TechnoWorld_WarehouseAccounting.Views.Windows;
using TechoWorld_DataModels;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Windows
{
    public class DeliveryWindowVM : BaseModalWindowVM
    {
        private ObservableCollection<Supplier> suppliers;
        private ObservableCollection<Storage> storages;
        private ObservableCollection<DeliveryItem> deliveryItems;
        private DeliveryItem selectedDeliveryItem;
        private Visibility cancelVisibility;
        private Visibility payVisibility;
        private Visibility createVisibility;
        private Visibility unloadVisibility;
        private Visibility createReceiptInvoiceVisibility;
        private bool isAdd;
        private DateTime dateOfDelivery;
        public DeliveryWindowVM()
        {
            Initialize();
            LoadData();

        }
        public DeliveryWindowVM(Delivery delivery) : this()
        {
            InitializeFields(delivery);
        }
        private void InitializeFields(Delivery delivery)
        {
            IsAdd = false;
            Delivery = delivery;
            DateOfDelivery = Delivery.DateOfDelivery;
            int id = 1;
            DeliveryItems = new ObservableCollection<DeliveryItem>();
            foreach (var item in Delivery.ElectronicsToDeliveries)
            {
                DeliveryItems.Add(new DeliveryItem { Id = id, Count = item.Quantity, Electronic = item.Electronics });
                id++;
            }
            if (Delivery.StatusId == 1)
            {
                PayVisibility = Visibility.Visible;
                CancelVisibility = Visibility.Visible;
                CreateVisibility = Visibility.Collapsed;
                CreateReceiptInvoiceVisibility = Visibility.Visible;
                UnloadVisibility = Visibility.Collapsed;
            }
            else if (Delivery.StatusId == 5)
            {
                PayVisibility = Visibility.Collapsed;
                CancelVisibility = Visibility.Collapsed;
                CreateVisibility = Visibility.Collapsed;
                CreateReceiptInvoiceVisibility = Visibility.Visible;
                UnloadVisibility = Visibility.Visible;
            }
            else if (Delivery.StatusId == 4 || Delivery.StatusId == 3)
            {
                PayVisibility = Visibility.Collapsed;
                CancelVisibility = Visibility.Collapsed;
                CreateVisibility = Visibility.Collapsed;
                CreateReceiptInvoiceVisibility = Visibility.Collapsed;
                UnloadVisibility = Visibility.Collapsed;
            }
        }
        public Delivery Delivery { get; set; }
        public RelayCommand BackCommand { get; set; }
        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand RemoveProductCommand { get; set; }
        public RelayCommand CreateDeliveryCommand { get; set; }
        public RelayCommand CreateReceiptInvoiceCommand { get; set; }
        public RelayCommand PayDeliveryCommand { get; set; }
        public RelayCommand CancelDeliveryCommand { get; set; }
        public RelayCommand UnloadToStorageCommand { get; set; }
        public DeliveryItem SelectedDeliveryItem { get => selectedDeliveryItem; set { selectedDeliveryItem = value; OnPropertyChanged(); } }
        public string DeliveryNumber { get => Delivery.DeliveryNumber; set { Delivery.DeliveryNumber = value; OnPropertyChanged(); } }
        public string DeliveryTitle => $"{DeliveryNumber} от {Delivery.DateOfOrder.ToShortDateString()}";
        public bool IsAdd { get => isAdd; set { isAdd = value; OnPropertyChanged(); } }
        public DateTime DateOfDelivery { get => dateOfDelivery; set { dateOfDelivery = value.Date >= Delivery.DateOfOrder.Date ? value : dateOfDelivery; OnPropertyChanged(); } }
        public Visibility PayVisibility { get => payVisibility; set { payVisibility = value; OnPropertyChanged(); } }
        public Visibility CancelVisibility { get => cancelVisibility; set { cancelVisibility = value; OnPropertyChanged(); } }
        public Visibility CreateVisibility { get => createVisibility; set { createVisibility = value; OnPropertyChanged(); } }
        public Visibility UnloadVisibility { get => unloadVisibility; set { unloadVisibility = value; OnPropertyChanged(); } }
        public Visibility CreateReceiptInvoiceVisibility { get => createReceiptInvoiceVisibility; set { createReceiptInvoiceVisibility = value; OnPropertyChanged(); } }
        public Storage SelectedStorage { get => Delivery.Storage; set { Delivery.Storage = value; Delivery.StorageId = value.StorageId; OnPropertyChanged(); } }
        public Supplier SelectedSupplier { get => Delivery.Supplier; set { Delivery.Supplier = value; Delivery.SupplierId = value.SupplierId; OnPropertyChanged(); } }
        public ObservableCollection<DeliveryItem> DeliveryItems { get => deliveryItems; set { deliveryItems = value; OnPropertyChanged(); } }
        public ObservableCollection<Supplier> Suppliers { get => suppliers; set { suppliers = value; OnPropertyChanged(); } }
        public ObservableCollection<Storage> Storages { get => storages; set { storages = value; OnPropertyChanged(); } }
        public decimal PayPrice => IsAdd ? 0 : Delivery.TotalPrice;
        private void Initialize()
        {
            IsAdd = true;
            Delivery = new Delivery();
            DeliveryNumber = $"ЗП{GenerateDeliveryNumber()}";
            Delivery.DateOfOrder = DateTime.Now;
            DateOfDelivery = DateTime.Now;
            BackCommand = new RelayCommand(Back);
            AddProductCommand = new RelayCommand(AddProduct);
            RemoveProductCommand = new RelayCommand(RemoveProduct);
            CreateDeliveryCommand = new RelayCommand(CreateDelivery);
            CreateReceiptInvoiceCommand = new RelayCommand(CreateReceiptInvoice);
            UnloadToStorageCommand = new RelayCommand(UnloadToStorage);
            CancelDeliveryCommand = new RelayCommand(CancelDelivery);
            PayDeliveryCommand = new RelayCommand(PayDelivery);
            DeliveryItems = new ObservableCollection<DeliveryItem>();
            DeliveryItems.CollectionChanged += DeliveryItems_CollectionChanged;
            PayVisibility = Visibility.Collapsed;
            CancelVisibility = Visibility.Collapsed;
            CreateVisibility = Visibility.Visible;
            UnloadVisibility = Visibility.Collapsed;
            CreateReceiptInvoiceVisibility = Visibility.Collapsed;
            OnPropertyChanged(nameof(DeliveryTitle));
        }

        private async void LoadData()
        {
            await LoadStorages();
            await LoadSuppliers();
        }


        private async Task LoadSuppliers()
        {
            var response = await ApiService.GetRequest("api/Suppliers");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Suppliers = new ObservableCollection<Supplier>(JsonConvert.DeserializeObject<List<Supplier>>(response.Content));
                SelectedSupplier = Suppliers.FirstOrDefault();
            }
        }

        private async Task LoadStorages()
        {
            var response = await ApiService.GetRequest("api/Storages");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Storages = new ObservableCollection<Storage>(JsonConvert.DeserializeObject<List<Storage>>(response.Content));
                SelectedStorage = Storages.FirstOrDefault();
            }
        }
        private async void AddProduct(object obj)
        {
            var productsListVM = new ProductListWindowVM();
            await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(productsListVM));

            if (productsListVM.DialogResult == true)
            {
                if (DeliveryItems.Any(p => p.Electronic.ElectronicsId == productsListVM.SelectedElectronic.ElectronicsId) == false)
                {
                    DeliveryItems.Add(new DeliveryItem(productsListVM.SelectedElectronic, 0));
                }
            }
        }
        private async void CreateDelivery(object obj)
        {
            if (DeliveryItems.Any(p => p.Count <= 0))
            {
                CustomMessageBox.Show("Не у всех товаров установлено количество!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Delivery.DateOfDelivery = DateOfDelivery;
            Delivery.StatusId = 1;
            Delivery.StorageId = SelectedStorage.StorageId;
            Delivery.SupplierId = SelectedSupplier.SupplierId;
            Delivery.EmployeeId = ClientService.Instance.User.UserId;
            Delivery.ElectronicsToDeliveries = DeliveryItems.Select(p => new ElectronicsToDelivery { ElectronicsId = p.Electronic.ElectronicsId, Quantity = p.Count }).ToList();
            var response = await ApiService.PostRequest("api/Deliveries", Delivery);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                GenerateReceiptInvoice(GenerateDeliveryNumber(), SelectedStorage, SelectedSupplier, Delivery.DateOfOrder, Delivery.DateOfDelivery, DeliveryItems);
                DialogResult = true;
            }
            else
            {
                CustomMessageBox.Show($"{response.Content}", "Произошла ошибка при добавлении!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CreateReceiptInvoice(object obj)
        {
            GenerateReceiptInvoice(Delivery.DeliveryNumber, SelectedStorage, SelectedSupplier, Delivery.DateOfOrder, Delivery.DateOfDelivery, DeliveryItems);
            CustomMessageBox.Show($"Приходная накладная сформирована в папке «Приходные накладные»", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GenerateReceiptInvoice(string deliveryNumber, Storage storage, Supplier supplier, DateTime dateOfOrder, DateTime dateOfDelivery, IEnumerable<DeliveryItem> deliveryItems)
        {
            var recieptInviceNumber = $"ПН{deliveryNumber}";
            FileStream fs = new FileStream($"Приходные накладные/{recieptInviceNumber}.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            Rectangle rec2 = new Rectangle(PageSize.A4);


            Document doc = new Document(rec2, 50, 50, 10, 10);

            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            string ttf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
            var baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            var textFont = new iTextSharp.text.Font(baseFont, 14, iTextSharp.text.Font.NORMAL);
            var tableFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.NORMAL);
            var titleFont = new iTextSharp.text.Font(baseFont, 21, iTextSharp.text.Font.NORMAL);
            writer.SetLanguage("ru-RU");
            doc.AddAuthor("Денис Симонов");
            doc.AddCreator("TechnoWorld_WarehouseAccounting using iTextSharp");
            doc.AddKeywords("PDF приходная накладная");
            doc.AddSubject("Приходная накладная");
            doc.AddTitle($"Приходная накладная {recieptInviceNumber}");
            doc.Open();

            Paragraph paragraph;
            paragraph = new Paragraph($"Приходная накладная {recieptInviceNumber} от {dateOfOrder.Day}.{dateOfOrder.Month}.{dateOfOrder.Year}", titleFont);
            paragraph.Alignment = 0;
            //paragraph.ExtraParagraphSpace = 20;
            paragraph.Leading = 30;
            paragraph.SpacingAfter = 20;
            doc.Add(paragraph);

            paragraph = new Paragraph($"Склад: {SelectedStorage.Name}", textFont);
            paragraph.Alignment = 3;
            paragraph.SpacingAfter = 20;
            doc.Add(paragraph);

            paragraph = new Paragraph($"Поставщик: {SelectedSupplier.Name}", textFont);
            paragraph.Alignment = 3;
            paragraph.SpacingAfter = 20;
            doc.Add(paragraph);

            paragraph = new Paragraph($"Покупатель: ООО «Техно Мир»", textFont);
            paragraph.Alignment = 3;
            paragraph.SpacingAfter = 20;
            doc.Add(paragraph);

            paragraph = new Paragraph($"Плановая дата поставки: {dateOfDelivery}", textFont);
            paragraph.Alignment = 3;
            paragraph.SpacingAfter = 20;
            doc.Add(paragraph);

            paragraph = new Paragraph($"Отпустил: {ClientService.Instance.User.Post} {ClientService.Instance.User.FullName}", textFont);
            paragraph.Alignment = 3;
            paragraph.SpacingAfter = 10;
            doc.Add(paragraph);

            paragraph = new Paragraph($"Принял: ___________________ ___________________", textFont);
            paragraph.Alignment = 3;
            paragraph.SpacingAfter = 0;
            doc.Add(paragraph);
            paragraph = new Paragraph($"                          Должность и ФИО рабочего", tableFont);
            paragraph.Alignment = 3;
            paragraph.SpacingAfter = 20;
            doc.Add(paragraph);

            PdfPTable table = new PdfPTable(8);
            table.WidthPercentage = 100;
            table.SetWidths(new float[8] { 20, 100, 50, 60, 55, 60, 55, 50 });
            PdfPCell cell;
            //PdfPRow row;

            cell = new PdfPCell(new Phrase("№", tableFont));
            cell.BorderWidth = 1;
            cell.HorizontalAlignment = 1;
            // cell.Width = 40;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Наименование товара", tableFont));
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 1;
            //cell.Width = 40;
            table.AddCell(cell);


            cell = new PdfPCell(new Phrase("Кол-во", tableFont));
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 1;
            // cell.FilledWidth = 100;
            //cell.Width = 40;
            table.AddCell(cell);


            cell = new PdfPCell(new Phrase("Закупочная цена", tableFont));
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 1;
            //cell.Width = 40;
            table.AddCell(cell);


            cell = new PdfPCell(new Phrase("Сумма закупки (НДС 18%)", tableFont));
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 1;
            //cell.Width = 40;
            table.AddCell(cell);


            cell = new PdfPCell(new Phrase("Отпускная цена", tableFont));
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 1;
            //cell.Width = 40;
            table.AddCell(cell);


            cell = new PdfPCell(new Phrase("Сумма отпуска", tableFont));
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 1;
            //cell.Width = 40;
            table.AddCell(cell);


            cell = new PdfPCell(new Phrase("Прибыль", tableFont));
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 1;
            //cell.Width = 40;
            table.AddCell(cell);

            int id = 1;
            decimal totalProfit = 0;
            foreach (var item in DeliveryItems)
            {
                cell = new PdfPCell(new Phrase(id.ToString(), tableFont));
                cell.HorizontalAlignment = 0;
                cell.BorderWidth = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(item.Electronic.Model, tableFont));
                cell.HorizontalAlignment = 0;
                cell.BorderWidth = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(item.Count.ToString(), tableFont));
                cell.HorizontalAlignment = 0;
                cell.BorderWidth = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Math.Round(item.Electronic.PurchasePrice, 2).ToString(), tableFont));
                cell.HorizontalAlignment = 0;
                cell.BorderWidth = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Math.Round((item.Electronic.PurchasePrice * item.Count + (item.Electronic.PurchasePrice * item.Count * 18) / 100), 2).ToString(), tableFont));
                cell.HorizontalAlignment = 0;
                cell.BorderWidth = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Math.Round(item.Electronic.SalePrice, 2).ToString(), tableFont));
                cell.HorizontalAlignment = 0;
                cell.BorderWidth = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Math.Round((item.Electronic.SalePrice * item.Count), 2).ToString(), tableFont));
                cell.HorizontalAlignment = 0;
                cell.BorderWidth = 1;
                table.AddCell(cell);

                decimal currentProfit = Math.Round(item.Electronic.SalePrice * item.Count, 2) - (item.Electronic.PurchasePrice * item.Count + (item.Electronic.PurchasePrice * item.Count * 18) / 100);
                totalProfit += currentProfit;
                cell = new PdfPCell(new Phrase(Math.Round(currentProfit, 2).ToString(), tableFont));
                cell.HorizontalAlignment = 0;
                cell.BorderWidth = 1;
                table.AddCell(cell);

                id++;
            }

            cell = new PdfPCell(new Phrase("Итого:", tableFont));
            cell.HorizontalAlignment = 0;
            cell.BorderWidth = 1;
            cell.Colspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(DeliveryItems.Sum(p => p.Count).ToString(), tableFont));
            cell.HorizontalAlignment = 0;
            cell.BorderWidth = 1;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase());
            cell.HorizontalAlignment = 0;
            cell.BorderWidth = 1;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Math.Round(DeliveryItems.Sum(p => p.TotalPriceWithNDS), 2).ToString(), tableFont));
            cell.HorizontalAlignment = 0;
            cell.BorderWidth = 1;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase());
            cell.HorizontalAlignment = 0;
            cell.BorderWidth = 1;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Math.Round(DeliveryItems.Sum(p => p.Electronic.SalePrice * p.Count), 2).ToString(), tableFont));
            cell.HorizontalAlignment = 0;
            cell.BorderWidth = 1;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Math.Round(totalProfit, 2).ToString(), tableFont));
            cell.HorizontalAlignment = 0;
            cell.BorderWidth = 1;
            table.AddCell(cell);

            doc.Add(table);



            doc.Close();
            writer.Close();
            fs.Close();
        }

        private void RemoveProduct(object obj)
        {
            if (SelectedDeliveryItem != null)
            {
                DeliveryItems.Remove(SelectedDeliveryItem);
            }
        }
        private string GenerateDeliveryNumber()
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

        private void UnloadToStorage(object obj)
        {
            throw new NotImplementedException();
        }

        private void Back(object obj)
        {
            DialogResult = false;
        }

        private async void CancelDelivery(object obj)
        {
            var result = CustomMessageBox.Show("Вы точно хотите отменить заказ?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Delivery.StatusId = 4;
                var response = await ApiService.PutRequest("api/Deliveries", Delivery.DelivertId, Delivery);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    DialogResult = true;
                    CustomMessageBox.Show($"Заказа поставщику № {Delivery.DeliveryNumber} отменён.", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    CustomMessageBox.Show($"{response.Content}", "Произошла ошибка при изменении!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void PayDelivery(object obj)
        {
            var result = CustomMessageBox.Show("Оплатить заказ?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Delivery.StatusId = 5;
                var response = await ApiService.PutRequest("api/Deliveries", Delivery.DelivertId, Delivery);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    DialogResult = true;
                    CustomMessageBox.Show($"Заказа поставщику № {Delivery.DeliveryNumber} оплачен.", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    CustomMessageBox.Show($"{response.Content}", "Произошла ошибка при изменении!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeliveryItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            int id = 1;
            foreach (var item in DeliveryItems)
            {
                if (item.Id != id)
                {
                    item.Id = id;
                }
                id++;
            }
        }
    }
}
