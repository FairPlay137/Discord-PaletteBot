using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaletteBot.Services
{
    public interface IBotConfiguration
    {
        string BotName { get; }
        string BotToken { get; }
        ulong BotOwnerID { get; }

        string DefaultPlayingString { get; }
        string DefaultPrefix { get; set; }

        string[] EightBallResponses { get; }

        Dictionary<string, List<string>> CustomReactions { get; set; }
        
    }
}
