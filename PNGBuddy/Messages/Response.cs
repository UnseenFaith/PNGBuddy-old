using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PNGBuddy.Messages
{
    public class Response
    {
        [JsonPropertyName("requestType")]

        public string? Type { get; set; }
        [JsonPropertyName("requestId")]

        public string? Id { get; set; }
        [JsonPropertyName("requestStatus")]

        public Status? Status { get; set; }

        [JsonPropertyName("responseData")]
        public object? Data { get; set; }
    }

    public class Status
    {
        [JsonPropertyName("result")]
        public bool? Result { get; set; }

        [JsonPropertyName("code")]
        public int? Code { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
    }
}
