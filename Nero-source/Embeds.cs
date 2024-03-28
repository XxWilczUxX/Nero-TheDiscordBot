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
                .WithColor(Color.DarkBlue)
                .WithCurrentTimestamp();

            return embed;
        }

        public EmbedBuilder NetworkArchitecture(NetworkArchitecture network) {
            var embed  = new EmbedBuilder()
                .WithTitle("Network Architecture")
                .WithDescription("Network Architecture")
                .WithColor(Color.Blue);
                
            for(int i = 0; i < network.Size; i++) {
                embed.AddField($"Floor {i+1}", "Floor");
            }

            return embed;
        }

    }

}