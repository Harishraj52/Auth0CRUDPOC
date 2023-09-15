using Newtonsoft.Json;

namespace Auth0CRUDPOC.Core_Layer.Models
{
    public class BlockUserResponse
    {
        [JsonProperty("blocked")]
        public bool Blocked { get; set; }
    }
}
