using Discord;
using Discord.WebSocket;
using Nero.Data;

namespace Nero;

public class SessionCommand
{
    public async Task CommandHandler(SocketSlashCommand command)
    {
        switch (command.Data.Options.First().Name)
        {
            case "create":
                await Create(command);
                return;
            case "log":
                var LogCommands = new LogSubCommand();
                await LogCommands.LogHandler(command, command.Data.Options.First().Options.First().Name);
                return;
            default:
                var embeds = new Embeds();
                await command.RespondAsync(embed: embeds.Error("Not implemented yet.").Build());
                return;
        }
    }

    private async Task Create(SocketSlashCommand command)
    {
        var userId = command.User.Id;

        var socketChannel = command.Channel as ITextChannel;

        var embeds = new Embeds();

        if (socketChannel != null)
        {

            User user = new User(userId);
            user.Load();

            if(!user.CanAddSession()) {
                await command.RespondAsync(embed: embeds.Error("You have too many sessions open.").Build());
                return;
            }

            var newSession = await socketChannel.CreateThreadAsync(
                name: command.Data.Options.First().Options.First().Value.ToString(),
                autoArchiveDuration: ThreadArchiveDuration.OneWeek,
                invitable: true,
                type: ThreadType.PrivateThread
            );

            DataController dataController = new DataController();

            dataController.SaveSession(command.GuildId?? 0, newSession.Id, userId);

            string channelMention = $"<#{newSession.Id}>";

            await command.RespondAsync($"Session created: {channelMention}");

            await newSession.SendMessageAsync($"Session created by: {command.User.Mention}!");
        }
        else
        {
            await command.RespondAsync(embed: embeds.Error("Channel Error.").Build());
        }
    }

    
}