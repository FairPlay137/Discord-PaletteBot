using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Diagnostics;
using System.Reflection;
using NLog;
using PaletteBot.Common;

namespace PaletteBot.Modules
{
    public class Admin : PaletteBotModuleBase<SocketCommandContext>
    {
        [Command("shutdown")]
        [Summary("Shuts down the bot. **BOT OWNER ONLY**")]
        [Alias("die")]
        public async Task Shutdown()
        {
            LogManager.GetCurrentClassLogger().Debug($"Owner ID is \"{Program.OwnerID}\"; author ID is \"{Context.Message.Author.Id.ToString()}\"");
            if (Program.OwnerID == 0) //TODO: Make this check a precondition rather than an in-command hardcoded check
            {
                await ReplyAsync($":no_entry: `{StringResourceHandler.GetTextStatic("err", "noBotOwner")}`");
            }
            else
            {
                if (Program.OwnerID != Context.Message.Author.Id)
                {
                    await ReplyAsync($":no_entry: `{StringResourceHandler.GetTextStatic("err", "notBotOwner")}`");
                }
                else
                {
                    await ReplyAsync($":ok: `{StringResourceHandler.GetTextStatic("Admin", "shutdown")}`").ConfigureAwait(false);
                    await Task.Delay(2000).ConfigureAwait(false);
                    Environment.Exit(0);
                }
            }
        }
        [Command("restart")]
        [Summary("Restarts the bot. **BOT OWNER ONLY**")]
        public async Task Restart()
        {
            LogManager.GetCurrentClassLogger().Debug($"Owner ID is \"{Program.OwnerID}\"; author ID is \"{Context.Message.Author.Id}\"");
            if (Program.OwnerID == 0) //TODO: Make this check a precondition rather than an in-command hardcoded check
            {
                await ReplyAsync($":no_entry: `{StringResourceHandler.GetTextStatic("err", "noBotOwner")}`");
            }
            else
            {
                if (Program.OwnerID != Context.Message.Author.Id)
                {
                    await ReplyAsync($":no_entry: `{StringResourceHandler.GetTextStatic("err", "notBotOwner")}`");
                }
                else
                {
                    await ReplyAsync($":ok: `{StringResourceHandler.GetTextStatic("Admin", "restart")}`");
                    await Task.Delay(2000);
                    Process.Start(Assembly.GetExecutingAssembly().Location);
                    Environment.Exit(0);
                }
            }
        }
        [Command("setgame")]
        [Summary("Sets the bot's game. **BOT OWNER ONLY**")]
        public async Task SetGame([Remainder] [Summary("Game to show on status")] string game)
        {
            LogManager.GetCurrentClassLogger().Debug($"Owner ID is \"{Program.OwnerID}\"; author ID is \"{Context.Message.Author.Id.ToString()}\"");
            if (Program.OwnerID == 0) //TODO: Make this check a precondition rather than an in-command hardcoded check
            {
                await ReplyAsync($":no_entry: `{StringResourceHandler.GetTextStatic("err", "noBotOwner")}`");
            }
            else
            {
                if (Program.OwnerID != Context.Message.Author.Id)
                {
                    await ReplyAsync($":no_entry: `{StringResourceHandler.GetTextStatic("err", "notBotOwner")}`");
                }
                else
                {
                    await Context.Client.SetGameAsync(game);
                    await ReplyAsync($":ok: `{StringResourceHandler.GetTextStatic("Admin", "setGame",game)}`").ConfigureAwait(false);
                }
            }
        }
        [Command("setstatus")]
        [Summary("Sets the bot's status. **BOT OWNER ONLY**")]
        public async Task SetStatus([Summary("Status (Online/Idle/DnD/Invisible)")] string status)
        {
            LogManager.GetCurrentClassLogger().Debug($"Owner ID is \"{Program.OwnerID}\"; author ID is \"{Context.Message.Author.Id.ToString()}\"");
            if (Program.OwnerID == 0) //TODO: Make this check a precondition rather than an in-command hardcoded check
            {
                await ReplyAsync($":no_entry: `{StringResourceHandler.GetTextStatic("err", "noBotOwner")}`");
            }
            else
            {
                if (Program.OwnerID != Context.Message.Author.Id)
                {
                    await ReplyAsync($":no_entry: `{StringResourceHandler.GetTextStatic("err", "notBotOwner")}`");
                }
                else
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
            }
        }
    }
}
