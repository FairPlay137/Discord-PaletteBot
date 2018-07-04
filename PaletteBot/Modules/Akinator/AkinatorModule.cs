using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Diagnostics;
using PaletteBot.Common;

namespace PaletteBot.Modules
{
    public class Akinator : PaletteBotModuleBase<SocketCommandContext>
    {
        [Command("akinator")]
        [Summary("Starts an Akinator game (COMING SOON)")]
        [Alias("aki")]
        public async Task StartAkinator()
        {
            await ReplyAsync($"`{StringResourceHandler.GetTextStatic("Akinator", "comingSoon")}`").ConfigureAwait(false);
        }
        [Command("akistop")]
        [Summary("Stops the currently running Akinator game (COMING SOON)")]
        public async Task StopAkinator()
        {
            await ReplyAsync($"`{StringResourceHandler.GetTextStatic("Akinator", "comingSoon")}`").ConfigureAwait(false);
        }
        [Command("akianswer")]
        [Summary("Within Akinator: Answers a question (COMING SOON)")]
        [Alias("aa")]
        public async Task SendAnswer(string answer)
        {
            await ReplyAsync($"`{StringResourceHandler.GetTextStatic("Akinator", "comingSoon")}`").ConfigureAwait(false);
        }
    }
}
