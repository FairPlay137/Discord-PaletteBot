using System.Threading.Tasks;

namespace PaletteBot.Services
{
    public interface IPaletteBotService
    {

    }

    public interface IUnloadableService
    {
        Task Unload();
    }
}
