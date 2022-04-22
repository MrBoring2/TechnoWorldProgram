using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
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

namespace TechnoWorld_WarehouseAccounting.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : BaseWindow, INotifyPropertyChanged
    {
        #region Fields
        private string message;
        private string header;
        private PackIconKind image;
        private Brush color;
        private MessageBoxResult Result = MessageBoxResult.None;

        public event PropertyChangedEventHandler PropertyChanged;
      
        #endregion;

        public CustomMessageBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Properties
        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged();
            }
        }
        public string Header
        {
            get => header;
            set
            {
                header = value;
                OnPropertyChanged();
            }
        }
        public PackIconKind Image
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged();
            }
        }
        public Brush Color
        {
            get => color;
            set
            {
                color = value;
                OnPropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// Добавление кнопок на модальное окно в зависимости от типа
        /// </summary>
        /// <param name="button"></param>
        private void AddButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OK:
                    AddButton("OK", MessageBoxResult.OK);
                    break;
                case MessageBoxButton.OKCancel:
                    AddButton("OK", MessageBoxResult.OK);
                    AddButton("Отмена", MessageBoxResult.Cancel, isCancel: true);
                    break;
                case MessageBoxButton.YesNo:
                    AddButton("Да", MessageBoxResult.Yes);
                    AddButton("Нет", MessageBoxResult.No);
                    break;
                case MessageBoxButton.YesNoCancel:
                    AddButton("Да", MessageBoxResult.Yes);
                    AddButton("Нет", MessageBoxResult.No);
                    AddButton("Отмена", MessageBoxResult.Cancel, isCancel: true);
                    break;
                default:
                    throw new ArgumentException("Неизместное значение кнопки", "button");
            }
        }

        /// <summary>
        /// Установка значений в модалбное окно в зависимости от типа
        /// </summary>
        /// <param name="image"></param>
        private void SetData(MessageBoxImage image)
        {
            switch (image)
            {
                case MessageBoxImage.Error:
                    {
                        Image = PackIconKind.AlertOutline;
                        Color = (SolidColorBrush)new BrushConverter().ConvertFrom("#E51400");
                        SystemSounds.Hand.Play();
                    }
                    break;
                case MessageBoxImage.Information:
                    {
                        Image = PackIconKind.InformationOutline;
                        Color = (SolidColorBrush)new BrushConverter().ConvertFrom("#03A9F4");
                        SystemSounds.Asterisk.Play();
                    }
                    break;
                case MessageBoxImage.Warning:
                    {
                        Image = PackIconKind.ShieldAlertOutline;
                        Color = (SolidColorBrush)new BrushConverter().ConvertFrom("#F0A30A");
                        SystemSounds.Hand.Play();
                    }
                    break;
                case MessageBoxImage.Question:
                    {
                        Image = PackIconKind.MessageQuestionOutline;
                        Color = (SolidColorBrush)new BrushConverter().ConvertFrom("#03A9F4");
                        SystemSounds.Question.Play();
                    }
                    break;
                case MessageBoxImage.None:
                    {
                        Image = PackIconKind.None;
                        Color = (SolidColorBrush)new BrushConverter().ConvertFrom("#03A9F4");
                        SystemSounds.Asterisk.Play();
                    }
                    break;
                default:
                    {
                        Image = PackIconKind.InformationOutline;
                        Color = (SolidColorBrush)new BrushConverter().ConvertFrom("#03A9F4");
                        SystemSounds.Asterisk.Play();
                    }
                    break;
            }
        }

        /// <summary>
        /// Добавление конкретной кнопка
        /// </summary>
        /// <param name="content"></param>
        /// <param name="result"></param>
        /// <param name="isCancel"></param>
        private void AddButton(string content, MessageBoxResult result, bool isCancel = false)
        {
            var button = new Button() { Content = content, IsCancel = isCancel, Foreground = Brushes.White, Style = Application.Current.Resources["btn"] as Style, Width=100, Height=40, Margin = new Thickness(5) };
            button.Click += (o, e) => { Result = result; DialogResult = true; };
            Buttons.Children.Add(button);
        }
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(prop, new PropertyChangedEventArgs(prop));
        }
        /// <summary>
        /// Вызов окна в соответствии с параметрами
        /// </summary>
        /// <param name="message"></param>
        /// <param name="header"></param>
        /// <param name="buttons"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public static MessageBoxResult Show(string message, string header, MessageBoxButton buttons, MessageBoxImage image)
        {
            var dialog = new CustomMessageBox() { Header = header, Message = message, WindowStartupLocation = WindowStartupLocation.CenterScreen };
            dialog.SetData(image);
            dialog.AddButtons(buttons);
            dialog.ShowDialog();
            return dialog.Result;
        }
    }
}
