using Discord;
using Discord.WebSocket;

namespace Nero
{

    public class Embeds
    {

        public EmbedBuilder Error(string errorMessage)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Error")
                .WithDescription(errorMessage)
                .WithColor(Color.Red)
                .WithCurrentTimestamp();
            
            return embed;
        }

        public EmbedBuilder DebugExecuted(SocketSlashCommand command)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Done")
                .WithDescription($"Executed option nr. {command.Data.Options.First().Value}")
                .WithColor(Color.DarkBlue)
                .WithCurrentTimestamp();

            return embed;
        }

    }

}