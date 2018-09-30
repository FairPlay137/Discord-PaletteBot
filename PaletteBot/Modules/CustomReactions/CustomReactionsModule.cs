using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using System.Diagnostics;
using PaletteBot.Common;
using PaletteBot.Modules.CustomReactions.Services;
using PaletteBot.Services;
using Discord.WebSocket;
using NLog;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace PaletteBot.Modules.CustomReactions
{
    public class CustomReactions : PaletteBotModuleBase<CustomReactionService>
    {

        private readonly IBotConfiguration _config;

        public CustomReactions(IBotConfiguration config)
        {
            _config = config;
        }

        [Command("lcr")]
        [Summary("Lists all the custom reactions stored in the bot.")]
        public async Task ListCustomReactions()
        {
            string desc = "";
            foreach (var cr in _config.CustomReactions)
                desc += "• " + cr.Key + '\n';
            if (_config.CustomReactions.Count == 0)
                desc += StringResourceHandler.GetTextStatic("CustomReactions", "lcr_noCustomReactions");
            else if(_config.CustomReactions.Count == 1)
                desc += StringResourceHandler.GetTextStatic("CustomReactions", "lcr_TotalCountOne");
            else
                desc += StringResourceHandler.GetTextStatic("CustomReactions", "lcr_TotalCountMultiple", _config.CustomReactions.Count);
            EmbedBuilder eb = new EmbedBuilder()
                .WithTitle(StringResourceHandler.GetTextStatic("CustomReactions", "ListCustomReactions"))
                .WithDescription(desc)
                .WithColor(Color.Green);
            await ReplyAsync(Context.Message.Author.Mention, false, eb.Build());
        }

        [Command("scr")]
        [Summary("Shows all the responses for a specified custom reaction")]
        public async Task ShowCustomReaction([Remainder] [Summary("Custom reaction to view")] string ikey)
        {
            int matches = 0;
            string inputkey = ikey.Trim().ToLower();
            EmbedBuilder eb = new EmbedBuilder();
            foreach (var cr in _config.CustomReactions)
            {
                if(cr.Key.Trim().ToLower().Equals(inputkey))
                {
                    string desc = "";
                    string title = cr.Key;
                    int respnum = 1;
                    foreach(var response in cr.Value.ToArray())
                    {
                        desc += $"***{StringResourceHandler.GetTextStatic("CustomReactions", "response", respnum)}*** {response}\n";
                        respnum++;
                    }
                    desc = desc.Trim();
                    if (matches > 0)
                        title += $" ({matches})";
                    eb.AddField(title, desc);
                    matches++;
                }
            }
            if(matches==0)
            {
                eb.WithTitle(StringResourceHandler.GetTextStatic("CustomReactions", "ShowCustomReaction_noResults"))
                    .WithDescription(StringResourceHandler.GetTextStatic("CustomReactions", "ShowCustomReaction_noResults_desc",ikey))
                    .WithColor(Color.Red);
            }
            else
            {
                eb.WithColor(Color.Green);
                if(matches>1)
                    eb.WithTitle(StringResourceHandler.GetTextStatic("CustomReactions", "ShowCustomReaction_multipleResults"));
                else
                    eb.WithTitle(StringResourceHandler.GetTextStatic("CustomReactions", "ShowCustomReaction"));
            }
            await ReplyAsync(Context.Message.Author.Mention, false, eb.Build());
        }
        //TODO: Add pal:acr (add custom reaction), pal:dcr (delete custom reaction), and pal:ecr (edit custom reaction)
    }
}
