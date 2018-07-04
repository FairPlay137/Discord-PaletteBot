using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using NLog;
using Discord.WebSocket;

namespace PaletteBot.Services
{
    public class CustomReactionService : PaletteBotService
    {
        static DiscordSocketClient _client;

        public CustomReactionService(DiscordSocketClient client)
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
                    .Replace("%mention%", _client.CurrentUser.Mention)
                    .Replace("%user%", message.Author.Mention);
                if (message.Content.ToLower().StartsWith(key))
                {
                    Random random = new Random();
                    string value = cr.Value[random.Next(cr.Value.Count)]
                        .Replace("%mention%", _client.CurrentUser.Mention)
                        .Replace("%user%", message.Author.Mention);
                    //TODO: Add "%target%" and "%rng% for full NadekoBot compatibility; may require extra coding
                    await message.Channel.SendMessageAsync(value);
                    return;
                }
            }
        }
    }
}
