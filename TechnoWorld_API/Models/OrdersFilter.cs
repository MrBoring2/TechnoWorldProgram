using Newtonsoft.Json;
using System;
using TechoWorld_DataModels_v2;

namespace TechnoWorld_API.Models
{
    public class OrdersFilter : FilterModel<Order>
    {
        [JsonProperty("search")]
        public string Search { get; set; }
        [JsonProperty("statusId")]
        public int StatusId { get; set; }
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("sortParameter")]
        public string SortParameter { get; set; }
        [JsonProperty("isAscending")]
        public bool IsAscending { get; set; }
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("itemsPerPage")]
        public int ItemsPerPage { get; set; }
        public override Func<Order, bool> FilterExpression
        {
            get
            {
                return p =>
                {
                    bool res1 = false;
                    bool res2 = false;
                    bool res3 = false;

                    res1 = p.OrderNumber.ToLower().Contains(Search.ToLower());

                    if (!res1)
                    {
                        return false;
                    }

                    res2 = p.StatusId == StatusId;
                    res3 = p.DateOfRegistration >= StartDate.ToLocalTime() && p.DateOfRegistration <= EndDate.ToLocalTime();

                    return res1 && res2 && res3;
                };



            }
        }
    }
}
