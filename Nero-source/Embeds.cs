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

        public EmbedBuilder Info(string title, string errorMessage)
        {
            var embed = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(errorMessage)
                .WithColor(Color.Green)
                .WithCurrentTimestamp();
            
            return embed;
        }

        public EmbedBuilder DebugExecuted()
        {
            var embed = new EmbedBuilder()
                .WithTitle("Done")
                .WithColor(Color.DarkBlue)
                .WithCurrentTimestamp();

            return embed;
        }

    }

}