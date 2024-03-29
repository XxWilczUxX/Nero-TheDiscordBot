﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Nero
{

    public class Info
    {
        public string Token { get; set; } = string.Empty;
        public ulong BasementGuildID { get; set; }
    }

    public class Names
    {
        public readonly string[] stats = { "Inteligence", "Reflex", "Agility", "Technology", "Charisma", "Will", "Luck", "Movement", "Body", "Empathy" };
        public readonly string[] abilityGroups = { "Long Range Weapons", "Body", "Education", "Control", "Observation", "Technology", "Social", "Meele", "Performances" };
        public readonly string[] skills = { "Heavy Weapons", "Long Weapons", "Short Weapons", "Archery", "Continous Fire", "Athletics", "Rubber Man", "Torture/Narcotic Tolerancy", "Stealth", "Dance", "Endurance", "Bureaucracy", "Deduction", "Language", "Composing", "Criminology", "Cryptography", "Accounting", "Science", "Animal Care", "Library Searching", "Making Deals", "Art of Survival", "Tactics", "Local Knowlege", "Education", "Riding", "Piloting", "Car Driving", "Sailing", "Lip Reading", "Concentration", "Perception", "Tracking", "Hiding/Finding an Item", "Cyber Engineering", "Electronics and Security", "Falsification", "Photography", "Pickpocketing", "Art", "Explosives", "Weapon Repair", "Land Vehicle Repair", "Water Vehicle Repair", "Air Vehicle Repair", "Lockpicking", "First Aid", "Basic Repairing", "Paramedics", "Attractiveness", "Trading", "Conversation", "Fashion", "Emotional Inteligence", "Persuasion", "Bribery", "Interrogation", "Semi-Literate Knowlege", "Fighting", "Meele Weapons", "Martial Arts", "Dodgeing", "Acting", "Playing an Instrument" };
        public readonly string[] subskills = { "Street Slang", "Your Area" };
        public readonly string[,] roles = {{ "Solo", "Netrunner", "Techie", "Media", "Cop", "Nomad", "Fixer", "Corporate", "Medtech", "Rockerboy / Rockergirl" }, { "Combat Sense", "Interface", "Jurry Rig", "Credibility", "Family", "Authority", "Connections", "Resources", "Medical Tech", "Charismatic Leadership" }};
    }

    class Program
    {

        static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client = new DiscordSocketClient();
        private CommandService _commands = new CommandService();
        
        private Info info = new Info();
        public async Task MainAsync()
        {

#pragma warning disable CS8601 // Possible null reference assignment.
            info = JsonConvert.DeserializeObject<Info>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Nero-source/json/safe/safe.json")));
#pragma warning restore CS8601 // Possible null reference assignment.
            if (info == null)
            {
                info = new Info();
            }

            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _client.Log += Log;
            _client.Ready += Client_Ready;
            _client.ModalSubmitted += ModalHandler;
            _client.ButtonExecuted += ButtonHandler;
            _client.SelectMenuExecuted += SelectMenuHandler;

            string Token = info.Token;

            if(Token == string.Empty)
            {
                Console.WriteLine("\nNo /safe/safe.json config file or token was unset.\n");
            }

            await _client.LoginAsync(TokenType.Bot, Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }


        public async Task Client_Ready()
        {

            var guild = _client.GetGuild(info.BasementGuildID);

            var commandBuilders = new CommandBuilders();

            var guildCommand = commandBuilders.Roll;

            try
            {
                await guild.CreateApplicationCommandAsync(guildCommand.Build());
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }


            _client.SlashCommandExecuted += SlashCommandHandler;

        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            Console.WriteLine($"Executed: {command.Data.Name}, ID: {command.Data.Id} \n");

            switch(command.CommandName)
            {
                case "debug":
                    var debug = new DebugCommands();
                    await debug.CommandHandler(command, _client.GetGuild(info.BasementGuildID), _client);
                    break;
                case "roll":
                    var comm = new GeneralCommands();
                    await comm.Roll(command);
                    break;
                case "character":
                    var character = new CharacterCommands();
                    await character.CommandHandler(command);
                    break;
            }
        }

        private async Task ModalHandler(SocketModal modal)
        {

            if (modal.Data.CustomId == "characterCreate_1") {
                var comm = new Nero.CharacterCommands();
                await comm.CreationModalHandler(modal);
            }
            else {
                var comm = new Nero.CharacterCommands();
                await comm.SubskillModalHandler(modal);
            }

        }

        private async Task ButtonHandler(SocketMessageComponent component)
        {
            // Syntax: type_action_pos_num_optional...
            string type = component.Data.CustomId.Split("_").First();
            string action = component.Data.CustomId.Split("_").ToArray()[1];
            switch(type)
            {
                case "stat":
                case "skill":
                    switch(action)
                    {
                        case "add":
                        case "move":
                        case "confirm":
                        case "new":
                        case "remove":
                            var comm = new Nero.CharacterCommands();
                            await comm.StatDistributor(component);
                            break;
                        
                    }
                break;
            }
        }

        private async Task SelectMenuHandler(SocketMessageComponent component)
        {
            switch(component.Data.CustomId)
            {
                case "charCreateRole":
                    var comm = new Nero.CharacterCommands();
                    await comm.StatDistributor(component);
                    break;
            }
        }

    }
}