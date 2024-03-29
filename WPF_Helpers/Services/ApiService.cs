﻿using MaterialNotificationLibrary;
using MaterialNotificationLibrary.Enums;
using Microsoft.AspNetCore.SignalR.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_VM_Abstractions
{
    public class ApiService
    {
        private static ApiService instance;
        private RestClient restClient;
        private HubConnection hubConnection { get; set; }
        private ApiService()
        {
            try
            {
                restClient = new RestClient(apiUrl);
                restClient.Timeout = 200000000;
                restClient.ReadWriteTimeout = 20000000;
                restClient.CookieContainer = new System.Net.CookieContainer();
                hubConnection = new HubConnectionBuilder()
                    .WithUrl($"{apiUrl}technoWorldHub",
                    options =>
                    {
                        var a = restClient.CookieContainer;
                        options.AccessTokenProvider = () => Task.FromResult(restClient.CookieContainer.GetCookies(new Uri(apiUrl))[".AspNetCore.Application.Id"].Value);
                    })
                    .Build();
            }
            catch (Exception ex)
            {
                MaterialNotification.Show("Ошибка, в конфиг файле установлен неверный URI", $"{ex.Message}", MaterialNotificationButton.Ok, MaterialNotificationImage.Error);               
            }

        }
        public static ApiService Instance => instance ?? (instance = new ApiService());
        public static readonly string apiUrl = ConfigurationManager.AppSettings["ApiConnection"].ToString();
        public RestClient GetRestClient => restClient;
        public HubConnection GetHubConnection => hubConnection;
        public async void ShutDownService()
        {
            await hubConnection.StopAsync();
        }
        public void RemoveRestClient()
        {
            restClient = null;
            hubConnection = null;
            instance = null;
        }
        public Task<IRestResponse> AuthorizeTerminal()
        {
            try
            {
                RestRequest request = new RestRequest($"{apiUrl}terminalToken", Method.POST);
                request.AddJsonBody(new { roleName = "terminalUser", terminalName = $"terminal_{Guid.NewGuid()}" });
                var response = restClient.ExecuteAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Task<IRestResponse> AuthorizeInWarehouse(string login, string hashPass)
        {
            try
            {
                RestRequest request = new RestRequest($"{apiUrl}userToken", Method.POST);
                request.AddJsonBody(new { userName = login, hashPass = hashPass, programm = "warehouse_accounting" });
                var response = restClient.ExecuteAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Task<IRestResponse> AuthorizeInCash(string login, string hashPass)
        {
            try
            {
                RestRequest request = new RestRequest($"{apiUrl}userToken", Method.POST);
                request.AddJsonBody(new { userName = login, hashPass = hashPass, programm = "cash" });
                var response = restClient.ExecuteAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<IRestResponse> GetRequest(string url)
        {
            var response = await restClient.ExecuteAsync(CreateRequest(url, Method.GET));
            return response;
        }
        public async Task<IRestResponse> GetRequestWithParameter(string url, string parameterName, object parameter)
        {
            var response = await restClient.ExecuteAsync(CreateRequestWithParameter(url, Method.GET, parameterName, parameter));
            return response;
        }
        public async Task<IRestResponse> GetRequest(string url, int id)
        {
            var response = await restClient.ExecuteAsync(CreateRequest(url, Method.GET, id));
            return response;
        }

        public async Task<IRestResponse> PostRequest(string url, object data)
        {
            var response = await restClient.ExecuteAsync(CreateRequest(url, Method.POST, data));
            return response;
        }
        public async Task<IRestResponse> PutRequest(string url, int id, object data)
        {
            var response = await restClient.ExecuteAsync(CreateRequest($"{url}/{id}", Method.PUT, data));
            return response;
        }
        public async Task<IRestResponse> PutRequest(string url, int id)
        {
            var response = await restClient.ExecuteAsync(CreateRequest($"{url}/{id}", Method.PUT));
            return response;
        }
        public async Task<IRestResponse> DeleteRequest(string url, int id)
        {
            var response = await restClient.ExecuteAsync(CreateRequest($"{url}/{id}", Method.DELETE));
            return response;
        }
        private IRestRequest CreateRequest(string url, Method httpMethod)
        {
            var restReqeust = new RestRequest(url, httpMethod);
            //restReqeust.AddHeader("Authorization", "Bearer " + ClientService.Instance.Token);
            restReqeust.AddParameter("culture", "ru-RU");
            return restReqeust;
        }

        private IRestRequest CreateRequest(string url, Method httpMethod, int id)
        {
            var restReqeust = new RestRequest($"{url}/{id}", httpMethod);
            //restReqeust.AddHeader("Authorization", "Bearer " + ClientService.Instance.Token);
            restReqeust.AddParameter("culture", "ru-RU");
            return restReqeust;
        }
        private IRestRequest CreateRequestWithParameter(string url, Method httpMethod, string parameterName, object parameter)
        {
            var restReqeust = new RestRequest(url, httpMethod);
            //restReqeust.AddHeader("Authorization", "Bearer " + ClientService.Instance.Token);
            restReqeust.AddParameter(parameterName, parameter);
            restReqeust.AddParameter("culture", "ru-RU");
            return restReqeust;
        }
        private IRestRequest CreateRequest(string url, Method httpMethod, object data)
        {
            var restReqeust = new RestRequest(url, httpMethod);
            //restReqeust.AddHeader("Authorization", "Bearer " + ClientService.Instance.Token);
            restReqeust.AddJsonBody(data);
            restReqeust.AddParameter("culture", "ru-RU");
            return restReqeust;
        }
    }
}
