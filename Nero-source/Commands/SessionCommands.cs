using Discord;
using Discord.WebSocket;

namespace Nero;
public class SessionCommand
{
    public async Task CommandHandler(SocketSlashCommand command) {

        switch (command.Data.Options.First().Name){
            case "create":
                await Create(command);
                return;
            case "log":
                await Log(command);
                return;
            default:
                var embeds = new Embeds();

                await command.RespondAsync(embed: embeds.Error("Not implemented yet.").Build());
                return;
        }

    }

    private async Task Create(SocketSlashCommand command) {

        var socketChannel = command.Channel as ITextChannel;

        if (socketChannel != null) {
            var newSession = await socketChannel.CreateThreadAsync(
                name: command.Data.Options.First().Options.First().Value.ToString(),
                autoArchiveDuration: ThreadArchiveDuration.OneWeek,
                invitable: true,
                type: ThreadType.PrivateThread
            );

            string channelMention = $"<#{newSession.Id}>";

            await command.RespondAsync($"Session created: {channelMention}");

            await newSession.SendMessageAsync($"Session created by: {command.User.Mention}!");
        } 
        else {
            var embeds = new Embeds();
            await command.RespondAsync(embed: embeds.Error("Channel Error.").Build());
        }
    }

    private async Task Log(SocketSlashCommand command) {

        var socketChannel = command.Channel as IThreadChannel;

        if (socketChannel != null) {

            var logMessage =  command.Data.Options.First().Options.First().Value.ToString();
            var embeds = new Embeds();

            if(logMessage != "" && logMessage != null) {

                var dataController = new DataController();

                dataController.SaveLog(socketChannel.GuildId, socketChannel.Id, command.User.Username, logMessage);

                await command.RespondAsync(embed: embeds.Log(command.User , logMessage).Build());
            } else {

                await command.RespondAsync(embed: embeds.Error("No message to log.").Build());
            }

        } else {
            var embeds = new Embeds();
            await command.RespondAsync(embed: embeds.Error("Not a thread.").Build());
        }

    }
}