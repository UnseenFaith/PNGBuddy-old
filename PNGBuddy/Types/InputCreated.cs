using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PNGBuddy.Types
{
    public class InputCreated
    {
        [JsonPropertyName("inputName")]
        public string? InputName { get; set; }

        [JsonPropertyName("inputKind")]
        public string? InputKind { get; set; }

        [JsonPropertyName("unversionedInputKind")]
        public string? UnversionedInputKind { get; set; }

        [JsonPropertyName("inputSettings")]
        public object? InputSettings { get; set; }

        [JsonPropertyName("defaultInputSettings")]
        public object? DefaultInputSettings { get; set; }
    }
}
