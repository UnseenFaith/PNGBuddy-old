using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PNGBuddy.Events
{
    public class InputVolumeMeter
    {
        [JsonPropertyName("inputs")]
        public List<object>? Inputs { get; set; }
    }
}
