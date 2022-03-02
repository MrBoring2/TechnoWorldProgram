using Microsoft.AspNetCore.SignalR.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TechnoWorld_Cash.Models;

namespace TechnoWorld_Terminal.Services
{
    public class ClientService : INotifyPropertyChanged
    {

        private static ClientService instance;
        public string Token { get; set; }

        private ClientService() { }
        public RestClient RestClient { get; set; }
        public HubConnection HubConnection { get; set; }
        public static ClientService Instance
        {
            get
            {
                if (instance == null)
                    instance = new ClientService();
                return instance;
            }
        }
        public User User { get; private set; }
        public int UserId { get; set; }
        public string UserFIO { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public void SetClient(string user_name, string full_name, int role_id, int user_id, string token)
        {
            Token = token;
            User = new User()
            {
                Name = user_name,
                RoleId = role_id,
                UserId = user_id,
                FullName = full_name
                
            };
        }

        public void Logout()
        {
            User = null;
            Token = null;
            HubConnection.StopAsync();
            RestClient = null;
        }
    }
}
