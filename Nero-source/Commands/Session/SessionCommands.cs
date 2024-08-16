using Discord;
using Discord.WebSocket;
using Nero.Commands.Session;

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

        if (socketChannel != null)
        {
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
        else
        {
            var embeds = new Embeds();
            await command.RespondAsync(embed: embeds.Error("Channel Error.").Build());
        }
    }
}