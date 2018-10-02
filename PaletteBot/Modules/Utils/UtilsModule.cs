using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Diagnostics;
using PaletteBot.Common;
using PaletteBot.Services;

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
    }
}
