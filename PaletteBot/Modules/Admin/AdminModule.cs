using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Diagnostics;
using System.Reflection;
using NLog;
using PaletteBot.Common;
using PaletteBot.Common.Attributes;
using PaletteBot.Services;

namespace PaletteBot.Modules
{
    public class Admin : PaletteBotModuleBase<CommandContext>
    {
        private readonly IBotConfiguration _config;

        public Admin(IBotConfiguration config)
        {
            _config = config;
        }

        [Command("shutdown")]
        [Summary("Shuts down the bot. **BOT OWNER ONLY**")]
        [Alias("die")]
        [OwnerOnly]
        public async Task Shutdown()
        {
            await ReplyAsync($":ok: `{StringResourceHandler.GetTextStatic("Admin", "shutdown")}`").ConfigureAwait(false);
            await Task.Delay(2000).ConfigureAwait(false);
            Environment.Exit(0);
        }
        [Command("restart")]
        [Summary("Restarts the bot. **BOT OWNER ONLY**")]
        [OwnerOnly]
        public async Task Restart()
        {
            await ReplyAsync($":ok: `{StringResourceHandler.GetTextStatic("Admin", "restart")}`");
            await Task.Delay(2000);
            Process.Start(Assembly.GetExecutingAssembly().Location);
            Environment.Exit(0);
        }
        [Command("setgame")]
        [Summary("Sets the bot's game. **BOT OWNER ONLY**")]
        [OwnerOnly]
        public async Task SetGame([Remainder] [Summary("Game to show on status")] string game)
        {
            await Context.Client.SetGameAsync(game);
            await ReplyAsync($":ok: `{StringResourceHandler.GetTextStatic("Admin", "setGame",game)}`").ConfigureAwait(false);
        }
        [Command("setstatus")]
        [Summary("Sets the bot's status. **BOT OWNER ONLY**")]
        [OwnerOnly]
        public async Task SetStatus([Summary("Status (Online/Idle/DnD/Invisible)")] string status)
        {
            switch (status.ToLower())
            {
                case "online":
                    await Context.Client.SetStatusAsync(UserStatus.Online);
                    break;
                case "idle":
                    await Context.Client.SetStatusAsync(UserStatus.Idle);
                    break;
                case "dnd":
                    await Context.Client.SetStatusAsync(UserStatus.DoNotDisturb);
                    break;
                case "invisible":
                    await Context.Client.SetStatusAsync(UserStatus.Invisible);
                    break;
                default:
                    await ReplyAsync($":no_entry: `{StringResourceHandler.GetTextStatic("err", "invalidStatus")}`");
                    return;
            }
            await ReplyAsync($":ok: `{StringResourceHandler.GetTextStatic("Admin", "setStatus")}`").ConfigureAwait(false);
        }
        [Command("verboseerrors")]
        [Summary("Enables/disables verbose error messages **BOT OWNER ONLY**")]
        [OwnerOnly]
        public async Task ToggleVerboseErrors()
        {
            _config.VerboseErrors = !_config.VerboseErrors;
            _config.SaveConfig();
            _config.ReloadConfig();
            string toCueUp = "verboseErrors_disable";
            if(_config.VerboseErrors)
                toCueUp = "verboseErrors_enable";
            await ReplyAsync($":ok: `{StringResourceHandler.GetTextStatic("Admin", toCueUp)}`").ConfigureAwait(false);
        }
    }
}
