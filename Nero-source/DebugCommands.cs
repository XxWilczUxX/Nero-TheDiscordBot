using Discord;
using Discord.WebSocket;

namespace Nero
{

    public class DebugCommands
    {

        public async Task CommandHandler(SocketSlashCommand command, SocketGuild guild, DiscordSocketClient _client)
        {
            switch(command.Data.Options.First().Name)
            {
                case "list":
                    switch(command.Data.Options.First().Options.First().Value.ToString())
                    {
                        case "0":
                            await ListAllGuildCommands(command, guild);
                            break;
                        case "1":
                            await ListAllGlobalCommands(command, _client);
                            break;
                    }
                    break;
                case "delete":
                    switch(command.Data.Options.First().Options.First().Value.ToString())
                    {
                        case "0":
                            await DeleteAllGuildCommands(command, guild);
                            break;
                        case "1":
                            await DeleteAllGlobalCommands(command, _client);
                            break;
                    }
                    break;
                //
            }
        }

        // List commands

        public static async Task ListAllGuildCommands(SocketSlashCommand command, SocketGuild guild)
        {

            var commands = await guild.GetApplicationCommandsAsync();
            var embed = new EmbedBuilder()
                .WithTitle("List of Guild Commands")
                .WithColor(Color.DarkBlue)
                .WithCurrentTimestamp();
            foreach(var com in commands)
            {
                embed.AddField(com.Name, $"ID: {com.Id}");
            }
            
            await command.RespondAsync(embed: embed.Build());
        }

        public static async Task ListAllGlobalCommands(SocketSlashCommand command, DiscordSocketClient _client)
        {

            var commands = await _client.GetGlobalApplicationCommandsAsync();
            var embed = new EmbedBuilder()
                .WithTitle("List of Global Commands")
                .WithColor(Color.DarkBlue)
                .WithCurrentTimestamp();
            foreach(var com in commands)
            {
                embed.AddField(com.Name, $"ID: {com.Id}");
            }
            
            await command.RespondAsync(embed: embed.Build());
        }

        // Delete commands

        public static async Task DeleteAllGuildCommands(SocketSlashCommand command, SocketGuild guild)
        {
            var commands = await guild.GetApplicationCommandsAsync();

            foreach(var delCom in commands)
            {
                if(delCom.Id != command.Data.Id)
                {
                    Console.WriteLine($"Deleted {delCom.Name}: {delCom.Id}");
                    await delCom.DeleteAsync();
                }
            }
            Embeds embeds = new Embeds();
            EmbedBuilder embed = embeds.DebugExecuted();
            await command.RespondAsync(embed: embed.Build());

        }

        public static async Task DeleteAllGlobalCommands(SocketSlashCommand command, DiscordSocketClient _client)
        {
            var commands = await _client.GetGlobalApplicationCommandsAsync();
            foreach(var delCom in commands)
            {
                if(delCom.Id != command.Id)
                {
                    Console.WriteLine($"Deleted {delCom.Name}: {delCom.Id}");
                    await delCom.DeleteAsync();
                }
            }
            Embeds embeds = new Embeds();
            var embed = embeds.DebugExecuted();
            await command.RespondAsync(embed: embed.Build());

        }

    }

}