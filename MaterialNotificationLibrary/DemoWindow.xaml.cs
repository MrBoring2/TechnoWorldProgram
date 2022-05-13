using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MaterialNotificationLibrary.Enums;

namespace MaterialNotificationLibrary
{
    /// <summary>
    /// Логика взаимодействия для DemoWindow.xaml
    /// </summary>
    public partial class DemoWindow : Window
    {
        public DemoWindow()
        {
            InitializeComponent();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            MaterialNotification.Show("Заколовок", "Текст сообщенияТекст сообщенияТекст сообщенияТекст сообщенияТекст сообщенияТекст сообщенияТекст сообщения", MaterialNotificationButton.Ok, MaterialNotificationImage.Information);
        }

        private void Warn_Click(object sender, RoutedEventArgs e)
        {
            MaterialNotification.Show("Внимание", "Текст сообщенияТекст сообщенияТекст сообщенияТекст сообщенияТекст сообщенияТекст сообщенияТекст сообщения", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
        }

        private void Error_Click(object sender, RoutedEventArgs e)
        {
            MaterialNotification.Show("Ошибка", "Текст сообщенияТекст сообщенияТекст сообщенияТекст сообщенияТекст сообщенияТекст сообщенияТекст сообщения", MaterialNotificationButton.OkCancel, MaterialNotificationImage.Error);
        }

        private void Question_Click(object sender, RoutedEventArgs e)
        {
            MaterialNotification.Show("Подтверждение", "Текст сообщенияТекст сообщенияТекст сообщенияТекст сообщенияТекст сообщенияТекст сообщенияТекст сообщения", MaterialNotificationButton.YesNo, MaterialNotificationImage.Question);
        }

        private void Success_Click(object sender, RoutedEventArgs e)
        {
            MaterialNotification.Show("Успешно", "Текст сообщенияТекст сообщенияТекст сообщенияТекст сообщенияТекст сообщенияТекст сообщенияТекст сообщения", MaterialNotificationButton.Ok, MaterialNotificationImage.Susccess);
        }
    }
}
