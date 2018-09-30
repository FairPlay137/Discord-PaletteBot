using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PaletteBot.Common
{
    public struct ConfigJsonStructure
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("prefix")]
        public string CommandPrefix { get; set; }

        [JsonProperty("defaultplaying")]
        public string DefaultPlayingString { get; set; }

        [JsonProperty("ownerid")]
        public ulong OwnerID { get; set; }

        [JsonProperty("botname")]
        public string BotName { get; set; }

        [JsonProperty("verboseerrors")]
        public bool VerboseErrors { get; set; }

        [JsonProperty("8ballResponses")]
        public string[] EightBallResponses { get; set; }

        [JsonProperty("customReactions")]
        public Dictionary<string, List<string>> CustomReactions { get; set; }
    }
}
