﻿using Newtonsoft.Json;
using System;
using System.IO;
using Discord;
using System.Linq;
using NLog;
using System.Collections.Generic;
using System.Text;
using PaletteBot.Common;

namespace PaletteBot.Services.Impl
{
    public class BotConfiguration : IBotConfiguration
    {
        private Logger _log;

        public string BotName { get; private set; }
        public string BotToken { get; private set; }
        public ulong BotOwnerID { get; private set; }

        public int TotalShards { get; private set; }

        public string DefaultPlayingString { get; private set; }
        public string DefaultPrefix { get; set; }

        public bool VerboseErrors { get; set; }

        public string[] EightBallResponses { get; private set; }

        public Dictionary<string, List<string>> CustomReactions { get; set; }

        public bool RotatePlayingStatuses { get; set; }
        public string[] PlayingStatuses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public BotConfiguration()
        {
            _log = LogManager.GetCurrentClassLogger();

            var json = "";
            try
            {
                _log.Info("Loading config.json");
                using (var fs = File.OpenRead("config.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
            }
            catch (Exception e)
            {
                if (e is FileNotFoundException)
                {
                    _log.Error("config.json wasn't found! Copying from config-example.json...");
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

            BotToken = "";
            try
            {
                var cfgjson = JsonConvert.DeserializeObject<ConfigJsonStructure>(json);
                BotToken = cfgjson.Token;
                if ((BotToken == null) || (BotToken == ""))
                {
                    _log.Error("No token found in configuration!");
                    Console.WriteLine("A token is required in order to use PaletteBot.");
                    Console.WriteLine("To get a bot token, go to https://discordapp.com/developers/applications/me, then follow these steps:");
                    Console.WriteLine(" 1. Create a new app.");
                    Console.WriteLine(" 2. Create a bot user associated with your app.");
                    Console.WriteLine(" 3. Below where it says \"App Bot User\", you should see an option to show your bot user's token. Click it.");
                    Console.WriteLine(" 4. Copy the Bot Token (NOT THE CLIENT SECRET) to this prompt.");
                    Console.WriteLine("Enter your bot token below...");
                    BotToken = Console.ReadLine();
                    Console.WriteLine("Okay, this'll be saved into config.json.");
                }
                DefaultPrefix = cfgjson.CommandPrefix;
                if ((DefaultPrefix == null) || (DefaultPrefix == ""))
                {
                    _log.Warn("No default prefix found in configuration! Using \"pal:\"...");
                    DefaultPrefix = "pal:";
                }
                DefaultPlayingString = cfgjson.DefaultPlayingString;
                BotOwnerID = cfgjson.OwnerID;
                BotName = cfgjson.BotName;
                EightBallResponses = cfgjson.EightBallResponses;
                CustomReactions = cfgjson.CustomReactions;
                VerboseErrors = cfgjson.VerboseErrors;
                RotatePlayingStatuses = cfgjson.RotatePlaying;
                PlayingStatuses = cfgjson.PlayingStatuses;
                TotalShards = cfgjson.TotalShards;
                _log.Info("Loaded.");
                SaveConfig(false);
            }
            catch (Exception e)
            {
                _log.Error(e, "Exception during JSON loading. Using default values...");
                if ((BotToken == null) || (BotToken == ""))
                {
                    Console.WriteLine("A token is required in order to use PaletteBot.");
                    Console.WriteLine("To get a bot token, go to https://discordapp.com/developers/applications/me, then follow these steps:");
                    Console.WriteLine(" 1. Create a new app.");
                    Console.WriteLine(" 2. Create a bot user associated with your app.");
                    Console.WriteLine(" 3. Below where it says \"App Bot User\", you should see an option to show your bot user's token. Click it.");
                    Console.WriteLine(" 4. Copy the Bot Token (NOT THE CLIENT SECRET) to this prompt.");
                    Console.WriteLine("Enter your bot token below...");
                    Console.Write("> ");
                    BotToken = Console.ReadLine();
                    Console.WriteLine("Okay, I'll save that whenever I need to save config.json.");
                }
                if ((DefaultPrefix == null) || (DefaultPrefix == ""))
                    DefaultPrefix = "pal:";
                if ((DefaultPlayingString == null) || (DefaultPlayingString == ""))
                    DefaultPlayingString = "pal:help";
                Console.WriteLine("Would you like to save these into config.json now? YOUR OLD CONFIG.JSON WILL BE OVERWRITTEN.");
                Console.WriteLine("If you want, make a backup of your current config.json before continuing.");
                Console.Write("(Y/N)> ");
                string choice = Console.ReadLine();
                if (choice.ToLower().StartsWith("y"))
                    SaveConfig(true);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public bool ReloadConfig(bool verbose)
        {
            var json = "";
            try
            {
                if (verbose)
                    _log.Info("Reloading config.json...");
                using (var fs = File.OpenRead("config.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
            }
            catch(Exception e)
            {
                _log.Error("Reload failed!");
                _log.Error(e);
                return false;
            }
            try
            {
                var cfgjson = JsonConvert.DeserializeObject<ConfigJsonStructure>(json);
                string oldtoken = BotToken;
                BotToken = cfgjson.Token;
                if ((BotToken == null) || (BotToken == ""))
                {
                    _log.Warn("No token found in configuration! This'll cause problems later on! It is recommended to shut down the bot NOW and correct the issue.");
                }
                else if(BotToken != oldtoken)
                    _log.Warn("The bot token specified in the configuration seems to be different than before. This'll cause problems later on! It is recommended to shut down the bot NOW and correct the issue.");
                DefaultPrefix = cfgjson.CommandPrefix;
                if ((DefaultPrefix == null) || (DefaultPrefix == ""))
                    DefaultPrefix = "pal:";
                DefaultPlayingString = cfgjson.DefaultPlayingString;
                if ((DefaultPlayingString == null) || (DefaultPlayingString == ""))
                    DefaultPlayingString = "pal:help";
                BotOwnerID = cfgjson.OwnerID;
                BotName = cfgjson.BotName;
                if ((BotName == null) || (BotName == ""))
                    BotName = "PaletteBot";
                EightBallResponses = cfgjson.EightBallResponses;
                CustomReactions = cfgjson.CustomReactions;
                VerboseErrors = cfgjson.VerboseErrors;
                RotatePlayingStatuses = cfgjson.RotatePlaying;
                PlayingStatuses = cfgjson.PlayingStatuses;
                TotalShards = cfgjson.TotalShards;
            }
            catch (Exception e)
            {
                _log.Error("Reload failed!");
                _log.Error(e);
                return false;
            }
            if (verbose)
                _log.Info("Reload complete!");
            return true;
        }
        public bool SaveConfig(bool verbose)
        {
            if(verbose)
                _log.Info("Saving config.json...");
            try
            {
                if(TotalShards<1)
                {
                    _log.Warn($"TotalShards was at an invalid value! ({TotalShards}) Resetting to 1...");
                    TotalShards = 1;
                }
                ConfigJsonStructure cfg = new ConfigJsonStructure
                {
                    Token = BotToken,
                    CommandPrefix = DefaultPrefix,
                    DefaultPlayingString = DefaultPlayingString,
                    OwnerID = BotOwnerID,
                    BotName = BotName,
                    VerboseErrors = VerboseErrors,
                    EightBallResponses = EightBallResponses,
                    CustomReactions = CustomReactions,
                    RotatePlaying = RotatePlayingStatuses,
                    PlayingStatuses = PlayingStatuses,
                    TotalShards = TotalShards
                };
                string json = JsonConvert.SerializeObject(cfg, Formatting.Indented);
                File.WriteAllText("config.json", json);
                if (verbose)
                    _log.Info("Save complete!");
                return true;
            }
            catch (Exception e)
            {
                _log.Error("Couldn't save config.json!");
                _log.Error(e);
                return false;
            }
        }
    }
}
