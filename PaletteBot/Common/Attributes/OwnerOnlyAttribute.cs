using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace PaletteBot.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class OwnerOnlyAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (Program.OwnerID == 0)
            {
                return Task.FromResult(PreconditionResult.FromError(StringResourceHandler.GetTextStatic("err", "noBotOwner")));
            }
            else
            {
                if (Program.OwnerID != context.Message.Author.Id)
                {
                    return Task.FromResult(PreconditionResult.FromError(StringResourceHandler.GetTextStatic("err", "notBotOwner")));
                }
                else
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }
            }
        }
    }
}
