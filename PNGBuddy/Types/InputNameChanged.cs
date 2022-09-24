using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PNGBuddy.Types
{
    public class InputNameChanged
    {
        [JsonPropertyName("oldInputName")]
        public string? OldInputName { get; set; }

        [JsonPropertyName("inputName")]
        public string? InputName { get; set; }
    }
}
