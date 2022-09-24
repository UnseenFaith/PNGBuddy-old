using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PNGBuddy.Types
{
    public class Input
    {
        [JsonPropertyName("inputs")]
        public List<InputElement>? Inputs { get; set; }
    }

    public partial class InputElement
    {
        [JsonPropertyName("inputName")]
        public string? Name { get; set; }

        [JsonPropertyName("inputKind")]
        public string? Kind { get; set; }

        [JsonPropertyName("inputLevelsMul")]
        public List<List<double>>? InputLevelsMul { get; set; }
    }
}
