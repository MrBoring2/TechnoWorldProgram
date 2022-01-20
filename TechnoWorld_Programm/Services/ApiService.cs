﻿using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoWorld_Programm.Services
{
    public class ApiService
    {
        public const string apiUrl = "http://localhost:29320/";
        public async static Task<IRestResponse> GetRequest(string url)
        {
            var response = await ClientService.Instance.RestClient.ExecuteAsync(CreateRequest(url, Method.GET));
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
            return new RestRequest(url, httpMethod);
        }

        private static IRestRequest CreateRequest(string url, Method httpMethod, int id)
        {
            return new RestRequest($"{url}/{id}", httpMethod);
        }

        private static IRestRequest CreateRequest(string url, Method httpMethod, object data)
        {
            return new RestRequest(url, httpMethod).AddJsonBody(data);
        }
    }
}
