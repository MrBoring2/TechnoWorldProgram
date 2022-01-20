using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TechnoWorld_Programm.Models;

namespace TechnoWorld_Programm.Services
{
    public class ClientService : INotifyPropertyChanged
    {
        private static ClientService instance;

        private ObservableCollection<CartItem> cart;

        private ClientService() { Cart = new ObservableCollection<CartItem>(); }
        public RestClient RestClient { get; set; }
        public static ClientService Instance 
        { 
            get
            {
                if (instance == null)
                    instance = new ClientService();
                return instance;
            } 
        }
        public int UserId { get; set; }
        public string UserFIO { get; set; }
        public ObservableCollection<CartItem> Cart { get => cart; set { cart = value;  } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
