using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nero
{

    public class CharacterCommands
    {

        public async Task CommandHandler(SocketSlashCommand command)
        {
            switch(command.Data.Options.First().Name)
            {
                case "create":
                    await Create(command);
                    break;
            }
        }

        async Task Create(SocketSlashCommand command)
        {
            var name = new TextInputBuilder()
                .WithCustomId("name")
                .WithLabel("Name")
                .WithRequired(true)
                .WithMinLength(6)
                .WithMaxLength(40)
                .WithStyle(TextInputStyle.Short)
                .WithPlaceholder("Name of your character ex. Bob \"Amogus\" Smith")
            ;
            var descr = new TextInputBuilder()
                .WithCustomId("descr")
                .WithLabel("Description")
                .WithRequired(true)
                .WithMaxLength(4000)
                .WithStyle(TextInputStyle.Paragraph)
                .WithPlaceholder("A description of your character. Looks, background, lore, etc.")
            ;

            /*
            var role = new SelectMenuBuilder()
                .WithCustomId("charCreateRole")
                .WithPlaceholder("Choose a character's role.")
                .WithMinValues(1)
                .WithMaxValues(1)
                .AddOption("Solo", "solo", "Lethal combat specialists skilled in weaponry, tactics, and survival.")
                .AddOption("Netrunner", "netrunner", "Quick-hacking experts who manipulate cyberspace and infiltrate computer systems.")
                .AddOption("Techie", "techie", "Inventive mechanics and engineers who excel at crafting and repairing technology.")
                .AddOption("Media", "media", "Hard-hitting journalists and media personalities who uncover secrets and shape public opinion.")
                .AddOption("Cop", "cop", "Duty-bound enforcers of the law, maintaining order in a chaotic world.")
                .AddOption("Nomad", "nomad", "Range-riding wanderers who thrive in the wastelands between cities, skilled in survival and more.")
                .AddOption("Fixer", "fixer", "Clever negotiators and dealmakers who connect people and facilitate transactions.")
                .AddOption("Corporate", "corporat", "Scheming corporate executives who wield power and influence behind the scenes.")
                .AddOption("Medtech", "medtech", "Lifesaving healers and medics who can also take lives when necessary.")
                .AddOption("Rockerboy / Rockergirl", "rocker", "Charismatic rebels who use music, art, and charisma to inspire and lead others.")
            ;
            */

            var modal = new ModalBuilder()
                .WithCustomId("characterCreate_1")
                .WithTitle($"Character Creation for {command.User.GlobalName}.")
                .AddTextInput(name)
                .AddTextInput(descr)
            ;

            await command.RespondWithModalAsync(modal.Build());
        }

        public async Task CreationModalHandler(SocketModal modal){
            List<SocketMessageComponentData> components = modal.Data.Components.ToList();
            var name = components
                .First(x => x.CustomId == "name").Value;
            var descr = components
                .First(x => x.CustomId == "descr").Value;

            

        }

    }

}