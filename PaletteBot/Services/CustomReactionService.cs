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
            SocketGuildChannel channel = (SocketGuildChannel)message.Channel;
            foreach (var cr in Program.CustomReactions)
            {
                string[] key = cr.Key.ToLower().Trim().Split(' ');
                string[] msg = message.Content.ToLower().Trim().Split(' ');
                bool matchesKey = true;
                int index = 0;
                try
                {
                    foreach(string kword in key)
                    {
                        string keyw;
                        switch(kword)
                        {
                            case "%mention%":
                                keyw = _client.CurrentUser.Mention; break;
                            case "%user%":
                                keyw = message.Author.Mention; break;
                            default:
                                keyw = kword; break;
                        }
                        if (kword != msg[index])
                            matchesKey = false;
                        index++;
                    }
                }
                catch (Exception) //TODO: This is a sloppy workaround and may not be the best solution; rewrite the code
                {
                    //LogManager.GetCurrentClassLogger().Debug("fix buggy message handling code in CustomReactionService.cs pl0x");
                }
                if (matchesKey)
                {
                    string target = "";
                    for(int x = index; x<msg.Length; x++)
                        target += ' '+msg[x];
                    target = target.TrimStart();
                    Random random = new Random();
                    string value = cr.Value[random.Next(cr.Value.Count)]
                        .Replace("%mention%", _client.CurrentUser.Mention)
                        .Replace("%user%", message.Author.Mention)
                        .Replace("%target%", target);
                    //TODO: Add "%rng%" for full NadekoBot compatibility
                    await message.Channel.SendMessageAsync(value);
                    LogManager.GetCurrentClassLogger().Info("**Custom Reaction Executed");
                    LogManager.GetCurrentClassLogger().Info($" Key: \"{cr}\"");
                    LogManager.GetCurrentClassLogger().Info($" Resp: \"{value}\"");
                    if (channel.Guild == null)
                        LogManager.GetCurrentClassLogger().Info(" [Sent in DMs]");
                    else
                    {
                        LogManager.GetCurrentClassLogger().Info($" Srvr: \"{channel.Guild.Name}\" ({channel.Guild.Id})");
                        LogManager.GetCurrentClassLogger().Info($" Chnl: #{channel.Name} ({channel.Id})");
                    }
                    return;
                }
            }
        }
    }
}
