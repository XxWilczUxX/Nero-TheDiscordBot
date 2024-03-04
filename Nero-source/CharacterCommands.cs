using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord.Rest;

namespace Nero
{

    public class Character
    {
        public string name;
        public string description;
        public string role;
        public int[] stats {get; set;} = new int[10];
        public Skill[] skills {get; set;} = new Skill[66];
        public int[] distrPoints {get;} = new int[] {42, 58};
        
        public Character(string name, string description)
        {
            this.name = name;
            this.description = description;
            for(int i = 0; i < 10; i++)
            {
                this.stats[i] = 2;
            }


        }

        public void PointsAllocation(int stat, int num)
        {
            
            

        }

    }

    public class Skill
    {
        public int level {get; set;} = 0;
        private int stat {get;} = 0;
        public int? cost {get;} = 1;
        public string? name {get;}
        public List<Skill>? subSkills {get; set;}

        public Skill(int stat, int cost)
        {
            this.stat = stat;
            this.cost = cost;
        }

        public Skill(int stat, string name)
        {
            this.name = name;
            this.stat = stat;
        }

        public void AddLevel(int num)
        {
            if(this.level + num < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                this.level += num;
            }
        }

        public void AddSubskill(Skill subskill)
        {
            if(this.subSkills == null)
            {
                this.subSkills = new List<Skill>();
            }
            else
            {
                this.subSkills.Add(subskill);
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

        async Task messageStatDistributor(Character character, SocketMessageComponent component, int num)
        {

            var names = new Nero.Names();

            var embed = new EmbedBuilder()
                .WithTitle("Stat Distribution")
                .WithDescription("Distribute stat points of your character from 2 to 8")
                .AddField($"Current Stat: {names.stats[num]}", $"lvl: {character.stats[num]}")
                .AddField("Available points:", character.distrPoints[0])
            ;
            var buttonMin = new ButtonBuilder()
                .WithCustomId($"stat_minus_{num}")
                .WithLabel("-")
                .WithStyle(ButtonStyle.Danger)
            ;
            var buttonPlus = new ButtonBuilder()
                .WithCustomId($"stat_plus_{num}")
                .WithLabel("+")
                .WithStyle(ButtonStyle.Success)
            ;
            var buttonBack = new ButtonBuilder()
                .WithCustomId($"stat_back_{num}")
                .WithLabel("Previous")
                .WithStyle(ButtonStyle.Primary)
            ;
            var buttonNext = new ButtonBuilder()
                .WithCustomId($"stat_next_{num}")
                .WithLabel("Next")
                .WithStyle(ButtonStyle.Primary)
            ;

            var buttonConfirm = new ButtonBuilder()
                .WithCustomId("stat_confirm")
                .WithLabel("Confirm")
                .WithStyle(ButtonStyle.Secondary)
            ;

            var builder = new ComponentBuilder()
                .WithButton(buttonMin)
                .WithButton(buttonBack)
                .WithButton(buttonNext)
                .WithButton(buttonPlus)
                .WithButton(buttonConfirm, 1)
            ;

            File.WriteAllText( Path.Join(Directory.GetCurrentDirectory(), $"\\Nero-source\\temp\\characters\\{component.User.Id}.json"), JsonConvert.SerializeObject(character, Formatting.Indented) );

            await component.UpdateAsync(x=>
                {
                    x.Embed = embed.Build();
                    x.Components = builder.Build();
                }
            );

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

        public async Task StatDistributor(SocketMessageComponent component)
        {
            Character? character = JsonConvert.DeserializeObject<Character>( File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), $"\\Nero-source\\temp\\characters\\{component.User.Id}.json")) );
            
            if(character == null)
            {   
                var embeds = new Nero.Embeds();
                await component.RespondAsync(embed: embeds.Error("No character file in temp").Build());
            }
            else
            {
                if(component.Data.CustomId == "charCreateRole")
                {
                    character.role = component.Data.Values.First();

                    await messageStatDistributor(character, component, 0);
                
                }
                else
                {
                    
                }
            }
        }

    }

}