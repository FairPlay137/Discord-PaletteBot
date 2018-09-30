using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace PaletteBot.Common.ModuleBehaviors
{
    /// <summary>
    /// Description TBD
    /// </summary>
    public interface IPreXBlockerExecutor
    {
        Task<bool> TryExecuteEarly(DiscordSocketClient client, IGuild guild, IUserMessage msg);
    }
}
