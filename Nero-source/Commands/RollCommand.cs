using Discord;
using Discord.WebSocket;

namespace Nero
{

    public class RollCommand
    {

        private int DicesNumber(SocketSlashCommand command) {
            int number = 1;

            var options = command.Data.Options;

            if(options.First() != options.Last()){
                int.TryParse(options.Last().Value.ToString(), out number);
            }
            
            if(number > 25){number = 25;}

            return number;
        }

        public async Task Roll(SocketSlashCommand command) {

            var rand = new Random();
            int max = 0;
            int.TryParse(command.Data.Options.First().Value.ToString(), out max);

            int[] rolls = new int[DicesNumber(command)];

            for(int i = 0; i < rolls.Length; i++){
                rolls[i] = rand.Next(1, max+1);

                while(max == 10 && rolls[i]%max == 0) {
                    rolls[i] += rand.Next(1, max+1);
                }
            }

            var embeds = new Embeds();

            var embed = embeds.Rolls(max, rolls);

            

            await command.RespondAsync(embed: embed.Build());

        }

    }

}