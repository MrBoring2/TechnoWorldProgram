using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
                OrderItems.Add(new OrderItem(id, item.Electronics, item.Count));
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

        private void GenerateCheque(string orderNumber, Order orderm, string cashUserFullName, IEnumerable<OrderItem> otderItems)
        {
            var chequeNumber = $"{orderNumber}";
            FileStream fs = new FileStream($"Чеки/{chequeNumber}.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            Rectangle rec2 = new Rectangle(PageSize.A4);


            Document doc = new Document(rec2, 50, 50, 10, 10);

            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            string ttf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
            var baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            var textFont = new iTextSharp.text.Font(baseFont, 14, iTextSharp.text.Font.NORMAL);
            var tableFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.NORMAL);
            var titleFont = new iTextSharp.text.Font(baseFont, 21, iTextSharp.text.Font.NORMAL);
            var bigTitleFont = new iTextSharp.text.Font(baseFont, 26, iTextSharp.text.Font.NORMAL);
            writer.SetLanguage("ru-RU");
            doc.AddAuthor($"{ClientService.Instance.User.FullName}");
            doc.AddCreator("TechnoWorld_Cash using iTextSharp");
            doc.AddKeywords("PDF чек");
            doc.AddSubject("Чек");
            doc.AddTitle($"Чек №{chequeNumber}");
            doc.Open();

            Paragraph paragraph;

            paragraph = new Paragraph($"ЧЕК", bigTitleFont);
            paragraph.Alignment = 1;
            //paragraph.ExtraParagraphSpace = 20;
            paragraph.Leading = 30;
            paragraph.SpacingAfter = 20;
            doc.Add(paragraph);

            paragraph = new Paragraph($"Заказ № {chequeNumber}", titleFont);
            paragraph.Alignment = 1;
            //paragraph.ExtraParagraphSpace = 20;
            paragraph.Leading = 30;
            paragraph.SpacingAfter = 20;
            doc.Add(paragraph);

            paragraph = new Paragraph($"Дата регистарции заказа: {Order.DateOfRegistration.ToLocalTime().ToShortDateString()}", textFont);
            paragraph.Alignment = 3;
            paragraph.SpacingAfter = 5;
            doc.Add(paragraph);
            paragraph = new Paragraph($"Дата оплаты: {DateTime.Now.ToLocalTime().ToShortDateString()}", textFont);
            paragraph.Alignment = 3;
            paragraph.SpacingAfter = 20;
            doc.Add(paragraph);


            paragraph = new Paragraph($"Компания продавец: ООО «Техно-мир»", textFont);
            paragraph.Alignment = 3;
            paragraph.SpacingAfter = 5;
            doc.Add(paragraph);

            paragraph = new Paragraph($"Кассир: {cashUserFullName}", textFont);
            paragraph.Alignment = 3;
            paragraph.SpacingAfter = 20;
            doc.Add(paragraph);

            paragraph = new Paragraph($"Товары", titleFont);
            paragraph.Alignment = 1;
            paragraph.SpacingAfter = 5;
            doc.Add(paragraph);

            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.SetWidths(new float[5] { 40, 100, 50, 60, 80 });
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


            cell = new PdfPCell(new Phrase("Цена за 1", tableFont));
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 1;
            //cell.Width = 40;
            table.AddCell(cell);


            cell = new PdfPCell(new Phrase("Сумма", tableFont));
            cell.HorizontalAlignment = 1;
            cell.BorderWidth = 1;
            //cell.Width = 40;
            table.AddCell(cell);

            int id = 1;
            decimal totalOrderPrice = 0;
            foreach (var item in orderItems)
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

                cell = new PdfPCell(new Phrase(Math.Round(item.Electronic.SalePrice, 2).ToString(), tableFont));
                cell.HorizontalAlignment = 0;
                cell.BorderWidth = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(Math.Round(item.TotalPrice, 2).ToString(), tableFont));
                cell.HorizontalAlignment = 0;
                cell.BorderWidth = 1;
                table.AddCell(cell);

                decimal currentPrice = item.TotalPrice;
                totalOrderPrice += currentPrice;
                id++;
            }

            cell = new PdfPCell(new Phrase("Итого:", tableFont));
            cell.HorizontalAlignment = 0;
            cell.BorderWidth = 1;
            cell.Colspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(orderItems.Sum(p => p.Count).ToString(), tableFont));
            cell.HorizontalAlignment = 0;
            cell.BorderWidth = 1;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase());
            cell.HorizontalAlignment = 0;
            cell.BorderWidth = 1;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Math.Round(totalOrderPrice, 2).ToString(), tableFont));
            cell.HorizontalAlignment = 0;
            cell.BorderWidth = 1;
            table.AddCell(cell);

            doc.Add(table);

            paragraph = new Paragraph($"Всего к оплате: {totalOrderPrice} руб.", textFont);
            paragraph.Alignment = 0;
            paragraph.SpacingAfter = 5;
            paragraph.SpacingBefore = 5;
            doc.Add(paragraph);

            doc.Close();
            writer.Close();
            fs.Close();

        }

        private async void Pay(object obj)
        {
            Order.StatusId = 2;
            Order.EmployeeId = ClientService.Instance.User.UserId;
            var response = await ApiService.Instance.PutRequest("api/Orders", Order.OrderId, Order);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await Task.Run(() => GenerateCheque(OrderNumber, Order, ClientService.Instance.User.FullName, OrderItems));
                MaterialNotification.Show("Оповещение", $"Заказ усешно оплачен.", MaterialNotificationButton.Ok, MaterialNotificationImage.Susccess);
                DialogResult = true;
            }
            else
            {
                MaterialNotification.Show("Произошла ошибка при оплате.", $"{response.Content}.", MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
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
