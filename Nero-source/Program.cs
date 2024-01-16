using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord.Interactions;

namespace Nero
{

    public class Info
    {
        public string token { get; set; }
        public ulong basementGuildID { get; set; }
    }

    public class Names
    {
        public readonly string[] abilityGroups = { "Long Range Weapons", "Body", "Education", "Control", "Observation", "Technology", "Social", "Meele", "Performances" };
        public readonly string[] abilities = { "Heavy Weapons (x2)", "Long Weapons", "Short Weapons", "Archery", "Continous Fire", "Athletics", "Rubber Man", "Torture/Narcotic Tolerancy", "Stealth", "Dance", "Endurance", "Bureaucracy", "Deduction", "Language", "Composing", "Criminology", "Cryptography", "Accounting", "Science", "Animal Care", "Library Searching", "Making Deals", "Art of Survival", "Tactics", "Local Knowlege", "Education", "Riding", "Piloting (x2)", "Car Driving", "Sailing", "Lip Reading", "Concentration", "Perception", "Tracking", "Hiding/Finding an Item", "Cyber Engineering", "Electronics and Security", "Falsification", "Photography", "Pickpocketing", "Art", "Explosives", "Weapon Repair", "Land Vehicle Repair", "Water Vehicle Repair", "Air Vehicle Repair", "Lockpicking", "First Aid", "Basic Repairing", "Paramedics", "Attractiveness", "Trading", "Conversation", "Fashion", "Emotional Inteligence", "Persuasion", "Bribery", "Interrogation", "Semi-Literate Knowlege", "Fighting", "Meele Weapons", "Martial Arts", "Dodgeing", "Acting", "Playing an Instrument" };
        public readonly string[] roles = { "Solo", "Netrunner", "Techie", "Media", "Cop", "Nomad", "Fixer", "Corporate", "Medtech", "Rockerboy / Rockergirl" };
        public readonly string[] roleSuperAbilityGroups = { "Combat Sense", "Interface", "Jurry Rig", "Credibility", "Family", "Authority", "Connections", "Resources", "Medical Tech", "Charismatic Leadership" };
    }

    class Program
    {

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        Info info = JsonConvert.DeserializeObject<Info>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Nero-source/json/safe/safe.json" )));
        Names names = new Names();
        public async Task MainAsync()
        {   
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _client.Log += Log;
            _client.Ready += Client_Ready;

            string token = info.token;


            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            //_client.MessageReceived += MessageReceived; 

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }


        public async Task Client_Ready()
        {

            var guild = _client.GetGuild(info.basementGuildID);

            /*

            var guildCommand = new SlashCommandBuilder()
                .WithName("help")

            */

            // Guild commands delete all

            /*

            var commands = await guild.GetApplicationCommandsAsync();

            foreach(var command in commands)
            {
                command.DeleteAsync();
            }
        
            */

            // character command tree

            /*
            
            var guildCommand = new SlashCommandBuilder()
                .WithName("character")
                .WithDescription("character managament command tree")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("list")
                    .WithDescription("Lists out all characters")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("create")
                    .WithDescription("Creates a character")
                    .AddOption("name", ApplicationCommandOptionType.String, "Name and surname of your character", isRequired: true)
                    .AddOption("nickname", ApplicationCommandOptionType.String, "The nickname of your character", isRequired: true)
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("role")
                        .WithDescription("Choose character's role")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.Integer)
                    )
                    .AddOption("description", ApplicationCommandOptionType.String, "A short description of your character", isRequired: true)
                    .AddOption("main-stats", ApplicationCommandOptionType.String, "Stats for: INT, REF, AGI, TECH, CHA, SW, LUC, MOV, BC, EMP. ex: \"8,6,7,8,5,6,5,6,3,8\" (sum 62)", isRequired: true)
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("edit")
                    .WithDescription("Edits a certain thing about a character")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    
                );

            for(int i = 0; i < names.roles.Length; i++)
            {
                guildCommand.Options[1].Options[2].AddChoice(names.roles[i], i);
            }

            

            try
            {
                await _client.Rest.CreateGuildCommand(guildCommand.Build(), info.basementGuildID);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            */

            // - Global Slash Command Template
            /* 

            var globalCommand = new SlashCommandBuilder();
                globalCommand.WithName("name");
                globalCommand.WithDescription("desc");

            try
            {

                await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            */

            _client.SlashCommandExecuted += SlashCommandHandler;

        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            Console.WriteLine($"Executed: {command.Data.Name}_{command.Data.Id}");

            

            switch(command.Data.Name)
            {
                case "roll":
                    await RollHandler(command);
                    break;
                case "character":
                    var cl = new CharacterEditor();
                    await cl.CharacterHandler(command, command.Data.Options.First().Name);
                    break;

                
            }

        }

        private async Task RollHandler(SocketSlashCommand command)
        {
            var commandOptions = command.Data.Options.Count;
            var rng = new Random();
            int[] outputs = new int[1];
            
            try
            {
                outputs = new int[Convert.ToInt32(command.Data.Options.ElementAt(1).Value)];
            }
            catch
            {
                outputs = new int[1];
            }
            int num = 0;   

            num = rng.Next(20,50);     

            // i: number of repeating all rolling, j: number of dices to roll

            if(commandOptions == 1)
            {
                for(int i = 0; i < num; i++)
                {
                    outputs[0] = rng.Next(1, Convert.ToInt32(command.Data.Options.ElementAt(0).Value) + 1 );
                }
            } 
            else
            {
                for(int i = 0; i < num; i++)
                {
                    for(int j = 0; j < outputs.Length; j++)
                    {
                        outputs[j] = rng.Next(1, Convert.ToInt32(command.Data.Options.ElementAt(0).Value) + 1 );
                    }
                }
            }

            
            string title = $"**D{command.Data.Options.ElementAt(0).Value}**";
            var output = String.Join($"\n", outputs);
            var sum = outputs.Sum();

            var embedBuilder = new EmbedBuilder()
                .WithColor(Color.Gold)
                .WithTitle(title)
                .WithDescription(output);

            await command.RespondAsync(embed: embedBuilder.Build());

        }

        

    }
}