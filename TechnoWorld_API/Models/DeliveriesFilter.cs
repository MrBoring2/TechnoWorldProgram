using Newtonsoft.Json;
using System;
using TechoWorld_DataModels;

namespace TechnoWorld_API.Models
{
    public class DeliveriesFilter : FilterModel<Delivery>
    {
        [JsonProperty("search")]
        public string Search { get; set; }
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("statusId")]
        public int StatusId { get; set; }
        [JsonProperty("sortParameter")]
        public string SortParameter { get; set; }
        [JsonProperty("isAscending")]
        public bool IsAscending { get; set; }
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }
        [JsonProperty("itemsPerPage")]
        public int ItemsPerPage { get; set; }

        public override Func<Delivery, bool> FilterExpression
        {
            get
            {
                return p =>
                {
                    bool res1 = false;
                    bool res2 = false;
                    bool res3 = false;
                    bool res4 = false;

                    res4 = p.DeliveryNumber.ToLower().Contains(Search.ToLower());

                    if (!res4)
                    {
                        return false;
                    }

                    if (StatusId != 0)
                    {
                        res1 = p.StatusId == StatusId;
                    }
                    else
                    {
                        res1 = true;
                    }

                    res2 = p.DateOfOrder >= StartDate;
                    res3 = p.DateOfDelivery <= EndDate;

                    return res1 && res2 && res3 && res4;
                };
            }
        }
    }
}
