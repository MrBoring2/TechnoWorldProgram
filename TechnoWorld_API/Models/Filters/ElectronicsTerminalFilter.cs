using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;

namespace TechnoWorld_API.Models.Filters
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
                    bool res1 = false;
                    bool res2 = false;
                    bool res3 = false;
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
                    else
                    {
                        res1 = true;
                    }

                    if (ListMaunfacturersId == null || ListMaunfacturersId.Count <= 0)
                    {
                        res2 = true;
                    }
                    else
                    {
                        res2 = ListMaunfacturersId.Contains(p.ManufactrurerId);
                    }

                    if (ListElectronicsTypeId == null || ListElectronicsTypeId.Count <= 0)
                    {
                        res6 = true;
                    }
                    else
                    {
                        res6 = ListElectronicsTypeId.Contains(p.TypeId);
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

                    return res1 && res2 && res3 && res4 && res5 && res6;
                };



            }
        }
    }
}
