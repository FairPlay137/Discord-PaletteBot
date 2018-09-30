﻿using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using NLog;
using Discord.Commands;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Immutable;
using System.IO;
using Discord.Net;
using PaletteBot.Common;
using PaletteBot.Common.ModuleBehaviors;

namespace PaletteBot.Services
{
    public class CommandHandler : IPaletteBotService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly Logger _log;
        private readonly IBotConfiguration _config;
        private IPaletteServiceProvider _services;
        public string DefaultPrefix { get; private set; }
        private readonly PaletteBot _bot;

        public CommandHandler(DiscordSocketClient client, CommandService commandService, IBotConfiguration config, PaletteBot bot)
        {
            _client = client;
            _commandService = commandService;
            _config = config;
            _bot = bot;

            _log = LogManager.GetCurrentClassLogger();

            DefaultPrefix = config.DefaultPrefix;
        }

        public string GetPrefix()
        {
            return DefaultPrefix;
        }
        public string SetPrefix(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentNullException(nameof(prefix));
            prefix = prefix.ToLowerInvariant();
            return DefaultPrefix = prefix;
        }

        /*private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            if (!(message.HasStringPrefix(DefaultPrefix, ref argPos))) return;
            // Create a Command Context

            var context = new SocketCommandContext(_client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await _commandService.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
            {
                string errtext;
                switch (result.Error)
                {
                    case CommandError.UnknownCommand: //Unknown command
                        errtext = StringResourceHandler.GetTextStatic("err", "unknownCommand");
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
        }*/

        private async Task MessageReceivedHandler(SocketMessage msg)
        {
            try
            {
                if (msg == null)
                    return;
                if (msg.Author.Equals(_client.CurrentUser) || !_bot.Ready.Task.IsCompleted)
                    return;
                if (!(msg is SocketUserMessage usrMsg)) // don't execute if it's a system message
                    return;

                var channel = msg.Channel as ISocketMessageChannel;
                var guild = (msg.Channel as SocketTextChannel)?.Guild;

                await TryExecuteCommand(guild, channel, usrMsg);
            }
            catch(Exception ex)
            {
                _log.Warn("Exception occurred in CommandHandler!");
                _log.Warn(ex);
                if (ex.InnerException != null)
                {
                    _log.Warn("Inner Exception:");
                    _log.Warn(ex.InnerException);
                }
            }
        }

        public async Task TryExecuteCommand(SocketGuild guild, ISocketMessageChannel channel, SocketUserMessage usrMsg)
        {
            
            foreach (var svc in _services)
            {
                if(svc is IPreXBlocker blocker &&
                    await blocker.TryBlockEarly(guild, usrMsg).ConfigureAwait(false))
                {
                    _log.Info("**BLOCKED USER");
                    _log.Info("User: "+usrMsg.Author);
                    _log.Info("Message: "+usrMsg.Content);
                    _log.Info("Service: "+svc.GetType().Name);
                    return;
                }
            }
            
            foreach (var svc in _services)
            {
                if (svc is IPreXBlockerExecutor exec &&
                    await exec.TryExecuteEarly(_client, guild, usrMsg).ConfigureAwait(false))
                {
                    _log.Info("**REACTION EXECUTED");
                    _log.Info("User: " + usrMsg.Author);
                    _log.Info("Message: " + usrMsg.Content);
                    _log.Info("Service: " + svc.GetType().Name);
                    return;
                }
            }
            
            string messageContent = usrMsg.Content;
            foreach (var svc in _services)
            {
                string newContent;
                if (svc is IInputOverrider exec &&
                    (newContent = await exec.TransformInput(guild, usrMsg.Channel, usrMsg.Author, messageContent).ConfigureAwait(false)) != messageContent.ToLowerInvariant())
                {
                    messageContent = newContent;
                    _log.Debug($"Message content overwritten with [{0}] by service [{1}]",newContent,svc.GetType().Name);
                    break;
                }
            }
            
            var prefix = DefaultPrefix; //TODO: Once server prefixes are added, change up this code.
            
            if(messageContent.StartsWith(prefix))
            {
                var result = await ExecuteCommandAsync(new SocketCommandContext(_client, usrMsg), messageContent, prefix.Length, _services, MultiMatchHandling.Best);
                if(result.IsSuccess)
                {
                    _log.Info("**COMMAND EXECUTED");
                    _log.Info("User: " + usrMsg.Author + " ("+usrMsg.Author.Id+")");
                    _log.Info("Server: " + (channel == null ? "[Direct]" : guild.Name + " (" + guild.Id + ")"));
                    _log.Info("Channel: " + (channel == null ? "[Direct]" : channel.Name + " (" + channel.Id + ")"));
                    _log.Info("Message: " + usrMsg.Content);
                    return;
                }
                else if(result.Error != null)
                {
                    _log.Warn("**COMMAND ERRORED");
                    _log.Warn("User: " + usrMsg.Author + " (" + usrMsg.Author.Id + ")");
                    _log.Warn("Server: " + (channel == null ? "[Direct]" : guild.Name + " (" + guild.Id + ")"));
                    _log.Warn("Channel: " + (channel == null ? "[Direct]" : channel.Name + " (" + channel.Id + ")"));
                    _log.Warn("Message: " + usrMsg.Content);
                    _log.Warn("Error: " + result.ErrorReason);

                    string errtext;
                    switch (result.Error)
                    {
                        case CommandError.UnknownCommand: //Unknown command
                            errtext = StringResourceHandler.GetTextStatic("err", "unknownCommand", prefix);
                            break;
                        //case CommandError.MultipleMatches: //oops
                        //    errtext = StringResourceHandler.GetTextStatic("err", "multipleCommandDefs");
                        //    break;
                        case CommandError.Exception: //Exception during command processing
                            errtext = StringResourceHandler.GetTextStatic("err", "exception", result.ErrorReason);
                            break;
                        default:
                            errtext = result.ErrorReason;
                            break;
                    }
                    await channel.SendMessageAsync($":no_entry: `{errtext}`");
                }
            }
            else
            {
                // ignore
            }

            foreach (var svc in _services)
            {
                if (svc is IPostXExecutor exec)
                {
                    await exec.LateExecute(_client, guild, usrMsg).ConfigureAwait(false);
                }
            }
        }

        public Task<IResult> ExecuteCommandAsync(SocketCommandContext context, string input, int argPos, IServiceProvider serviceProvider, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception)
            => ExecuteCommand(context, input.Substring(argPos), serviceProvider, multiMatchHandling);

        public async Task<IResult> ExecuteCommand(SocketCommandContext context, string input, IServiceProvider services, MultiMatchHandling multiMatchHandling = MultiMatchHandling.Exception)
        {
            var searchResult = _commandService.Search(context, input);
            if (!searchResult.IsSuccess)
                return searchResult;
            var commands = searchResult.Commands;
            var preconditionResults = new Dictionary<CommandMatch, PreconditionResult>();

            foreach (var match in commands)
            {
                preconditionResults[match] = await match.Command.CheckPreconditionsAsync(context, services).ConfigureAwait(false);
            }

            var successfulPreconditions = preconditionResults
                .Where(x => x.Value.IsSuccess)
                .ToArray();

            if (successfulPreconditions.Length == 0)
            {
                //All preconditions failed, return the one from the highest priority command
                var bestCandidate = preconditionResults
                    .OrderByDescending(x => x.Key.Command.Priority)
                    .FirstOrDefault(x => !x.Value.IsSuccess);
                return bestCandidate.Value;
            }

            var parseResultsDict = new Dictionary<CommandMatch, ParseResult>();
            foreach (var pair in successfulPreconditions)
            {
                var parseResult = await pair.Key.ParseAsync(context, searchResult, pair.Value, services).ConfigureAwait(false);

                if (parseResult.Error == CommandError.MultipleMatches)
                {
                    IReadOnlyList<TypeReaderValue> argList, paramList;
                    switch (multiMatchHandling)
                    {
                        case MultiMatchHandling.Best:
                            argList = parseResult.ArgValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                            paramList = parseResult.ParamValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                            parseResult = ParseResult.FromSuccess(argList, paramList);
                            break;
                    }
                }

                parseResultsDict[pair.Key] = parseResult;
            }
            // Calculates the 'score' of a command given a parse result
            float CalculateScore(CommandMatch match, ParseResult parseResult)
            {
                float argValuesScore = 0, paramValuesScore = 0;

                if (match.Command.Parameters.Count > 0)
                {
                    var argValuesSum = parseResult.ArgValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;
                    var paramValuesSum = parseResult.ParamValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;

                    argValuesScore = argValuesSum / match.Command.Parameters.Count;
                    paramValuesScore = paramValuesSum / match.Command.Parameters.Count;
                }

                var totalArgsScore = (argValuesScore + paramValuesScore) / 2;
                return match.Command.Priority + totalArgsScore * 0.99f;
            }

            //Order the parse results by their score so that we choose the most likely result to execute
            var parseResults = parseResultsDict
                .OrderByDescending(x => CalculateScore(x.Key, x.Value));

            var successfulParses = parseResults
                .Where(x => x.Value.IsSuccess)
                .ToArray();

            if (successfulParses.Length == 0)
            {
                //All parses failed, return the one from the highest priority command, using score as a tie breaker
                var bestMatch = parseResults
                    .FirstOrDefault(x => !x.Value.IsSuccess);
                return bestMatch.Value;
            }

            var chosenOverload = successfulParses[0];
            return await chosenOverload.Key.ExecuteAsync(context, chosenOverload.Value, services).ConfigureAwait(false);
        }

        public void AddServices(IPaletteServiceProvider services)
        {
            _services = services;
        }

        public Task StartHandling()
        {
            _client.MessageReceived += (msg) => { var _ = Task.Run(() => MessageReceivedHandler(msg)); return Task.CompletedTask; };
            return Task.CompletedTask;
        }
    }
}
