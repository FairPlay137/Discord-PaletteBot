using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PaletteBot.Common;

namespace PaletteBot.Modules
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Summary("To be implemented...")]
        [Alias("h")]
        public async Task HelpCmd()
        {
            var dmchannel = await Context.User.GetOrCreateDMChannelAsync();

            string dmcontent = StringResourceHandler.GetTextStatic("Help", "DMContent",Program.prefix,
                (Program.botName == "PaletteBot")? //I did have some compiler flags to switch out the two strings when it was a PublicRelease build, but they didn't work, so I removed them before I committed this change
                StringResourceHandler.GetTextStatic("Help", "introPublic")
                :
                StringResourceHandler.GetTextStatic("Help", "intro", Program.botName)

                );
            if (Program.OwnerID == 0)
                dmcontent += StringResourceHandler.GetTextStatic("Help", "DMContentNoBotOwner");
            else
                dmcontent += StringResourceHandler.GetTextStatic("Help", "DMContentContactBotOwner",Context.Client.GetUser(Program.OwnerID).Mention);
            await dmchannel.SendMessageAsync(dmcontent);
            await ReplyAsync(StringResourceHandler.GetTextStatic("Help", "DMedHelp"));
        }
        [Command("modules")]
        [Summary("Lists all modules.")]
        public async Task Modules()
        {
            var moduleseb = new EmbedBuilder().WithTitle(StringResourceHandler.GetTextStatic("Help", "modules_header")).WithColor(Color.Orange);
            foreach (var module in Program._commands.Modules)
                moduleseb.AddField("» " + module.Name, StringResourceHandler.GetTextStatic("Help", "modules_commandcount",module.Commands.Count));
            await ReplyAsync(Context.User.Mention, false, moduleseb.Build());
        }
        [Command("commands")]
        [Summary("Lists all commands in a module.")]
        [Alias("cmds")]
        public async Task Commands(string modulename)
        {
            ModuleInfo targetmodule = null;
            foreach (var module in Program._commands.Modules)
                if (module.Name.ToLower() == modulename.ToLower())
                    targetmodule = module;
            if(targetmodule == null)
                await ReplyAsync($":no_entry: `{StringResourceHandler.GetTextStatic("err", "nonexistentModule")}`");
            else
            {
                var commandseb = new EmbedBuilder().WithTitle(StringResourceHandler.GetTextStatic("Help", "commands_header",targetmodule.Name)).WithColor(Color.Orange);
                if (targetmodule.Commands.Count > 0)
                {
                    foreach (var command in targetmodule.Commands)
                        commandseb.AddField("» " + Program.prefix + command.Name, command.Summary);
                }
                else
                {
                    commandseb.WithDescription(StringResourceHandler.GetTextStatic("Help", "commandListEmpty"));
                }
                await ReplyAsync(Context.User.Mention, false, commandseb.Build());
            }
        }
    }
}
