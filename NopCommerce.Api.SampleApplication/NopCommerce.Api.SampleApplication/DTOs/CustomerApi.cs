using Newtonsoft.Json;

namespace NopCommerce.Api.SampleApplication.DTOs
{
    // Simplified Customer dto object with only the first and last name
    public class CustomerApi
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
    }
}