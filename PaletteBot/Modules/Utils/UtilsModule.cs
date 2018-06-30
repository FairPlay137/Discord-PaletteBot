using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Diagnostics;
using PaletteBot.Common;

namespace PaletteBot.Modules
{
    public class Utils : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Pings the bot.")]
        public async Task Ping()
        {
            string pingwaitmsg = StringResourceHandler.GetTextStatic("Utils", "ping_wait");
            var sw = Stopwatch.StartNew();
            var msg = await Context.Channel.SendMessageAsync("🏓 " + pingwaitmsg).ConfigureAwait(false);
            sw.Stop();
            await msg.DeleteAsync();
            Random random = new Random();
            string subtitleText = StringResourceHandler.GetTextStatic("Utils", "ping_subtitle" + random.Next(1, 3));
            string footerText = StringResourceHandler.GetTextStatic("Utils", "ping_footer1");
            if(sw.ElapsedMilliseconds > 200)
                footerText = StringResourceHandler.GetTextStatic("Utils", "ping_footer2");
            if (sw.ElapsedMilliseconds > 500)
                footerText = StringResourceHandler.GetTextStatic("Utils", "ping_footer3");
            if (sw.ElapsedMilliseconds > 1000)
                footerText = StringResourceHandler.GetTextStatic("Utils", "ping_footer4");
            if (sw.ElapsedMilliseconds > 2000)
                footerText = StringResourceHandler.GetTextStatic("Utils", "ping_footer5");
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
            TimeSpan uptime = new TimeSpan(DateTime.Now.Ticks - Program.StartTime.Ticks);
            await ReplyAsync(Context.User.Mention, false, new EmbedBuilder()
                .WithTitle(StringResourceHandler.GetTextStatic("Utils", "stats_title"))
                .WithDescription((Program.botName == "PaletteBot") ?
                StringResourceHandler.GetTextStatic("Utils", "stats_descriptionPublic")
                :
                StringResourceHandler.GetTextStatic("Utils", "stats_description", Program.botName))
                .WithAuthor($"{Context.Client.CurrentUser.Username} v{typeof(Program).Assembly.GetName().Version}",Context.Client.CurrentUser.GetAvatarUrl())
                .AddField(StringResourceHandler.GetTextStatic("Utils", "stats_guilds"),Context.Client.Guilds.Count,true)
                .AddField(StringResourceHandler.GetTextStatic("Utils", "stats_uptime"), uptime.ToString(), true)
                .Build());
        }
    }
}
