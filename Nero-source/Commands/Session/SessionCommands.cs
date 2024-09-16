using Discord;
using Discord.WebSocket;
using Nero.Data;
using Nero.Data.SessionData;
using Nero.Data.UserData;

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
                await command.RespondAsync(embed: Embeds.Error("Not implemented yet.").Build());
                return;
        }
    }

    private async Task Create(SocketSlashCommand command)
    {
        var userId = command.User.Id;

        var socketChannel = command.Channel as ITextChannel;

        if (socketChannel != null)
        {

            User user = new User(userId);
            user.Load();

            var sessionChannel = await socketChannel.CreateThreadAsync(
                name: command.Data.Options.First().Options.First().Value.ToString(),
                autoArchiveDuration: ThreadArchiveDuration.OneWeek,
                invitable: true,
                type: ThreadType.PrivateThread
            );

            try
            {
                Session session = new Session(sessionChannel.GuildId, sessionChannel.Id);
                session.Save();

                user.AddSession(session.ChannelID);
                user.Save();
            }
            catch (Exception ex)
            {
                await command.RespondAsync(embed: Embeds.Error(ex.Message).Build());
                return;
            }

            string channelMention = $"<#{sessionChannel.Id}>";

            await command.RespondAsync($"Session created: {channelMention}");

            await sessionChannel.SendMessageAsync($"Session created by: {command.User.Mention}!");
        }
        else
        {
            await command.RespondAsync(embed: Embeds.Error("Channel Error.").Build());
        }
    }

    
}