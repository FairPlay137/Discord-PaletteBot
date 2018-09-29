using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace PaletteBot.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class OwnerOnlyPrecondition : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            return Task.FromResult(PreconditionResult.FromError("Unimplemented precondition OwnerOnlyPrecondition"));
        }
    }
}
