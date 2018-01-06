using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace PaletteBot
{
    public class Fun : ModuleBase<SocketCommandContext>
    {
        string[] EightBallResponses = { "Yes.", "No.", "Ask again later.", "Maybe???", "It is certain", "Very doubtful",
                                        "NO - It'll cause a time paradox.", "Chances are as high as the sky.", "lolnope",
                                        "Obviously.", "Don't count on it.", "I'd say so.", "Received interference; try again."};
        [Command("8ball")]
        [Summary("Ask the 8ball a question!")]
        public async Task EightBall([Remainder] [Summary("The question")] string question)
        {
            Random random = new Random();
            string answer = EightBallResponses[random.Next(EightBallResponses.Length)];
            await ReplyAsync("", false, new EmbedBuilder()
                .AddField(StringResourceHandler.GetTextStatic("Fun", "8ball_question"), question)
                .AddField(StringResourceHandler.GetTextStatic("Fun", "8ball_answer"), answer)
                .Build());
        }
    }
}
