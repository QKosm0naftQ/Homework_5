using Newtonsoft.Json;

namespace Homework_5.Models.Department
{
    public class DepartmentPostModel
    {
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; } = string.Empty;
        [JsonProperty("modelName")]
        public string ModelName { get; set; } = "Address";
        [JsonProperty("calledMethod")]
        public string CalledMethod { get; set; } = "getWarehouses";
        [JsonProperty("methodProperties")]
        public MethodDepatmentProperties? MethodProperties { get; set; }
    }

    public class MethodDepatmentProperties
    {
        public string CityRef { get; set; } = String.Empty;
    }
}
