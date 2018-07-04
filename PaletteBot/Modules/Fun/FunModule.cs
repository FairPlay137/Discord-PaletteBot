using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using PaletteBot.Common;
using NLog;

namespace PaletteBot.Modules
{
    public class Fun : PaletteBotModuleBase<SocketCommandContext>
    {
        string[] EightBallResponses = { "Yes.", "No.", "Ask again later.", "Maybe???", "It is certain", "Very doubtful",
                                        "NO - It'll cause a time paradox.", "Chances are as high as the sky.", "lolnope",
                                        "Obviously.", "Don't count on it.", "I'd say so.", "Received interference; try again."};
        protected override void OnModuleBuilding(CommandService cs, ModuleBuilder b)
        {
            if(Program.EightBallResponses.Length == 0)
            {
                LogManager.GetCurrentClassLogger().Warn("No 8ball responses found in config.json! Defaulting to built-in responses...");
            }else
            {
                EightBallResponses = Program.EightBallResponses;
                LogManager.GetCurrentClassLogger().Info($"Loaded {EightBallResponses.Length} 8ball response(s).");
            }
        }

        [Command("8ball")]
        [Summary("Ask the 8ball a question!")]
        public async Task EightBall([Remainder] [Summary("The question")] string question)
        {
            Random random = new Random();
            string answer = EightBallResponses[random.Next(EightBallResponses.Length)];
            await ReplyAsync("", false, new EmbedBuilder()
                .AddField($":question: {StringResourceHandler.GetTextStatic("Fun", "8ball_question")}", question)
                .AddField($":8ball: {StringResourceHandler.GetTextStatic("Fun", "8ball_answer")}", answer)
                .Build());
        }
        [Command("choose")]
        [Summary("Choose an option from a given set")]
        public async Task Choose([Remainder] [Summary("Options (seperate with `;`)")] string options)
        {
            Random random = new Random();
            string[] optionsIndiv = options.Split(';');
            string choice = optionsIndiv[random.Next(optionsIndiv.Length)];
            await ReplyAsync("", false, new EmbedBuilder()
                .AddField(":thinking:", choice)
                .Build());
        }
    }
}
