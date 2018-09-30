﻿using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace PaletteBot.Common.ModuleBehaviors
{
    /// <summary>
    /// Blocks execution before anything is executed
    /// </summary>
    public interface IPreXBlocker
    {
        Task<bool> TryBlockEarly(IGuild guild, IUserMessage msg);
    }
}
