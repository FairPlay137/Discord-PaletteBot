# PaletteBot - An open-source, multipurpose Discord bot
[![Build Status](https://travis-ci.org/FairPlay137/Discord-PaletteBot.svg?branch=master)](https://travis-ci.org/FairPlay137/Discord-PaletteBot)

PaletteBot is a Discord Chatbot written by FairPlay137-TTS using C# along with the Discord.NET library. Currently, this is a very incomplete and WIP project. However, it *is* public, but it isn't ready for widespread usage yet, so I wouldn't recommend adding PaletteBot to your server at the moment.

* [PaletteBot Website](https://fairplay137.github.io/PaletteBot-Webpage/)
* [Add PaletteBot to your server](https://discordapp.com/oauth2/authorize?client_id=385963697631395840&permissions=8&scope=bot) (NOT RECOMMENDED RIGHT NOW)
* [Join the PaletteBot Central server](https://discord.gg/dach9vB)
* Documentation (Coming soon)

## How to compile for self-hosting?

### To compile, you will need:
* Visual Studio 2017 Community (or any version of VS2017 or VS2015 you own)

### Steps that need to be taken:
*(assuming you already created the bot's application on [this page](https://discordapp.com/developers/applications/me))*
* Open `PaletteBot.sln` with Visual Studio 2017.
* Hit F5 to compile PaletteBot.
* Head to where the binaries for PaletteBot have been made.
* If `config.json` doesn't exist in the folder already, make a copy of `config-example.json` and call it `config.json`.
* Check to see if your `config.json` has the bot token in there. If not, add your bot's token (NOT THE CLIENT ID OR THE CLIENT SECRET) and save it. (DO NOT PUT IT AS THE DATABASE KEY; IT WON'T WORK)
* Run PaletteBot.

## Development Progress
### ✔ Finished features
* Help module (A bit of a stretch, as command help still needs to be finished)
* Basic bot owner commands
* Some utility commands (ping, stats, invite)
### 🛠 Work-in-progress features
* Akinator module (.NET-based API is yet to be worked on)
* Basic Moderation commands (kick, ban, softban, etc.)
* Custom Reactions module (still need a way for a bot owner to edit and remove custom reactions)
### 💭 Planned features
* Palette module (will allow for users to assign themselves color roles) **Backlog**
* *Twitch Notification module*
* Counting game module
* Text Portal command (allows for communication across two servers)
* User join and leave messages
* *Module enabling/disabling* **(Backlog)**
* *Moderation logs* **(Backlog)**
* *Automatic Slowmode*

*Italicized entries mean no strings currently exist for these features*
