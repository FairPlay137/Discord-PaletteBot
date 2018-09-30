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
using PaletteBot.Services;
using PaletteBot.Common.ModuleBehaviors;

namespace PaletteBot.Modules.CustomReactions.Services
{
    public class CustomReactionService : IPreXBlockerExecutor, IPaletteBotService
    {
        private readonly DiscordSocketClient _client;

        private readonly IBotConfiguration _config;

        public CustomReactionService(DiscordSocketClient client, IBotConfiguration config)
        {
            _client = client;
            _config = config;
        }

        public async Task<bool> TryExecuteEarly(DiscordSocketClient client, IGuild guild, IUserMessage msg)
        {
            IGuildChannel channel = (IGuildChannel)msg.Channel;
            foreach (var cr in _config.CustomReactions)
            {
                string[] key = cr.Key.ToLower().Trim().Split(' ');
                string[] message = msg.Content.ToLower().Trim().Split(' ');
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
                                keyw = msg.Author.Mention; break;
                            default:
                                keyw = kword; break;
                        }
                        if (kword != message[index])
                            matchesKey = false;
                        index++;
                    }
                }
                catch (Exception)
                {

                }
                if (matchesKey)
                {
                    string target = "";
                    for(int x = index; x<message.Length; x++)
                        target += ' '+message[x];
                    target = target.TrimStart();
                    Random random = new Random();
                    string value = cr.Value[random.Next(cr.Value.Count)]
                        .Replace("%mention%", _client.CurrentUser.Mention)
                        .Replace("%user%", msg.Author.Mention)
                        .Replace("%target%", target);
                    //TODO: Add "%rng%" for full NadekoBot compatibility
                    await msg.Channel.SendMessageAsync(value);
                    return true;
                    /*LogManager.GetCurrentClassLogger().Info("**Custom Reaction Executed");
                    LogManager.GetCurrentClassLogger().Info($" Key: \"{cr}\"");
                    LogManager.GetCurrentClassLogger().Info($" Resp: \"{value}\"");
                    if (channel.Guild == null)
                        LogManager.GetCurrentClassLogger().Info(" [Sent in DMs]");
                    else
                    {
                        LogManager.GetCurrentClassLogger().Info($" Srvr: \"{channel.Guild.Name}\" ({channel.Guild.Id})");
                        LogManager.GetCurrentClassLogger().Info($" Chnl: #{channel.Name} ({channel.Id})");
                    }*/
                }
            }
            return false;
        }
    }
}
