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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BNS_Programm.CustomElements
{
    /// <summary>
    /// Логика взаимодействия для UserControlMenuItem.xaml
    /// </summary>
    public partial class UserControlMenuItem : UserControl
    {
       // private MainAppWindow _context;
        //public UserControlMenuItem(ItemMenu itemMenu, MainAppWindow context)
        //{
        //    InitializeComponent();

        //    _context = context;

        //    ExpanderMenu.Visibility = itemMenu.SubItems == null ? Visibility.Hidden : Visibility.Visible;
        //    ListViewItemMenu.Visibility = itemMenu.SubItems == null ? Visibility.Visible : Visibility.Hidden;

        //    this.DataContext = itemMenu;
        //}

     
        private void ListViewItemMenu_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (ListViewMenu == null)
            //    return;
            
            //if((DataContext as ItemMenu).Header.Equals("Выход"))
            //{
            //    Application.Current.Shutdown();
            //    return;
            //}

            //_context.SwitchPage((DataContext as ItemMenu).Page);
        }
    }
}
