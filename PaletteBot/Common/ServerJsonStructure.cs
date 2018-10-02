﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace PaletteBot.Common
{
    public struct ServerJsonStructure
    {
        [JsonProperty("prefix")]
        public string CommandPrefix { get; set; }

        [JsonProperty("verboseErrors")]
        public bool VerboseErrors { get; set; }

        [JsonProperty("customReactions")]
        public Dictionary<string, List<string>> CustomReactions { get; set; }

        [JsonProperty("enableGreetMessage")]
        public bool EnableGreetMessage { get; set; }

        [JsonProperty("greetMessage")]
        public string GreetMessage { get; set; }

        [JsonProperty("enableByeMessage")]
        public bool EnableByeMessage { get; set; }

        [JsonProperty("byeMessage")]
        public string ByeMessage { get; set; }
    }
}
