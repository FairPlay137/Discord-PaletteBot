﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Diagnostics;
using PaletteBot.Common;
using PaletteBot.Services;
using PaletteBot.Common.Attributes;

namespace PaletteBot.Modules
{
    public class Utils : PaletteBotModuleBase<SocketCommandContext>
    {
        private readonly PaletteBot _bot;

        private readonly IBotConfiguration _config;

        public Utils(PaletteBot bot)
        {
            _bot = bot;
            _config = bot.Configuration;
        }

        [Command("ping")]
        [Summary("Pings the bot.")]
        public async Task Ping()
        {
            var pleasewait = Context.Channel.EnterTypingState();
            string pingwaitmsg = StringResourceHandler.GetTextStatic("Utils", "ping_wait");
            var msg = await Context.Channel.SendMessageAsync("🏓 " + pingwaitmsg).ConfigureAwait(false);
            var sw = Stopwatch.StartNew();
            await msg.DeleteAsync();
            sw.Stop();
            Random random = new Random();
            string subtitleText = StringResourceHandler.GetTextStatic("Utils", "ping_subtitle" + random.Next(1, 5));
            string footerText = StringResourceHandler.GetTextStatic("Utils", "ping_footer1");
            if(sw.ElapsedMilliseconds > 200)
                footerText = StringResourceHandler.GetTextStatic("Utils", "ping_footer2");
            if (sw.ElapsedMilliseconds > 500)
                footerText = StringResourceHandler.GetTextStatic("Utils", "ping_footer3");
            if (sw.ElapsedMilliseconds > 1200)
                footerText = StringResourceHandler.GetTextStatic("Utils", "ping_footer4");
            if (sw.ElapsedMilliseconds > 5000)
                footerText = StringResourceHandler.GetTextStatic("Utils", "ping_footer5");
            pleasewait.Dispose();
            await ReplyAsync(Context.User.Mention, false, new EmbedBuilder()
                .WithTitle("🏓 " + StringResourceHandler.GetTextStatic("Utils", "ping_title"))
                .WithDescription(subtitleText+'\n'+StringResourceHandler.GetTextStatic("Utils", "ping_pingtime", sw.ElapsedMilliseconds))
                .WithFooter(footerText)
                .Build());
        }
        [Command("invite")]
        [Summary("Gets the invite link for this bot.")]
        public async Task Invite()
        {
            /*await ReplyAsync($"{Context.User.Mention} - {StringResourceHandler.GetTextStatic("Utils", "invite")}", false, new EmbedBuilder()
                .WithTitle("Invite PaletteBot onto your own server!")
                .WithUrl($"https://discordapp.com/oauth2/authorize?client_id={Context.Client.CurrentUser.Id}&permissions=8&scope=bot")
                .Build());*/
            await ReplyAsync($"{Context.User.Mention} - {StringResourceHandler.GetTextStatic("Utils", "invite")} https://discordapp.com/oauth2/authorize?client_id={Context.Client.CurrentUser.Id}&permissions=8&scope=bot");
        }
        [Command("stats")]
        [Summary("Gets this bot's stats.")]
        public async Task Stats()
        {
            TimeSpan uptime = new TimeSpan(DateTime.Now.Ticks - _bot.StartTime.Ticks);
            await ReplyAsync(Context.User.Mention, false, new EmbedBuilder()
                .WithTitle(StringResourceHandler.GetTextStatic("Utils", "stats_title"))
                .WithDescription((_config.BotName == "PaletteBot") ?
                StringResourceHandler.GetTextStatic("Utils", "stats_descriptionPublic")
                :
                StringResourceHandler.GetTextStatic("Utils", "stats_description", _config.BotName))
                .WithAuthor($"{Context.Client.CurrentUser.Username} v{typeof(Program).Assembly.GetName().Version}",Context.Client.CurrentUser.GetAvatarUrl())
                .AddField(StringResourceHandler.GetTextStatic("Utils", "stats_guilds"),Context.Client.Guilds.Count,true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "stats_uptime"), uptime.ToString(), true)
                .Build());
        }
        [Command("serverinfo")]
        [Summary("Retrieves the server's information")]
        [CannotUseInDMs]
        public async Task GuildInfo()
        {
            string featureList = StringResourceHandler.GetTextStatic("Utils", "sinfo_noFeatures");
            if(Context.Guild.Features.Count>0)
            {
                featureList = "";
                foreach(string feature in Context.Guild.Features)
                {
                    featureList += "• " + feature + '\n';
                }
            }
            var inviteLinks = await Context.Guild.GetInvitesAsync().ConfigureAwait(false);
            int userCount = 0;
            int botCount = 0;
            foreach(var user in Context.Guild.Users)
            {
                if (user.IsBot)
                    botCount++;
                else
                    userCount++;
            }
            await ReplyAsync(Context.User.Mention, false, new EmbedBuilder()
                .WithTitle(Context.Guild.Name)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_id"),Context.Guild.Id,true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_created"), Context.Guild.CreatedAt, true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_owner"), $"@{Context.Guild.Owner.Username}#{Context.Guild.Owner.Discriminator} ({Context.Guild.Owner.Id})", true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_users"), $"{Context.Guild.MemberCount} {StringResourceHandler.GetTextStatic("Utils", "sinfo_userbotratio",userCount,botCount)}", true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_categories"), Context.Guild.CategoryChannels.Count, true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_textchannels"), Context.Guild.TextChannels.Count, true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_voicechannels"), Context.Guild.VoiceChannels.Count, true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_roles"), Context.Guild.Roles.Count, true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_features"), featureList, true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_invites"), inviteLinks.Count, true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "sinfo_customemotes"), Context.Guild.Emotes.Count, true)
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .Build());
        }
    }
}
