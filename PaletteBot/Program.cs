using System;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Newtonsoft.Json;
using System.IO;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NLog.Config;
using NLog.Targets;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PaletteBot.Common;
using PaletteBot.Services;
using Discord.Net.Providers.WS4Net;
using System.Collections.Generic;

namespace PaletteBot
{
    class Program
    {
        static void Main(string[] args)
            => new PaletteBot().StartAndBlockAsync(args).GetAwaiter().GetResult();
        /*
        public static CommandService _commands;
        private DiscordSocketClient _client;
        private IPaletteServiceProvider _services;

        public Logger _log;

        private static string token = null;
        public static string defaultPlayingString = null;
        public static string prefix = null;
        public static ulong OwnerID = 0;
        //public static string databaseKey = null;
        public static string botName = "PaletteBot";
        public static bool verboseErrors = false;
        public static string[] EightBallResponses = { };
        public static Dictionary<string, List<string>> CustomReactions = new Dictionary<string, List<string>>();

        public static DateTime StartTime = DateTime.Now; //An easy way to calculate the startup time
        public static DateTime ConnectedAtTime;

        public static void SetupLogger()
        {
            var logConfig = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget()
            {
                Layout = @"${date:format=HH\:mm\:ss} ${logger} | ${message}"
            };
            logConfig.AddTarget("Console", consoleTarget);

            logConfig.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));

            LogManager.Configuration = logConfig;
        }

        private Task Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Info:
                    _log.Info(msg.Source + " | " + msg.Message);
                    break;
                case LogSeverity.Error:
                    _log.Error(msg.Source + " | " + msg.Message);
                    break;
                case LogSeverity.Critical:
                    _log.Fatal(msg.Source + " | " + msg.Message);
                    break;
                case LogSeverity.Debug:
                    _log.Debug(msg.Source + " | " + msg.Message);
                    break;
                case LogSeverity.Verbose:
                    _log.Info(msg.Source + " | " + msg.Message);
                    break;
                case LogSeverity.Warning:
                    _log.Warn(msg.Source + " | " + msg.Message);
                    break;
                default: //Hopefully this'll never happen
                    _log.Info(msg.Source + " | " + msg.Message + $" (INVALID SEVERITY LEVEL {msg.Severity.ToString()})");
                    break;
            }

            if (msg.Exception != null) //If there's an exception, output it to the console.
                _log.Warn(msg.Exception);
            
            return Task.CompletedTask;
        }
        public async Task StartAsync()
        {
            SetupLogger();
            _log = LogManager.GetCurrentClassLogger();

            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                WebSocketProvider = WS4NetProvider.Instance, //To maintain compatibility with Windows 7. Mono doesn't like this very much
                LogLevel = LogSeverity.Info //Not too verbose, since we want to utilize the logging ourselves too, and we don't want it buried underneath a ton of Discord.NET logs
            });
            _commands = new CommandService();

            _client.Log += Log; //Route Discord.NET logs to the console

            _log.Info($"PaletteBot v{GetType().Assembly.GetName().Version} is starting up...");

            var json = "";
            try
            {
                _log.Info("Loading config.json");
                using (var fs = File.OpenRead("config.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = await sr.ReadToEndAsync();
            }
            catch (Exception e)
            {
                if (e is FileNotFoundException)
                {
                    _log.Warn("config.json wasn't found! Copying from config-example.json...");
                    try
                    {
                        File.Copy("config-example.json", "config.json");
                    }
                    catch (Exception ee)
                    {
                        _log.Fatal($"FATAL ERROR: Unable to copy config-example.json to config.json: {ee.Message}");
                        Console.ReadKey();
                        Environment.Exit(0);
                    }
                }
                else
                {
                    _log.Fatal($"FATAL ERROR: Unable to load config.json: {e.Message}");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
            token = "";
            try
            {
                var cfgjson = JsonConvert.DeserializeObject<ConfigJson>(json);
                token = cfgjson.Token;
                if ((token == null) || (token == ""))
                {
                    _log.Warn("No token found in configuration!");
                    Console.WriteLine("A token is required in order to use PaletteBot.");
                    Console.WriteLine("To get a bot token, go to https://discordapp.com/developers/applications/me, then follow these steps:");
                    Console.WriteLine(" 1. Create a new app.");
                    Console.WriteLine(" 2. Create a bot user associated with your app.");
                    Console.WriteLine(" 3. Below where it says \"App Bot User\", you should see an option to show your bot user's token. Click it.");
                    Console.WriteLine(" 4. Copy the Bot Token (NOT THE CLIENT SECRET) to this prompt.");
                    Console.WriteLine("Enter your bot token below...");
                    token = Console.ReadLine();
                    Console.WriteLine("Okay, this'll be saved into config.json.");
                }
                prefix = cfgjson.CommandPrefix;
                if ((prefix == null) || (prefix == ""))
                {
                    _log.Warn("No prefix found in configuration! Resetting to \"pal:\"...");
                    prefix = "pal:";
                }
                defaultPlayingString = cfgjson.DefaultPlayingString;
                OwnerID = cfgjson.OwnerID;
                //databaseKey = cfgjson.DatabaseKey;
                botName = cfgjson.BotName;
                EightBallResponses = cfgjson.EightBallResponses;
                CustomReactions = cfgjson.CustomReactions;
                SaveConfig();
            }
            catch (Exception e){
                _log.Error(e, "Exception during JSON loading. Setting unassigned values to default...");
                if ((token == null) || (token == ""))
                {
                    Console.WriteLine("A token is required in order to use PaletteBot.");
                    Console.WriteLine("To get a bot token, go to https://discordapp.com/developers/applications/me, then follow these steps:");
                    Console.WriteLine(" 1. Create a new app.");
                    Console.WriteLine(" 2. Create a bot user associated with your app.");
                    Console.WriteLine(" 3. Below where it says \"App Bot User\", you should see an option to show your bot user's token. Click it.");
                    Console.WriteLine(" 4. Copy the Bot Token (NOT THE CLIENT SECRET) to this prompt.");
                    Console.WriteLine("Enter your bot token below...");
                    Console.Write("> ");
                    token = Console.ReadLine();
                    Console.WriteLine("Okay, I'll save that whenever I need to save config.json.");
                }
                if ((prefix == null) || (prefix == ""))
                    prefix = "pal:";
                if ((defaultPlayingString == null) || (defaultPlayingString == ""))
                    defaultPlayingString = "pal:help";
                //if (databaseKey == null)
                //    databaseKey = "";
                Console.WriteLine("Would you like to save these into config.json now? YOUR OLD CONFIG.JSON WILL BE OVERWRITTEN.");
                Console.WriteLine("If you want, make a backup of your current config.json before continuing.");
                Console.Write("(Y/N)> ");
                string choice = Console.ReadLine();
                if(choice.ToLower().StartsWith("y"))
                    SaveConfig();
            }

            _client.JoinedGuild += GuildJoin;
            _client.LeftGuild += GuildLeave;
            
            if (OwnerID == 0)
                _log.Warn("An owner ID has not been specified! You will need to shut down the bot manually.");

            _log.Info("Initializing modules...");

            _services = new PaletteServiceProvider.ServiceProviderBuilder()
                .AddManual(_client)
                .AddManual(_commands)
                .LoadFrom(Assembly.GetEntryAssembly())
                .Build();

            await InstallCommandsAsync();

            _commands.Log += Log;
            _commands.CommandExecuted += LogCommandExecution;

            _log.Info("Logging in...");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await _client.SetGameAsync(defaultPlayingString);
            ConnectedAtTime = DateTime.Now;
            _log.Info($"Booted in {new TimeSpan(ConnectedAtTime.Ticks - StartTime.Ticks).TotalSeconds} seconds.");
            await Task.Delay(-1);
        }
        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived Event into our Command Handler
            _client.MessageReceived += HandleCommandAsync;
            // Discover all of the commands in this assembly and load them.
            //await _commands.AddModulesAsync(Assembly.GetEntryAssembly()); //Pre-2.0.0beta2 format
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // If it's part of a custom reaction, skip command processing
                foreach (var k in CustomReactions.Keys)
                {
                    string[] key = k.ToLower().Trim().Split(' ');
                    string[] msg = message.Content.ToLower().Trim().Split(' ');
                    bool matchesKey = true;
                    int index = 0;
                    try
                    {
                        foreach (string kword in key)
                        {
                            string keyw;
                            switch (kword)
                            {
                                case "%mention%":
                                    keyw = _client.CurrentUser.Mention; break;
                                case "%user%":
                                    keyw = message.Author.Mention; break;
                                default:
                                    keyw = kword; break;
                            }
                            if (kword != msg[index])
                                matchesKey = false;
                            index++;
                        }
                    }
                    catch(Exception) //TODO: This is a sloppy workaround and may not be the best solution; rewrite the code
                    { }
                    if (matchesKey)
                        return;
                }

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            if (!(message.HasStringPrefix(prefix, ref argPos))) return;
            // Create a Command Context

            var context = new SocketCommandContext(_client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
            {
                string errtext;
                switch (result.Error)
                {
                    case CommandError.UnknownCommand: //Unknown command
                        errtext = StringResourceHandler.GetTextStatic("err","unknownCommand");
                        break;
                    case CommandError.MultipleMatches: //oops
                        errtext = StringResourceHandler.GetTextStatic("err", "multipleCommandDefs");
                        break;
                    case CommandError.Exception: //Exception during command processing
                        errtext = StringResourceHandler.GetTextStatic("err", "exception", result.ErrorReason);
                        break;
                    default: //Other situations which I haven't accounted for (or are better shown as-is)
                        errtext = result.ErrorReason;
                        break;
                }
                await context.Channel.SendMessageAsync($":no_entry: `{errtext}`");
            }
        }
        private Task LogCommandExecution(CommandInfo cmdinfo, ICommandContext context, IResult result)
        {
            if (result.IsSuccess)
            {
                _log.Info(">>COMMAND EXECUTED");
                _log.Info($" Cmd: \"{cmdinfo.Name}\" in module \"{cmdinfo.Module.Name}\"");
                _log.Info($" Msg: \"{context.Message}\"");
                _log.Info($" Usr: @{context.User.Username}#{context.User.Discriminator} ({context.User.Id})");
                if (context.Guild == null)
                    _log.Info(" [Sent in DMs]");
                else
                {
                    _log.Info($" Srvr: \"{context.Guild.Name}\" ({context.Guild.Id})");
                    _log.Info($" Chnl: #{context.Channel.Name} ({context.Channel.Id})");
                }
            }
            else
            {
                _log.Warn($">>COMMAND ERRORED: {result.ErrorReason}");
                _log.Warn($" Cmd: \"{cmdinfo.Name}\" in module \"{cmdinfo.Module.Name}\"");
                _log.Warn($" Msg: \"{context.Message}\"");
                _log.Warn($" Usr: @{context.User.Username}#{context.User.Discriminator} ({context.User.Id})");
                if (context.Guild == null)
                    _log.Info(" [Sent in DMs]");
                else
                {
                    _log.Info($" Srvr: \"{context.Guild.Name}\" ({context.Guild.Id})");
                    _log.Info($" Chnl: #{context.Channel.Name} ({context.Channel.Id})");
                }
            }
            return Task.CompletedTask;
        }
        private Task GuildJoin(SocketGuild guild)
        {
            _log.Info($"Joined guild: {guild.Name} ({guild.Id})");
            _log.Info($" {guild.TextChannels.Count} Text Channel(s)");
            _log.Info($" {guild.VoiceChannels.Count} Voice Channel(s)");
            _log.Info($" {guild.Users.Count} user(s)");
            _log.Info($" Owner: @{guild.Owner.Username}#{guild.Owner.Discriminator} ({guild.OwnerId})");
            _log.Info($" Created {guild.CreatedAt}");
            return Task.CompletedTask;
        }
        private Task GuildLeave(SocketGuild guild)
        {
            _log.Info($"Left guild: {guild.Name} ({guild.Id})");
            return Task.CompletedTask;
        }
        
        private void SaveConfig()
        {
            _log.Info("Saving config.json...");
            try
            {
                ConfigJson cfg = new ConfigJson();
                cfg.Token = token;
                cfg.CommandPrefix = prefix;
                cfg.DefaultPlayingString = defaultPlayingString;
                cfg.OwnerID = OwnerID;
                //cfg.DatabaseKey = databaseKey;
                cfg.BotName = botName;
                cfg.VerboseErrors = verboseErrors;
                cfg.EightBallResponses = EightBallResponses;
                cfg.CustomReactions = CustomReactions;
                string json = JsonConvert.SerializeObject(cfg, Formatting.Indented);
                //TODO: This doesn't actually save yet. Implement writing to config.json
                _log.Info("Save complete!");
            }
            catch(Exception e)
            {
                _log.Error("Save failed!");
                _log.Error(e);
            }
        }
    }
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("prefix")]
        public string CommandPrefix { get; set; }

        [JsonProperty("defaultplaying")]
        public string DefaultPlayingString { get; set; }

        [JsonProperty("ownerid")]
        public ulong OwnerID { get; set; }

        //[JsonProperty("databasekey")]
        //public string DatabaseKey { get; set; }

        [JsonProperty("botname")]
        public string BotName { get; set; }

        [JsonProperty("verboseerrors")]
        public bool VerboseErrors { get; set; }

        [JsonProperty("8ballResponses")]
        public string[] EightBallResponses { get; set; }

        [JsonProperty("customReactions")]
        public Dictionary<string, List<string>> CustomReactions { get; set; }*/
    }
}
