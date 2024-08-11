using Discord;
using Discord.WebSocket;
using Nero.Commands.Session;

namespace Nero;

public class SessionCommand
{
    private readonly SessionManager _sessionManager;
    private const int maxSessionsPerUser = 10; // I'd rather have this in a file (somethng like profiles of settings for the bot in json(ofc))
    public SessionCommand()
        => _sessionManager = new SessionManager(maxSessionsPerUser);
    public async Task CommandHandler(SocketSlashCommand command)
    {
        switch (command.Data.Options.First().Name)
        {
            case "create":
                await Create(command);
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

        if (!_sessionManager.CanCreateSession(userId))
        {
            var embeds = new Embeds();
            await command.RespondAsync(embed: embeds.Error("You have reached the limit of active sessions.").Build());
            return;
        }

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

            _sessionManager.AddSession(userId, newSession.Id);

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