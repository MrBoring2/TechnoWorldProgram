
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_Terminal.Services;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;

namespace TechnoWorld_Terminal.Models
{
    public class CartItem : NotifyPropertyChangedModel
    {
        private decimal totalPrice;
        private int amount;
        public CartItem(Electronic electronic)
        {
            Electronic = electronic;
            Amount = 1;
        }

        public Electronic Electronic { get; private set; }
        public string ElectronicType => Electronic.Type.Name;
        public string ElectronicModel => Electronic.Model;
        public byte[] ElectronicImage => Electronic.Image;
        public int Amount
        {
            get => amount;
            set
            {
                if (value <= 0)
                {
                    var content = new NotificationContent
                    {
                        Message = $"Вы точно хотите удалить товар {ElectronicModel} из корзины?",
                        Title = "Подтверждение",
                        Type = NotificationType.Information,
                        LeftButtonContent = "Да",
                        RightButtonContent = "Нет",
                        RightButtonAction = () => { },
                        LeftButtonAction = () =>
                        {
                            amount = value;
                            ClientService.Instance.Cart.Remove(ClientService.Instance.Cart.FirstOrDefault(p => p.Electronic.ElectronicsId == this.Electronic.ElectronicsId));
                        }
                    };
                    CustomNotificationManager.ShowNotification(content, "ResultNotiifcationArea");
                }
                else
                {
                    amount = value;
                    TotalPrice = amount * SalePrice;
                }
                OnPropertyChanged();
            }
        }
        public decimal SalePrice => Electronic.SalePrice;
        public decimal TotalPrice
        {
            get => totalPrice;
            set
            {
                totalPrice = value;
                OnPropertyChanged();
            }
        }

        public void SetAmount(int amount)
        {
            if (amount <= 0)
            {
                var notificationManager = new NotificationManager();
                notificationManager.Show(new NotificationContent
                {
                    Message = $"Вы точно хотите удалить товар {ElectronicModel} из корзины?",
                    Title = "Подтверждение",
                    Type = NotificationType.Information
                }, areaName: "MainNotificationArea");
            }
            else this.Amount = amount;
        }
    }
}
