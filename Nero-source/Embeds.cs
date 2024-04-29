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

        public EmbedBuilder Navigation(INavigation navigation) {

            var output = navigation.GetContents().Split('-');
            string[][] contents = new string[output.Length][];

            for (int i = 0; i < output.Length; i++) {
                contents[i] = output[i].Split(':');
            }

            var embed = new EmbedBuilder()
                .WithTitle($"{contents[0][0]}: {contents[0][1]}")
                .WithDescription($"{contents[1][0]}: {contents[1][1]}")
                .WithColor(Color.DarkBlue)
                .WithCurrentTimestamp();

            return embed;
        }

    }

}