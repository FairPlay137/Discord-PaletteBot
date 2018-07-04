using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using System.Diagnostics;
using PaletteBot.Common;
using Discord.WebSocket;
using NLog;
using System.Text.RegularExpressions;
using System.Collections.Generic;

// TODO: Finish this feature

namespace PaletteBot.Modules.CustomReactions
{
    public class CustomReactions : PaletteBotModuleBase<SocketCommandContext>
    {
        protected override void OnModuleBuilding(CommandService cs, ModuleBuilder b)
        {
            
        }
        
        [Command("lcr")]
        [Summary("List all the custom reactions stored in the bot.")]
        public async Task ListCustomReactions()
        {
            string desc = "";
            foreach(var cr in Program.CustomReactions)
                desc += "• " + cr.Key + '\n';
            EmbedBuilder eb = new EmbedBuilder()
                .WithTitle(StringResourceHandler.GetTextStatic("CustomReactions", "ListCustomReactions"))
                .WithDescription(desc)
                .WithColor(Color.Green);
            await ReplyAsync(Context.Message.Author.Mention, false, eb.Build());
        }

        //TODO: Add pal:scr (show custom reaction), pal:acr (add custom reaction), pal:dcr (delete custom reaction), and pal:ecr (edit custom reaction)
    }
}
