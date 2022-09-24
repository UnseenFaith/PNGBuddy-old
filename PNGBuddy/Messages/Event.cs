using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PNGBuddy.Messages
{
    public class Event
    {
        [JsonPropertyName("eventType")]
        public string? Type { get; set; }

        [JsonPropertyName("eventIntent")]
        public int? Intent { get; set; }

        [JsonPropertyName("eventData")]
        public object? Data { get; set; }
    }
}
