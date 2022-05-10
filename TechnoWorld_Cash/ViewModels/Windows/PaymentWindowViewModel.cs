using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TechnoWorld_Cash.Models;
using TechnoWorld_Cash.Views.Windows;
using TechnoWorld_Notification;
using TechnoWorld_Notification.Enums;
using TechnoWorld_Terminal.Services;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_VM_Abstractions;
using Word = Microsoft.Office.Interop.Word;
namespace TechnoWorld_Cash.ViewModels.Windows
{
    public class PaymentWindowViewModel : BaseModalWindowVM
    {
        private bool isEnabled;

        private string orderNumber;
        private DateTime orderDate;
        private string sellerPerson;
        private string paymentMethod;
        private decimal totalPrice;
        private bool isNal;
        private bool isBesNal;

        private ObservableCollection<OrderItem> orderItems;
        public PaymentWindowViewModel(Order order)
        {
            Order = order;
            OrderItems = new ObservableCollection<OrderItem>();
            int id = 1;
            foreach (var item in Order.OrderElectronics)
            {

                OrderItems.Add(new OrderItem(id, item.Electronics.Model, item.Count, item.Electronics.SalePrice, item.Count * item.Electronics.SalePrice));
                id++;
            }

            TotalPrice = OrderItems.Sum(p => p.TotalPrice);

            PaymentMethod = "Безналичный";
            OrderNumber = order.OrderNumber;
            OrderDate = order.DateOfRegistration;
            SellerPerson = ClientService.Instance.User.FullName;
            IsNal = false;
            IsBesnal = true;
            PayCommand = new RelayCommand(Pay);
            CancelCommand = new RelayCommand(Cancel);
            ChangePaymentMethodToNalCommand = new RelayCommand(ChangePaymentMethodToNal);
            ChangePaymentMethodToBesnalCommand = new RelayCommand(ChangePaymentMethodToBesnal);
        }


        private Order Order { get; set; }

        public string OrderNumber { get { return orderNumber; } set { orderNumber = value; OnPropertyChanged(); } }
        public DateTime OrderDate { get { return orderDate; } set { orderDate = value; OnPropertyChanged(); } }
        public string SellerPerson { get { return sellerPerson; } set { sellerPerson = value; OnPropertyChanged(); } }
        public decimal TotalPrice { get { return totalPrice; } set { totalPrice = value; OnPropertyChanged(); } }
        public bool IsEnabled { get => isEnabled; set { isEnabled = value; OnPropertyChanged(); } }
        public bool IsNal { get { return isNal; } set { isNal = value; OnPropertyChanged(); } }
        public ObservableCollection<OrderItem> OrderItems { get { return orderItems; } set { orderItems = value; OnPropertyChanged(); } }
        public bool IsBesnal { get => isBesNal; set { isBesNal = value; OnPropertyChanged(); } }
        public string PaymentMethod { get { return paymentMethod; } set { paymentMethod = value; OnPropertyChanged(); } }
        public RelayCommand PayCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand ChangePaymentMethodToNalCommand { get; set; }
        public RelayCommand ChangePaymentMethodToBesnalCommand { get; set; }
        private async void Pay(object obj)
        {
            await Task.Run(() =>
            {
                var app = new Word.Application();
                Word.Document document = app.Documents.Add();

                Word.Paragraph title = document.Paragraphs.Add();
                title.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                title.Range.Font.Size = 25;
                title.Range.Text = $"Чек";
                title.Range.InsertParagraphAfter();

                var paragraph = document.Paragraphs.Add();
                paragraph.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                paragraph.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                paragraph.Range.Font.Size = 20;
                paragraph.Range.Text = $"Номер заказа: {Order.OrderNumber}";
                paragraph.Range.InsertParagraphAfter();

                paragraph = document.Paragraphs.Add();
                paragraph.Range.Font.Size = 16;
                paragraph.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                paragraph.Range.Text = $"Дата заказа: {Order.DateOfRegistration}";
                paragraph.Range.InsertParagraphAfter();

                paragraph = document.Paragraphs.Add();
                paragraph.Range.Font.Size = 16;
                paragraph.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                paragraph.Range.Text = $"Компания продавец: ООО «Техно-мир»\tКассир: {SellerPerson}";
                paragraph.Range.InsertParagraphAfter();

                var tableParagrath = document.Paragraphs.Add();
                tableParagrath.Range.Font.Size = 16;
                tableParagrath.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                tableParagrath.Range.Text = $"Товары: ";
                Word.Table table = document.Tables.Add(tableParagrath.Range, OrderItems.Count() + 1, 5);
                table.Borders.InsideLineStyle = table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                Word.Range cellRange;
                cellRange = table.Cell(1, 1).Range;
                cellRange.Text = "№";
                cellRange = table.Cell(1, 2).Range;
                cellRange.Text = "Название";
                cellRange = table.Cell(1, 3).Range;
                cellRange.Text = "Количество";
                cellRange = table.Cell(1, 4).Range;
                cellRange.Text = "Стоимость за 1";
                cellRange = table.Cell(1, 5).Range;
                cellRange.Text = "Общая стоимость";

                table.Rows[1].Range.Bold = 1;
                table.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                int index = 1;
                foreach (var item in OrderItems)
                {
                    cellRange = table.Cell(index + 1, 1).Range;
                    cellRange.Text = item.Number.ToString();

                    cellRange = table.Cell(index + 1, 2).Range;
                    cellRange.Text = item.Name.ToString();

                    cellRange = table.Cell(index + 1, 3).Range;
                    cellRange.Text = item.Count.ToString() + " шт.";

                    cellRange = table.Cell(index + 1, 4).Range;
                    cellRange.Text = item.PriceForOne.ToString() + " руб.";

                    cellRange = table.Cell(index + 1, 5).Range;
                    cellRange.Text = item.TotalPrice.ToString() + " руб.";
                    index++;
                }
                tableParagrath.Range.InsertParagraphAfter();

                var totalPriceParagraph = document.Paragraphs.Add();
                totalPriceParagraph.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                totalPriceParagraph.Range.Font.Size = 25;
                totalPriceParagraph.Range.Text = $"Всего: {OrderItems.Sum(p => p.TotalPrice)} руб.";

                document.SaveAs2(AppDomain.CurrentDomain.BaseDirectory + $@"Чеки/Чек №{Order.OrderNumber}.pdf", Word.WdExportFormat.wdExportFormatPDF);

            });
            Order.StatusId = 2;
            Order.EmployeeId = ClientService.Instance.User.UserId;
            var response = await ApiService.Instance.PutRequest("api/Orders", Order.OrderId, Order);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                MaterialNotification.Show("Оповещение", $"Заказ усешно оплачен.", MaterialNotificationButton.Ok, MaterialNotificationImage.Susccess);
                DialogResult = true;
            }

        }
        private void Cancel(object obj)
        {
            DialogResult = false;
        }

        private void ChangePaymentMethodToNal(object obj)
        {
            IsBesnal = false;
            IsNal = true;
            PaymentMethod = "Наличный";
        }
        private void ChangePaymentMethodToBesnal(object obj)
        {
            IsBesnal = true;
            IsNal = false;
            PaymentMethod = "Безналичный";
        }

    }
}
