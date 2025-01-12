using Discord;
using Discord.WebSocket;
using Nero.Data.SessionData;

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
                await command.RespondAsync(embed: Embeds.Error("Not implemented yet.").Build());
                return;
        }

    }  

    private MessageComponent createComponents(List<Data.SessionData.Log> logs, int page) {

        Console.WriteLine(logs.Count);

        var back = new ButtonBuilder()
            .WithLabel("<")
            .WithStyle(ButtonStyle.Primary)
            .WithCustomId($"log-button.back-{page}")
            .WithDisabled(logs.Count <= 25);

        var forward = new ButtonBuilder()
            .WithLabel(">")
            .WithStyle(ButtonStyle.Primary)
            .WithCustomId($"log-button.forward-{page}")
            .WithDisabled(logs.Count <= 25);

        var component = new ComponentBuilder()
            .WithButton(back)
            .WithButton(forward);

        return component.Build();
    }

    private bool IsThread(ITextChannel channel) {
        return channel.GetChannelType() == ChannelType.PrivateThread || channel.GetChannelType() == ChannelType.PublicThread;
    }

    private async Task LogCreate(SocketSlashCommand command)
    {

        var guildId = command.GuildId ?? 0;
        var socketChannel = command.Channel as ITextChannel;

        if(socketChannel == null || guildId == 0) {
            throw new Exception("Channel is null.");
        } else if(!IsThread(socketChannel)) {
            throw new Exception("Not a thread.");
        }

        
        var logMessage = command.Data.Options.First().Options.First().Options.First().Value.ToString();

        if (string.IsNullOrEmpty(logMessage)) {
            throw new Exception("No message provided.");
        }

        Session session = new Session(guildId, socketChannel.Id);
        session.Load();

        var logs = session.Logs;

        logs.Add(new Log(command.User.Id, logMessage));
        session.Logs = logs;

        session.Save();

        var embed = Embeds.Log(logs, logs.Count / 25).Build();

        await command.RespondAsync(embed: embed, components: createComponents(logs, 0));
    }

    private async Task LogRead(SocketSlashCommand command)
    {

        var guildId = command.GuildId ?? 0;
        var socketChannel = command.Channel as ITextChannel;

        if(socketChannel == null || guildId == 0) {
            throw new Exception("Channel is null.");
        } else if(!IsThread(socketChannel)) {
            throw new Exception("Not a thread.");
        }

        Session session = new Session(guildId, socketChannel.Id);
        session.Load();

        var logs = session.Logs;
        var embed = Embeds.Log(logs).Build();

        await command.RespondAsync(embed: embed, components: createComponents(logs, 0));

    }

    private async Task LogDelete(SocketSlashCommand command) 
    {

        var guildId = command.GuildId ?? 0;
        var socketChannel = command.Channel as ITextChannel;

        if(socketChannel == null || guildId == 0) {
            throw new Exception("Channel is null.");
        } else if(!IsThread(socketChannel)) {
            throw new Exception("Not a thread.");
        }

        var dataController = new Data.DataController();

        var input = command.Data.Options.First().Options.First().Options.First().Value.ToString();
        var logIndex = int.Parse(input?? "-1") - 1;

        if(logIndex < 0) {
            throw new Exception("Invalid log index.");   
        }

        Session session = new Session(guildId, socketChannel.Id);
        session.Load();

        var logs = session.Logs;
        logs.RemoveAt(logIndex);

        session.Save();

        await command.RespondAsync(embed: Embeds.Log(logs).Build());


    }

    // Button handling

    public async Task LogHandler(SocketMessageComponent component, string subCommand, int page) // Button Handler
    {
        
        switch(subCommand) {
            case "button.back":
                if(page > 0) {
                    await LogRead(component, page - 1);
                } else {
                    await LogRead(component, page);
                }
                return;

            case "button.forward":
                await LogRead(component, page + 1);
                return;

            default:
                await component.RespondAsync(embed: Embeds.Error("Not implemented yet.").Build(), ephemeral: true);
                return;

        }

    } 

    private async Task LogRead(SocketMessageComponent component, int page)
    {

        var guildId = component.GuildId ?? 0;   
        var socketChannel = component.Channel as ITextChannel;
        
        if(socketChannel == null || guildId == 0) {
            throw new Exception("Channel is null.");
        } else if(!IsThread(socketChannel)) {
            throw new Exception("Not a thread.");
        }

        Session session = new Session(component.GuildId?? 0, socketChannel.Id);
        session.Load();

        var logs = session.Logs;

        if(logs.Count / 25 + 1 <= page) {
            page = logs.Count / 25;
        }

        var embed = Embeds.Log(logs, page).Build();

        await component.UpdateAsync(msg => {
            msg.Embeds = new[] { embed };
            msg.Components = new Optional<MessageComponent>(createComponents(logs, page));
        });


        await component.RespondAsync(embed: Embeds.Error("Not a thread.").Build());

    }


}