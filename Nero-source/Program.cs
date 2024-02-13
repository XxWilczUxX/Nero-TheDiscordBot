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
        public string token { get; set; }
        public ulong basementGuildID { get; set; }
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

            var guildCommand = new SlashCommandBuilder()
                .WithName("admin")
                .WithDescription("admin commands that are only for impactfull management of the bot")
                .WithDefaultMemberPermissions(GuildPermission.Administrator)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("command")
                    .WithDescription("choose a command")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.Integer)
                    .AddChoice("Delete All Guild Commands", 0)
                    .AddChoice("Delete All Global Commands", 1)
                    .AddChoice("Delete All Characters", 2)  
                );

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
                case "admin":
                    var admin = new AdminCommands();
                    await admin.commandHandler(command, _client.GetGuild(info.basementGuildID));
                    break;
            }
        }

    }
}