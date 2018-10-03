﻿using System;
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

        int TotalShards { get; }

        string DefaultPlayingString { get; }
        string DefaultPrefix { get; set; }

        bool VerboseErrors { get; set; }

        string[] EightBallResponses { get; }

        Dictionary<string, List<string>> CustomReactions { get; set; }

        bool RotatePlayingStatuses { get; set; }
        string[] PlayingStatuses { get; set; }

        bool ReloadConfig(bool verbose);
        bool SaveConfig(bool verbose);
    }
}
