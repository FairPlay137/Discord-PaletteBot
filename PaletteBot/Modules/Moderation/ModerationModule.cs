using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Diagnostics;
using System.Reflection;
using NLog;
using PaletteBot.Common;
using Discord.WebSocket;

namespace PaletteBot.Modules
{
    public class Moderation : PaletteBotModuleBase<SocketCommandContext>
    {
        /*[Command("kick")]
        [Summary("Kicks a specified user from the server.")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Kick([Summary("User to kick")] IUser target)
        {
            //TODO: Finish this command
            await ReplyAsync($":ok: `{StringResourceHandler.GetTextStatic("Moderation", "kick", $"@{target.Username}#{target.Discriminator}")}`").ConfigureAwait(false);
        }*/
        [Command("ban")]
        [Summary("Bans a specified user from the server.")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Ban([Summary("User to ban")] IUser target)
        {
            //TODO: Make sure this works properly
            await Context.Guild.AddBanAsync(target);
            await ReplyAsync($":ok: `{StringResourceHandler.GetTextStatic("Moderation", "ban", $"@{target.Username}#{target.Discriminator}")}`").ConfigureAwait(false);
            try
            {
                var userdm = await target.GetOrCreateDMChannelAsync();
                await userdm.SendMessageAsync($":anger: {StringResourceHandler.GetTextStatic("Moderation", "dm_ban_header")}");
            }
            catch(Exception e)
            {
                await ReplyAsync(StringResourceHandler.GetTextStatic("Moderation", "DMFailed", e.Message));
            }
        }
    }
}
