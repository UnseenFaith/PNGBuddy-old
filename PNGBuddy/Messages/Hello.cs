using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PNGBuddy.Messages.Hello
{
    public class Hello 
    {
        [JsonPropertyName("obsWebSocketVersion")]
        public string? ObsWebSocketVersion { get; set; }

        [JsonPropertyName("rpcVersion")]
        public int? RpcVersion { get; set; }

        [JsonPropertyName("authentication")]
        public Authentication? Authentication { get; set; }

    }

    public class Authentication
    {
        [JsonPropertyName("challenge")]
        public string? Challenge { get; set; }

        [JsonPropertyName("salt")]
        public string? Salt { get; set; }
    }
}
