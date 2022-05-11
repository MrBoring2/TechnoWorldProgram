using Newtonsoft.Json;
using System;
using System.Linq.Expressions;
using TechnoWorld_API.Models.Abstractions;
using TechoWorld_DataModels_v2.Entities;

namespace TechnoWorld_API.Models.Filters
{
    public class EmployeesFilter : FilterModel<Employee>
    {
        [JsonProperty("search")]
        public string Search { get; set; }
        [JsonProperty("postId")]
        public int PostId { get; set; }
        [JsonProperty("sortParameter")]
        public string SortParameter { get; set; }
        [JsonProperty("isAscending")]
        public bool IsAscending { get; set; }
        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("itemsPerPage")]
        public int ItemsPerPage { get; set; }
        public override Func<Employee, bool> FilterExpression
        {

            get
            {
                return p =>
                {
                    bool res1 = false;
                    bool res2 = false;

                    res1 = p.FullName.ToLower().Contains(Search.ToLower()) ||
                           p.PhoneNumber.ToLower().Contains(Search.ToLower()) ||
                           p.Email.ToLower().Contains(Search.ToLower());

                    if (!res1)
                    {
                        return false;
                    }

                    res2 = PostId == 0 ? true : p.PostId == PostId;

                    return res1 && res2;
                };
            }
        }
    }
}
