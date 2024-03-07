using Discord;
using Discord.WebSocket;

namespace Nero
{

    public class GeneralCommands
    {

        public async Task Roll(SocketSlashCommand command)
        {

            var rand = new Random();
            int max = 0 ;
            int.TryParse(command.Data.Options.First().Value.ToString(), out max);
            var embed = new EmbedBuilder()
                .WithTitle($"D{max}")
                .WithDescription($"{rand.Next(1, max+1)}")
                .WithColor(Color.Green);

            await command.RespondAsync(embed: embed.Build());

        }

    }

}