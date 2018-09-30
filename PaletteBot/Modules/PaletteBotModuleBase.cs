using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using PaletteBot.Services;

namespace PaletteBot.Common
{
    public abstract class PaletteBotModuleBase<T> : ModuleBase<SocketCommandContext>
    {
        public CommandHandler _cmdHandler { get; set; }
    }
}
