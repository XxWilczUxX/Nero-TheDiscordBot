using Discord;
using Discord.WebSocket;

namespace Nero
{
    public class CharacterCommand
    {
        public async Task CommandHandler(SocketSlashCommand command)
        {
            Console.WriteLine(command.Data.Options.First().Name);
            switch (command.Data.Options.First().Name)
            {
                case "create":
                    await CreateCharacter(command);
                    break;
                case "delete":
                    await DeleteCharacter(command);
                    break;
                case "list":
                    await ListCharacters(command);
                    break;
                default:

                    await command.RespondAsync(embed: Embeds.Error("Not implemented yet.").Build());
                    break;
            }
        }

        public async Task CreateCharacter(SocketSlashCommand command)
        {
            var options = command.Data.Options.First().Options;

            var name = options.First().Value.ToString();
            var role = options.ElementAt(1).Value.ToString();

            await command.RespondAsync("Not implemented yet.");
        }

        public async Task DeleteCharacter(SocketSlashCommand command)
        {
            await command.RespondAsync("Not implemented yet.");
        }

        public async Task ListCharacters(SocketSlashCommand command)
        {
            await command.RespondAsync("Not implemented yet.");
        }
    }
}