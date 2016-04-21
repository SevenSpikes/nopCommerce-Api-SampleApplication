using System.Collections.Generic;
using Newtonsoft.Json;

namespace NopCommerce.Api.SampleApplication.DTOs
{
    public class CustomersRootObject
    {
        [JsonProperty("customers")]
        public List<CustomerApi> Customers { get; set; }
    }
}