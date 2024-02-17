using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord.Interactions;
using Discord.Net;

namespace Nero
{

    public class Info
    {
        public string Token { get; set; }
        public ulong BasementGuildID { get; set; }
    }

    public class Names
    {
        public readonly string[] stats = { "Inteligence", "Reflex", "Agility", "Technology", "Charisma", "Will", "Luck", "Movement", "Body", "Empathy" };
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
        public async Task MainAsync()
        {   

            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _client.Log += Log;
            _client.Ready += Client_Ready;

            string Token = info.Token;

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

            

            var guildCommand = new SlashCommandBuilder()
                .WithName("debug")
                .WithDescription("debug commands that are only for impactfull management of the bot")
                .WithDefaultMemberPermissions(GuildPermission.Administrator)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("list")
                    .WithDescription("Gives a list of all commands")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("type")
                        .WithDescription("Choose a type")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.Integer)
                        .AddChoice("guild", 0)
                        .AddChoice("global", 1)
                    )
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("delete")
                    .WithDescription("Deletes all commands of given type")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("type")
                        .WithDescription("Choose a type")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.Integer)
                        .AddChoice("guild", 0)
                        .AddChoice("global", 1)
                    )
                )
            ;

           

            

            // var guildCommand = new SlashCommandBuilder()
            //     .WithName("character")
            //     .WithDescription("All character command tree")
            //     .AddOption(new SlashCommandOptionBuilder()
            //             .WithName("create")
            //             .WithDescription("Starts a character creation process")
            //             .WithType(ApplicationCommandOptionType.SubCommand)
            //     )
            //     .AddOption(new SlashCommandOptionBuilder()
            //         .WithName("edit")
            //         .WithDescription("Choose a subcommand")
            //         .WithType(ApplicationCommandOptionType.SubCommandGroup)
            //         .AddOption(new SlashCommandOptionBuilder()
            //             .WithName("edit")
            //             .WithDescription("Allows character edition")
            //             .WithType(ApplicationCommandOptionType.SubCommand)
            //         )
            //     );

            try
            {
                await guild.CreateApplicationCommandAsync(guildCommand.Build());
            }
            catch (ApplicationCommandException ex)
            {
                var json = JsonConvert.SerializeObject(ex.Errors, Formatting.Indented);

                Console.WriteLine(json);
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
            }
        }

    }
}