using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoWorld_Terminal.Services
{
    public class ApiService
    {
        public const string apiUrl = "http://172.20.1.165:5000/";
        public static Task<IRestResponse> Authorize()
        {
            try
            {
                RestRequest request = new RestRequest($"{apiUrl}terminalToken", Method.POST);
                request.AddJsonBody(new { roleName = "terminalUser", terminalName = $"terminal_{Guid.NewGuid()}" });
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
        public async static Task<IRestResponse> GetRequest(string url, int id)
        {
            var response = await ClientService.Instance.RestClient.ExecuteAsync(CreateRequest(url, Method.GET, id));
            return response;
        }

        public async static Task<IRestResponse> PostReqeust(string url, object data)
        {
            var response = await ClientService.Instance.RestClient.ExecuteAsync(CreateRequest(url, Method.POST, data));
            return response;
        }

        private static IRestRequest CreateRequest(string url, Method httpMethod)
        {
            var restReqeust = new RestRequest(url, httpMethod);
            restReqeust.AddHeader("Authorization", "Bearer " + ClientService.Instance.Token);
            return restReqeust;
        }

        private static IRestRequest CreateRequest(string url, Method httpMethod, int id)
        {
            var restReqeust = new RestRequest($"{url}/{id}", httpMethod);
            restReqeust.AddHeader("Authorization", "Bearer " + ClientService.Instance.Token);
            return restReqeust;
        }
        private static IRestRequest CreateRequestWithParameter(string url, Method httpMethod, string parameterName, object parameter)
        {
            var restReqeust = new RestRequest(url, httpMethod);
            restReqeust.AddHeader("Authorization", "Bearer " + ClientService.Instance.Token);
            restReqeust.AddParameter(parameterName, parameter);
            return restReqeust;
        }
        private static IRestRequest CreateRequest(string url, Method httpMethod, object data)
        {
            var restReqeust = new RestRequest(url, httpMethod);
            restReqeust.AddHeader("Authorization", "Bearer " + ClientService.Instance.Token);
            restReqeust.AddJsonBody(data);
            return restReqeust;
        }
    }
}
