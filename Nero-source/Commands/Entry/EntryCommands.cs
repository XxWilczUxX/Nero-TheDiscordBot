using System.ComponentModel;
using Discord;
using Discord.WebSocket;
using Nero.Commands.Session;

namespace Nero;

public class LogSubCommand
{

    public async Task LogHandler(SocketSlashCommand command, string subCommand)
    {
        
        switch(subCommand) {
            case "create":
                await LogCreate(command);
                return;
            case "read":    
                await LogRead(command);
                return;
            case "delete":    
                await LogDelete(command);
                return;
            default:
                var embeds = new Embeds();
                await command.RespondAsync(embed: embeds.Error("Not implemented yet.").Build());
                return;
        }

    }  

    private MessageComponent createComponents(List<Data.Log> logs, int page) {

        Console.WriteLine(logs.Count);

        var back = new ButtonBuilder()
            .WithLabel("<")
            .WithStyle(ButtonStyle.Primary)
            .WithCustomId($"log-back-{page}")
            .WithDisabled(logs.Count <= 25);

        var forward = new ButtonBuilder()
            .WithLabel(">")
            .WithStyle(ButtonStyle.Primary)
            .WithCustomId($"log-forward-{page}")
            .WithDisabled(logs.Count <= 25);

        var component = new ComponentBuilder()
            .WithButton(back)
            .WithButton(forward);

        return component.Build();
    }

    private async Task LogCreate(SocketSlashCommand command)
    {
        var userId = command.User.Id;

        var socketChannel = command.Channel as ITextChannel;

        if (socketChannel != null && socketChannel.GetChannelType() == ChannelType.PrivateThread)
        {
            var dataController = new Data.DataController();
            var embeds = new Embeds();

            var logMessage = command.Data.Options.First().Options.First().Options.First().Value.ToString();

            if(logMessage != null) {
                dataController.SaveLog(command.GuildId?? 0, socketChannel.Id, userId, logMessage);

                var logs = dataController.GetLogs(command.GuildId?? 0, socketChannel.Id);
                var embed = embeds.Log(logs, logs.Count / 25).Build();

                await command.RespondAsync(embed: embed, components: createComponents(logs, 0));
            } else {

                await command.RespondAsync(embed: embeds.Error("No message provided.").Build());
            }
        }
        else
        {
            var embeds = new Embeds();
            await command.RespondAsync(embed: embeds.Error("Not a thread.").Build());
        }
    }

    private async Task LogRead(SocketSlashCommand command)
    {

        var socketChannel = command.Channel as ITextChannel;

        if (socketChannel != null && socketChannel.GetChannelType() == ChannelType.PrivateThread)
        {
            var dataController = new Data.DataController();
            var embeds = new Embeds();

            var logs = dataController.GetLogs(command.GuildId?? 0, socketChannel.Id);

            var embed = embeds.Log(logs).Build();

            await command.RespondAsync(embed: embed, components: createComponents(logs, 0));
        }
        else
        {
            var embeds = new Embeds();
            await command.RespondAsync(embed: embeds.Error("Not a thread.").Build());
        }
    }

    private async Task LogDelete(SocketSlashCommand command) 
    {

        var socketChannel = command.Channel as ITextChannel;

        if (socketChannel != null && socketChannel.GetChannelType() == ChannelType.PrivateThread)
        {
            var dataController = new Data.DataController();
            var embeds = new Embeds();

            var input = command.Data.Options.First().Options.First().Options.First().Value.ToString();
            var logIndex = int.Parse(input?? "-1") - 1;

            if(logIndex >= 0) {
                dataController.DeleteLog(command.GuildId?? 0, socketChannel.Id, logIndex);

                await command.RespondAsync(embed: embeds.Log(dataController.GetLogs(command.GuildId?? 0, socketChannel.Id)).Build());
            } else {
                await command.RespondAsync(embed: embeds.Error("No index provided.").Build());
            }
        }
        else
        {
            var embeds = new Embeds();
            await command.RespondAsync(embed: embeds.Error("Not a thread.").Build());
        }

    }

}