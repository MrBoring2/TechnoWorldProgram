using MaterialDesignExtensions.Controls;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TechnoWorld_Notification.Enums;

namespace TechnoWorld_Notification
{
    /// <summary>
    /// Логика взаимодействия для MaterialNotification.xaml
    /// </summary>
    public partial class MaterialNotification : MaterialWindow, INotifyPropertyChanged
    {
        private List<CancelEventHandler> _preCloseEvents = new List<CancelEventHandler>();

        private string msgTitle;
        private string msgContent;
        private bool _isCloseable = true;

        private MaterialNotificationButton msgButton = MaterialNotificationButton.Ok;
        private MaterialNotificationImage msgImage = MaterialNotificationImage.Information;

        public MaterialNotification()
        {
            InitializeComponent();
            OkYesButton.Focus();
            DataContext = this;
        }
        public MaterialNotification(string title, string message) : this()
        {
            MsgTitle = title;
            MsgContent = message;
        }
        /// <summary>
        /// Создаёт объект MaterialNotification с заголовком, текстом, кнопками и иконкой
        /// </summary>
        /// <param name="title">Заголовок сообщения</param>
        /// <param name="message">Текст сообщения</param>
        /// <param name="msgButton">Определяет, что за кнопка будут появляться: Ok, OkCancel или YesNo</param>
        /// <param name="msgImage">Определяет, что за иконка будет на окне: None, Information, Success, Warning, Error или Question</param>
        public MaterialNotification(string title, string message, MaterialNotificationButton msgButton, MaterialNotificationImage msgImage) : this()
        {
            MsgTitle = title;
            MsgContent = message;
            MsgButton = msgButton;
            MsgImage = msgImage;

            switch (msgImage)
            {
                case MaterialNotificationImage.None:
                    SystemSounds.Asterisk.Play();
                    break;
                case MaterialNotificationImage.Information:
                    SystemSounds.Asterisk.Play();
                    break;
                case MaterialNotificationImage.Susccess:
                    SystemSounds.Asterisk.Play();
                    break;
                case MaterialNotificationImage.Warning:
                    SystemSounds.Hand.Play();
                    break;
                case MaterialNotificationImage.Error:
                    SystemSounds.Hand.Play();
                    break;
                case MaterialNotificationImage.Question:
                    SystemSounds.Beep.Play();
                    break;
                default:
                    break;
            }

        }

        public static void Show(string v1, string v2, object matetialNotificationButton)
        {
            throw new NotImplementedException();
        }

        public string MsgTitle { get => msgTitle; set { msgTitle = value; OnPropertyChanged(); } }
        public string MsgContent { get => msgContent; set { msgContent = value; OnPropertyChanged(); } }
        public MaterialNotificationButton MsgButton { get => msgButton; set { msgButton = value; OnPropertyChanged(); } }
        public MaterialNotificationImage MsgImage { get => msgImage; set { msgImage = value; OnPropertyChanged(); } }
        public MaterialNotificationResult Result { get; private set; } = MaterialNotificationResult.Cancel;
        public SolidColorBrush BtnBackground { get; private set; }
        public SolidColorBrush BtnForeGround { get; private set; } 
        public Thickness BtnBorderThicness { get; private set; }
        public SolidColorBrush BtnBorderColor { get; private set; }
        /// <summary>
        /// Вывов всплывающего окна с заголовком, текстом, кнопками и иконкой
        /// </summary>
        /// <param name="title">Заголовок сообщения</param>
        /// <param name="message">Текст сообщения</param>
        /// <param name="msgButton">Определяет, что за кнопка будут появляться: Ok, OkCancel или YesNo</param>
        /// <param name="msgImage">Определяет, что за иконка будет на окне: None, Information, Success, Warning, Error или Question</param>
        /// <returns>Возвращает резульат в виде OK или Cancel</returns>
        public static MaterialNotificationResult Show(string title, string message, MaterialNotificationButton msgButton = MaterialNotificationButton.Ok, MaterialNotificationImage msgImage = MaterialNotificationImage.None)
        {
            var alert = new MaterialNotification(title, message, msgButton, msgImage);
            return alert.ShowDialog();
        }

        public void SetButtonColors(SolidColorBrush btnBackground, SolidColorBrush btnForeGround, Thickness btnBorderThicness, SolidColorBrush btnBorderColor)
        {
            BtnBackground = btnBackground;
            BtnForeGround = btnForeGround;
            BtnBorderColor = btnBorderColor;
            BtnBorderThicness = btnBorderThicness;

            OkYesButton.Background = btnBackground;
            OkYesButton.Foreground = btnForeGround;
            OkYesButton.BorderBrush = btnBorderColor;
            OkYesButton.BorderThickness = btnBorderThicness;
            CancelNoButton.Background = btnBackground;
            CancelNoButton.Foreground = btnForeGround;
            CancelNoButton.BorderBrush = btnBorderColor;
            CancelNoButton.BorderThickness = btnBorderThicness;
        }

        /// <summary>
        /// Вызывает окно как диалог
        /// </summary>
        /// <returns>Возвращает результат в виде Ok или Cancel</returns>
        public new MaterialNotificationResult ShowDialog()
        {
            base.ShowDialog();
            return Result;
        }


        private void Event_HideAnimation_Completed(object sender, EventArgs e)
        {
            if (_isCloseable)
            {
                return;
            }

            Close();
        }

        private void Event_Closing(object sender, CancelEventArgs e)
        {
            var hideAnimation = (this.FindResource("HideAnimation") as Storyboard).Clone();
            if (hideAnimation == null || !_isCloseable)
            {
                return;
            }

            e.Cancel = true;

            if (!RaiseCloseEvent())
            {
                _isCloseable = true;
                return;
            }

            _isCloseable = false;

            hideAnimation.Completed += Event_HideAnimation_Completed;
            hideAnimation.Begin(_Dialog);
        }
        public event CancelEventHandler PreClose
        {
            add => _preCloseEvents.Add(value);
            remove => _preCloseEvents.Remove(value);
        }
        private bool RaiseCloseEvent()
        {
            var args = new CancelEventArgs();
            foreach (var preCloseEvent in _preCloseEvents)
            {
                preCloseEvent(this, args);

                if (args.Cancel)
                {
                    return false;
                }
            }

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void OkYesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isCloseable)
            {
                return;
            }

            Result = MaterialNotificationResult.Ok;
            Close();
        }
        private void CanelNoButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isCloseable)
            {
                return;
            }

            Result = MaterialNotificationResult.Cancel;
            Close();
        }
    }
}
