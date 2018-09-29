using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaletteBot.Services
{
    public interface IBotConfigurationProvider
    {
        string BotName { get; }
        string BotToken { get; }
        ulong[] BotOwnerIDs { get; }

        Dictionary<string, List<string>> CustomReactions { get; set; }
        string[] EightBallResponses { get; }
    }
}
