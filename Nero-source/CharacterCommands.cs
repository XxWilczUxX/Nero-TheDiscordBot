using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nero
{

    public class Character
    {
        public string name;
        public string description;
        public string role;
        public int[] stats = new int[10];
        public int[,] skills = new int[66,2];
        public int[] distrPoints = new int[] {42, 58};

        public Character(string name, string description)
        {
            this.name = name;
            this.description = description;
            for(int i = 0; i < 10; i++)
            {
                this.stats[i] = 2;
            }
        }

    }

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

            Character character = new Character(name, descr);

            Directory.CreateDirectory(Path.Join(Directory.GetCurrentDirectory(), "\\Nero-source\\temp\\characters"));
            File.WriteAllText( Path.Join(Directory.GetCurrentDirectory(), $"\\Nero-source\\temp\\characters\\{modal.User.Id}.json"), JsonConvert.SerializeObject(character, Formatting.Indented) );
            
            var role = new SelectMenuBuilder()
                .WithCustomId("charCreateRole")
                .WithPlaceholder("Choose character's role.")
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

            var componentBuilder = new ComponentBuilder()
                .WithSelectMenu(role)
            ;

            await modal.RespondAsync(components: componentBuilder.Build(), ephemeral: true);

        }

        public async Task StatDistribution(SocketMessageComponent component)
        {
            Character? character = JsonConvert.DeserializeObject<Character>( File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), $"\\Nero-source\\temp\\characters\\{component.User.Id}.json")) );
            if(character == null)
            {   
                var embeds = new Nero.Embeds();
                await component.RespondAsync(embed: embeds.Error("No character file in temp").Build());
            }
            else
            {
                var names = new Nero.Names();

                character.role = component.Data.Values.First();

                var embed = new EmbedBuilder()
                    .WithTitle("Stat Distribution")
                    .WithDescription("Distribute stat points of your character from 2 to 8")
                    .AddField($"Current Stat: {names.stats[0]}", $"lvl: {character.stats[0]}")
                    .AddField("Available points:", character.distrPoints[0])
                ;

                var buttonMin = new ButtonBuilder()
                    .WithCustomId("minus_0")
                    .WithLabel("-")
                    .WithStyle(ButtonStyle.Danger)
                ;
                var buttonPlus = new ButtonBuilder()
                    .WithCustomId("plus_0")
                    .WithLabel("+")
                    .WithStyle(ButtonStyle.Success)
                ;
                var buttonBack = new ButtonBuilder()
                    .WithCustomId("back_0")
                    .WithLabel("Previous")
                    .WithStyle(ButtonStyle.Primary)
                ;
                var buttonNext = new ButtonBuilder()
                    .WithCustomId("next_0")
                    .WithLabel("Next")
                    .WithStyle(ButtonStyle.Primary)
                ;

                var builder = new ComponentBuilder()
                    .WithButton(buttonMin)
                    .WithButton(buttonBack)
                    .WithButton(buttonNext)
                    .WithButton(buttonPlus)
                ;

                await component.UpdateAsync(x =>
                    {
                        x.Embed = embed.Build();
                        x.Components = builder.Build();
                    }
                );
            }
        }

    }

}