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

            //embed.AddField($"Floor {i+1}", "Floor"); // embed.AddField("\u200B", "\u200B");

            Console.WriteLine("\u200B");
            List<Floor>[] floors = network.PreorderList(new List<Floor>[network.Size], network.RootFloor);

            for(int i = 0; i < floors.Length; i++) {
                for(int j = 0; j < floors[i].Count; j++) {
                    Console.WriteLine(floors[i].Count);
                    embed.AddField($"Floor {i+1}, branch {j}", "Floor", inline: true);
                }
                embed.AddField("\u200B", "\u200B", inline: false);
            }


            return embed;
        }

    }

}