using Newtonsoft.Json;
using System;
using System.Linq.Expressions;
using TechnoWorld_API.Models.Abstractions;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;

namespace TechnoWorld_API.Models.Filters
{
    public class ElectronicsFilter : FilterModel<Electronic>
    {
        [JsonProperty("search")]
        public string Search { get; set; }
        [JsonProperty("categoryId")]
        public int CategoryId { get; set; }
        [JsonProperty("electronicsTypeId")]
        public int ElectronicsTypeId { get; set; }
        [JsonProperty("sortParameter")]
        public string SortParameter { get; set; }
        [JsonProperty("isAscending")]
        public bool IsAscending { get; set; }
        [JsonProperty("isOfferedForSale")]
        public bool? IsOfferedForSale { get; set; }
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("itemsPerPage")]
        public int ItemsPerPage { get; set; }
        public override Func<Electronic, bool> FilterExpression
        {
            get
            {
                return p =>
                {
                    bool? res1 = null;
                    bool? res2 = null;
                    bool? res3 = null;
                    bool res4 = false;

                    res4 = p.Model.ToLower().Contains(Search.ToLower());

                    if (!res4)
                    {
                        return false;
                    }

                    if (CategoryId != 0)
                    {
                        res1 = p.Type.CategoryId == CategoryId;
                    }
                    if (ElectronicsTypeId != 0)
                    {
                        res2 = p.TypeId == ElectronicsTypeId;
                    }
                    if (IsOfferedForSale != null)
                    {
                        res3 = p.IsOfferedForSale == IsOfferedForSale;
                    }

                    return (res1 == null ? true : (bool)res1) && (res2 == null ? true : (bool)res2) && (res3 == null ? true : (bool)res3) && res4;
                };



            }
        }
    }
}
