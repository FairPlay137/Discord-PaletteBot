using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace PaletteBot.Common
{
    public abstract class PaletteBotModuleBase<T> : ModuleBase<SocketCommandContext>
    {
        public virtual void Initialize(DiscordSocketClient client)
        {

        }
    }
}
