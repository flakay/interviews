using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Realmdigital_Interview.Models;

namespace Realmdigital_Interview.Controllers
{
    public class ProductController
    {
        [Route("product")]
        public object GetProductById(string productId)
        {
            string response = "";

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                response = client.UploadString("http://192.168.0.241/eanlist?type=Web", "POST", "{ \"id\": \"" + productId + "\" }");
            }

            var reponseObject = JsonConvert.DeserializeObject<List<ApiResponseProduct>>(response);

            var result = new List<ApiResult>();
            for (int i = 0; i < reponseObject.Count; i++)
            {
                List<ApiResponsePrice> prices = new List<ApiResponsePrice>();
                GetProductDetailsByID(reponseObject, result, i, prices);
            }
            return result.Count > 0 ? result[0] : null;
        }

        private static void GetProductDetailsByID(List<ApiResponseProduct> reponseObject, List<ApiResult> result, int i, List<ApiResponsePrice> prices)
        {
            for (int j = 0; j < reponseObject[i].PriceRecords.Count; j++)
            {
                if (reponseObject[i].PriceRecords[j].CurrencyCode == "ZAR")
                {
                    prices.Add(new ApiResponsePrice()
                    {
                        SellingPrice = reponseObject[i].PriceRecords[j].SellingPrice,
                        CurrencyCode = reponseObject[i].PriceRecords[j].CurrencyCode
                    });
                }
            }
            result.Add(new ApiResult()
            {
                Id = reponseObject[i].BarCode,
                Name = reponseObject[i].ItemName,
                Prices = prices
            });
        }

        [Route("product/search")]
        public List<ApiResult> GetProductsByName(string productName)
        {
            string response = "";

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                response = client.UploadString("http://192.168.0.241/eanlist?type=Web", "POST", "{ \"names\": \"" + productName + "\" }");
            }

            var reponseObject = JsonConvert.DeserializeObject<List<ApiResponseProduct>>(response);

            var result = new List<ApiResult>();
            for (int i = 0; i < reponseObject.Count; i++)
            {
                List<ApiResponsePrice> prices = new List<ApiResponsePrice>();
                GetProductDetailsByName(reponseObject, result, i, prices);
            }
            return result;
        }

        private static void GetProductDetailsByName(List<ApiResponseProduct> reponseObject, List<ApiResult> result, int i, List<ApiResponsePrice> prices)
        {
            for (int j = 0; j < reponseObject[i].PriceRecords.Count; j++)
            {
                if (reponseObject[i].PriceRecords[j].CurrencyCode == "ZAR")
                {
                    prices.Add(new ApiResponsePrice()
                    {
                        SellingPrice = reponseObject[i].PriceRecords[j].SellingPrice,
                        CurrencyCode = reponseObject[i].PriceRecords[j].CurrencyCode
                    });
                }
            }
            result.Add(new ApiResult()
            {
                Id = reponseObject[i].BarCode,
                Name = reponseObject[i].ItemName,
                Prices = prices
            });
        }
    }
}
