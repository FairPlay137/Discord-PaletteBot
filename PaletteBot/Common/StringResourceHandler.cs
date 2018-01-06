using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using PaletteBot.Resources;
using NLog;

/************************************************************/
/*                 String fetching routines                 */
/************************************************************/

namespace PaletteBot.Common
{
    class StringResourceHandler
    {
        public static ResourceManager StringsRsManager { get; } = new ResourceManager(typeof(BotStrings));

        public static string GetTextStatic(string category, string key)
        {
            var text = StringsRsManager.GetString(category + "_" + key);

            if (string.IsNullOrWhiteSpace(text))
            {
                LogManager.GetCurrentClassLogger().Warn(category + "_" + key + " key is missing from BotStrings! Report this ASAP.");
                text = $"Error: Key {category + "_" + key} not found!";
            }
            return text;
        }
        public static string GetTextStatic(string category, string key,
            params object[] replacements)
        {
            try
            {
                return string.Format(GetTextStatic(category, key), replacements);
            }
            catch (FormatException)
            {
                LogManager.GetCurrentClassLogger().Warn(category + "_" + key + " key is not formatted correctly! Please report this.");
                return $"Error: Key {category + "_" + key} is not formatted correctly!";
            }
        }
    }
}
