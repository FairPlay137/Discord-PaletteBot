﻿using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace PaletteBot.Common.ModuleBehaviors
{
    /// <summary>
    /// Description TBD
    /// </summary>
    public interface IInputOverrider
    {
        Task<string> TransformInput(IGuild guild, IMessageChannel channel, IUser user, string input);
    }
}
