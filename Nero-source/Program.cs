using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json;

namespace Nero
{

    public class Info
    {
        public string? token { get; set; }
        public string? guildId { get; set; }
    }

    class Program
    {

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient? _client;
        private CommandService? _commands;
        Info info = JsonConvert.DeserializeObject<Info>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "json/safe/info.json")));

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

            /* - Guild Slash Command Template

            var guild = _client.GetGuild(info.guildID);

            var guildCommand = new SlashCommandBuilder()
                .WithName("roll")
                .WithDescription("Rolls a dice or dices which number of sides is equal to the first parameter")
                .AddOption("sides", ApplicationCommandOptionType.Integer, "A number of sides that the dice(s) will have", isRequired: true)
                .AddOption("number-of-dices", ApplicationCommandOptionType.Integer, "A number of dices to roll", isRequired: false);

            try
            {
                await _client.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            */


            /* - Global Slash Command Template

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