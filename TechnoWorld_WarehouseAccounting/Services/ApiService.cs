using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoWorld_WarehouseAccounting.Services
{
    public class ApiService
    {
        public const string apiUrl = "http://localhost:5000/";
        public static Task<IRestResponse> Authorize(string login, string password)
        {
            try
            {
                RestRequest request = new RestRequest($"{apiUrl}userToken", Method.POST);
                request.AddJsonBody(new { userName = login, password = password, programm = "warehouse_accounting" });
                var response = ClientService.Instance.RestClient.ExecuteAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async static Task<IRestResponse> GetRequest(string url)
        {
            var response = await ClientService.Instance.RestClient.ExecuteAsync(CreateRequest(url, Method.GET));
            return response;
        }
        public async static Task<IRestResponse> GetRequestWithParameter(string url, string parameterName, object parameter)
        {
            var response = await ClientService.Instance.RestClient.ExecuteAsync(CreateRequestWithParameter(url, Method.GET, parameterName, parameter));
            return response;
        }
        public static Task<IRestResponse> GetRequest(string url, int id)
        {
            var response = ClientService.Instance.RestClient.ExecuteAsync(CreateRequest(url, Method.GET, id));
            return response;
        }

        public async static Task<IRestResponse> PostRequest(string url, object data)
        {
            var response = await ClientService.Instance.RestClient.ExecuteAsync(CreateRequest(url, Method.POST, data));
            return response;
        }
        public async static Task<IRestResponse> PutRequest(string url, int id, object data)
        {
            var response = await ClientService.Instance.RestClient.ExecuteAsync(CreateRequest($"{url}/{id}", Method.PUT, data));
            return response;
        }

        private static IRestRequest CreateRequest(string url, Method httpMethod)
        {
            var restReqeust = new RestRequest(url, httpMethod);
            restReqeust.AddHeader("Authorization", "Bearer " + ClientService.Instance.Token);
            restReqeust.AddParameter("culture", "ru-RU");
            return restReqeust;
        }

        private static IRestRequest CreateRequest(string url, Method httpMethod, int id)
        {
            var restReqeust = new RestRequest($"{url}/{id}", httpMethod);
            restReqeust.AddHeader("Authorization", "Bearer " + ClientService.Instance.Token);
            restReqeust.AddParameter("culture", "ru-RU");
            return restReqeust;
        }
        private static IRestRequest CreateRequestWithParameter(string url, Method httpMethod, string parameterName, object parameter)
        {
            var restReqeust = new RestRequest(url, httpMethod);
            restReqeust.AddHeader("Authorization", "Bearer " + ClientService.Instance.Token);
            restReqeust.AddParameter(parameterName, parameter);
            restReqeust.AddParameter("culture", "ru-RU");
            return restReqeust;
        }
        private static IRestRequest CreateRequest(string url, Method httpMethod, object data)
        {
            var restReqeust = new RestRequest(url, httpMethod);
            restReqeust.AddHeader("Authorization", "Bearer " + ClientService.Instance.Token);
            restReqeust.AddJsonBody(data);
            restReqeust.AddParameter("culture", "ru-RU");
            return restReqeust;
        }
    }
}
