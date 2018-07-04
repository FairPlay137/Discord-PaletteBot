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
        static DiscordSocketClient _client;

        protected override void OnModuleBuilding(CommandService cs, ModuleBuilder b)
        {
            
        }

        public override void Initialize(DiscordSocketClient client)
        {
            try
            {
                LogManager.GetCurrentClassLogger().Info("Initializing custom reactions...");
                _client = client;
                _client.MessageReceived += CheckMessageAsync;
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error("Failed to load custom reaction handler!");
                LogManager.GetCurrentClassLogger().Error(e);
            }
        }

        private async Task CheckMessageAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            foreach (var cr in Program.CustomReactions)
            {
                string key = cr.Key.ToLower()
                    .Replace("%mention%", Context.Client.CurrentUser.Mention)
                    .Replace("%user%", message.Author.Mention);
                if (message.Content.ToLower().StartsWith(key))
                {
                    Random random = new Random();
                    string value = cr.Value[random.Next(cr.Value.Count)]
                        .Replace("%mention%", Context.Client.CurrentUser.Mention)
                        .Replace("%user%", message.Author.Mention);
                    //TODO: Add "%target%" for full NadekoBot compatibility; may require extra coding
                    await message.Channel.SendMessageAsync(value);
                    return;
                }
            }
        }
        /*
        [Command("lcr")]
        [Summary("List all the custom reactions stored in the bot.")]
        public async Task ListCustomReactions()
        {

        }*/
    }
}
