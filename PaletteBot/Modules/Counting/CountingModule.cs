using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Diagnostics;
using PaletteBot.Common;

//   Counting game for PaletteBot
//You can start a game in a text channel, and the objective is to get as high as you can while abiding to these rules:
// - You must not skip or repeat a number
// - Once you've posted one number, you must wait until another person to post the next number.
// - The numbers must be in decimal format only. No written out numbers or hexadecimal numbers.
//If you break one of these rules, you lose a life. If you lose all your lives, you're eliminated from the game.

namespace PaletteBot.Modules
{
    public class Counting : PaletteBotModuleBase<SocketCommandContext>
    {
        //TODO: Start this feature
    }
}
