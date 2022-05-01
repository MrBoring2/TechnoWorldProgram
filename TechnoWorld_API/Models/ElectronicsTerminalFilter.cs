using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using TechoWorld_DataModels;

namespace TechnoWorld_API.Models
{
    public class ElectronicsTerminalFilter : ElectronicsFilter
    {
        [JsonProperty("listElectronicsTypeId")]
        public List<int> ListElectronicsTypeId { get; set; }
        [JsonProperty("listManufacturersId")]
        public List<int> ListMaunfacturersId { get; set; }
        [JsonProperty("minPrice")]
        public decimal MinPrice { get; set; }
        [JsonProperty("maxPrice")]
        public decimal MaxPrice { get; set; }
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
                    bool res5 = false;
                    bool res6 = false;

                    res4 = p.Model.ToLower().Contains(Search.ToLower());

                    if (!res4)
                    {
                        return false;
                    }

                    if (CategoryId != 0)
                    {
                        res1 = p.Type.CategoryId == CategoryId;
                    }
                    if (ListElectronicsTypeId.Count > 0)
                    {
                        res2 = ListElectronicsTypeId.Contains(p.TypeId);
                    }
                    else
                    {
                        res2 = true;
                    }
                    if (ListMaunfacturersId.Count > 0)
                    {
                        res6 = ListMaunfacturersId.Contains(p.ManufactrurerId);
                    }
                    else
                    {
                        res6 = true;
                    }
                    if (IsOfferedForSale != null)
                    {
                        res3 = p.IsOfferedForSale == IsOfferedForSale;
                    }
                    if (MinPrice == 0 && MaxPrice == 0)
                    {
                        res5 = true;
                    }
                    else
                    {
                        res5 = p.SalePrice >= MinPrice && p.SalePrice <= MaxPrice;
                    }

                    return (res1 == null ? true : (bool)res1) && (res2 == null ? true : (bool)res2) && (res3 == null ? true : (bool)res3) && res4 && res5 && res6;
                };



            }
        }
    }
}
