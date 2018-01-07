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

namespace PaletteBot
{
    class Program
    {
        public static CommandService _commands;
        private DiscordSocketClient _client;
        private IServiceProvider _services;

        public Logger _log;

        public static string defaultPlayingString = null;
        public static string prefix = null;
        public static ulong OwnerID = 0;
        public static string databaseKey = null;

        public static DateTime StartTime = DateTime.Now;
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

        private async Task Log(LogMessage msg)
            //TODO: Make this synchronous
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
                default:
                    _log.Info(msg.Source + " | " + msg.Message + $" (INVALID SEVERITY LEVEL {msg.Severity.ToString()})");
                    break;
            }

            if (msg.Exception != null)
                _log.Warn(msg.Exception);
        }
        public async Task StartAsync()
        {
            SetupLogger();
            _log = LogManager.GetCurrentClassLogger();

            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _client.Log += Log;

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
            string token = "";
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
                    Console.WriteLine("To avoid getting this message later, please put the bot token into config.json.");
                }
                prefix = cfgjson.CommandPrefix;
                if ((prefix == null) || (prefix == ""))
                {
                    _log.Warn("No prefix found in configuration! Resetting to \"pal:\"...");
                    prefix = "pal:";
                }
                defaultPlayingString = cfgjson.DefaultPlayingString;
                OwnerID = cfgjson.OwnerID;
                databaseKey = cfgjson.DatabaseKey;
            }catch (Exception e){
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
                    token = Console.ReadLine();
                }
                if ((prefix == null) || (prefix == ""))
                    prefix = "pal:";
                if ((defaultPlayingString == null) || (defaultPlayingString == ""))
                    defaultPlayingString = "pal:help";
                if (databaseKey == null)
                    databaseKey = "";
            }

            _client.JoinedGuild += GuildJoin;
            _client.LeftGuild += GuildLeave;

            if (databaseKey == "")
                _log.Warn("The database key is blank! This could pose a potential security risk!");
            if (OwnerID == 0)
                _log.Warn("An owner ID has not been specified!");

            _log.Info("Initializing modules...");

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

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
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
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
                    case CommandError.UnknownCommand:
                        errtext = StringResourceHandler.GetTextStatic("err","unknownCommand");
                        break;
                    case CommandError.UnmetPrecondition:
                        errtext = StringResourceHandler.GetTextStatic("err", "unmetPrecondition");
                        break;
                    case CommandError.MultipleMatches:
                        errtext = StringResourceHandler.GetTextStatic("err", "multipleCommandDefs");
                        break;
                    case CommandError.Exception:
                        errtext = StringResourceHandler.GetTextStatic("err", "exception", result.ErrorReason);
                        break;
                    default:
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
                if(context.Guild == null)
                    _log.Info(" Srvr: (PRIVATE)");
                else
                    _log.Info($" Srvr: \"{context.Guild.Name}\" ({context.Guild.Id})");
            }
            else
            {
                _log.Warn($">>COMMAND ERRORED: {result.ErrorReason}");
                _log.Warn($" Cmd: \"{cmdinfo.Name}\" in module \"{cmdinfo.Module.Name}\"");
                _log.Warn($" Msg: \"{context.Message}\"");
                _log.Warn($" Usr: @{context.User.Username}#{context.User.Discriminator} ({context.User.Id})");
                if (context.Guild == null)
                    _log.Info(" Srvr: (PRIVATE)");
                else
                    _log.Warn($" Srvr: \"{context.Guild.Name}\" ({context.Guild.Id})");
            }
            return Task.CompletedTask;
        }
        private Task GuildJoin(SocketGuild guild)
        {
            _log.Info($"Joined guild: {guild.Name} ({guild.Id})");
            _log.Info($" {guild.TextChannels.Count} Text Channel(s)");
            _log.Info($" {guild.VoiceChannels.Count} Voice Channel(s)");
            _log.Info($" {guild.Users.Count} users");
            _log.Info($" Owner: @{guild.Owner.Username}#{guild.Owner.Discriminator} ({guild.OwnerId})");
            _log.Info($" Created {guild.CreatedAt}");
            return Task.CompletedTask;
        }
        private Task GuildLeave(SocketGuild guild)
        {
            _log.Info($"Left guild: {guild.Name} ({guild.Id})");
            return Task.CompletedTask;
        }
        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();
    }
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string CommandPrefix { get; private set; }

        [JsonProperty("defaultplaying")]
        public string DefaultPlayingString { get; private set; }

        [JsonProperty("ownerid")]
        public ulong OwnerID { get; private set; }

        [JsonProperty("databasekey")]
        public string DatabaseKey { get; private set; }
    }
}
