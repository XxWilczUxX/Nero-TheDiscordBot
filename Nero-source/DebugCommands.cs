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

    public class DebugCommands
    {

        public async Task CommandHandler(SocketSlashCommand command, SocketGuild guild, DiscordSocketClient _client)
        {
            switch(command.Data.Options.First().Value)
            {
                case 0:
                    await DeleteAllGuildCommands(command, guild);
                    break;
                case 1:
                    await DeleteAllGlobalCommands(command, _client);
                    break;
                
            }
        }

        public static async Task DeleteAllGuildCommands(SocketSlashCommand command, SocketGuild guild)
        {
            var commands = await guild.GetApplicationCommandsAsync();

            foreach(var delCom in commands)
            {
                if(delCom.Id != command.Id)
                {
                    Console.WriteLine($"Deleted {delCom.Name}: {delCom.Id}");
                    await delCom.DeleteAsync();
                }
            }

            await command.RespondAsync("Done.");

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

            await command.RespondAsync("Done.");

        }

    }

}