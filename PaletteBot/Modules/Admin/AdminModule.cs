using System;
using System.Threading.Tasks;
using Discord.Commands;
using System.Diagnostics;
using System.Reflection;
using NLog;
using PaletteBot.Common;

namespace PaletteBot.Modules
{
    public class Admin : ModuleBase<SocketCommandContext>
    {
        [Command("shutdown")]
        [Summary("Shuts down the bot. **BOT OWNER ONLY**")]
        [Alias("die")]
        public async Task Shutdown()
        {
            LogManager.GetCurrentClassLogger().Debug($"Owner ID is \"{Program.OwnerID}\"; author ID is \"{Context.Message.Author.Id.ToString()}\"");
            if (Program.OwnerID == 0)
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
            if (Program.OwnerID == 0)
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
    }
}
