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

    public class AdminCommands
    {

        public async Task commandHandler(SocketSlashCommand command, SocketGuild guild)
        {
            switch(command.Data.Options.First().Value)
            {
                case 0:
                    await deleteAllGuildCommands(command, guild);
                    break;
            }
        }

        public async Task deleteAllGuildCommands(SocketSlashCommand command, SocketGuild guild)
        {
            var commands = await guild.GetApplicationCommandsAsync();

            foreach(var delCom in commands)
            {
                if(delCom.Id != command.Id)
                {
                    Console.WriteLine($"Deleted {delCom.Name}: {delCom.Id}");
                    delCom.DeleteAsync();
                }
            }

            command.RespondAsync("Done.");

        }

    }

}