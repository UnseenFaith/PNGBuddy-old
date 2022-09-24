using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PNGBuddy.Types
{
    public class InputMuteStateChanged
    {
        [JsonPropertyName("inputName")]
        public string? InputName { get; set; }

        [JsonPropertyName("inputMuted")]
        public bool? InputMuted { get; set; }
    }
}
