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
        public string Name;
        public string Description;
        public string Role {get; set;} = string.Empty;
        public int[] Stats {get; set;} = new int[10];
        public Skill[] Skills {get; set;} = new Skill[65];
        public int[] DistrPoints {get; set;} = new int[] {42, 58};
        
        public Character(string name, string description)
        {
            Names names = new Names();
            this.Name = name;
            this.Description = description;
            for(int i = 0; i < Stats.Length; i++)
            {
                this.Stats[i] = 2;
            }
            
            int stat;
            int cost;
            int level;
            bool hasLevel;
            Skill? subskill = null;

            for(int i = 0; i < Skills.Length; i++)
            {
                cost = 1;
                hasLevel = true;
                level = 0;
                subskill = null;
                if(i == 7 || i == 9 || i == 10 || i == 31) // SW
                {
                    stat = 5;
                }
                else if(i == 52 || i == 54) // EMP
                {
                    stat = 4;
                }
                else if(i < 5 || (25 < i && i < 30)) // REF
                {
                    stat = 1;
                }
                else if((4 < i && i < 9) || (58 < i && i < 63)) // ZW
                {
                    stat = 2;
                }
                else if(i > 10 && i < 35) // INT
                {
                    stat = 0;
                }
                else if((34 < i && i < 50) || i == 65) // TECH
                {
                    stat = 3;
                }
                else // CHA
                {
                    stat = 9;
                }
                if(i == 0 || i == 4 || i == 27 || i == 36 || i == 41 || i == 49 || i == 61) // Cost
                {
                    cost = 2;
                }

                if(i == 13 || i == 19 || i == 24 || i == 64) // Subskill skills
                {
                    hasLevel = false;
                }

                if(i == 5 || i == 8 || i == 13 || i == 8 || i == 19 || i == 24 || i == 25 || i == 31 || i == 32 || i == 47 || i == 52 || i == 54 || i == 55 || i == 59 || i == 62) // Primary level allocation or creation of primary subskill
                {
                    if(!hasLevel)
                    {
                        switch(i)
                        {
                            case 13: // Language
                                subskill = new Skill(names.subskills[0], 2);
                                break;
                            case 24: // Local Knowlege
                                subskill = new Skill(names.subskills[1], 2);
                                break;
                        }
                    }
                    else
                    {
                        level = 2;
                    }
                }

                this.Skills[i] = new Skill(names.skills[i], stat, cost, hasLevel, level);
                if(subskill != null)
                {
                    this.Skills[i].AddSubskill(subskill);
                }

            }

        }

        public void AddLevelStat(int stat, int num = 1)
        {
            if(this.DistrPoints[0] == 0 || (num < 1 && this.Stats[stat] == 2))
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                this.Stats[stat] += num;
                this.DistrPoints[0] -= num;
            }
        }

    }

    public class Skill
    {
        public string Name {get;}
        public int? Level {get; set;}
        public int? Stat {get;}
        public int? Cost {get;}
        public List<Skill>? SubSkills {get; set;}

        public Skill(string name, int stat, int cost, bool hasLevel, int level = 0) // Primary Skill
        {
            this.Name = name;
            this.Stat = stat;
            this.Cost = cost;
            if(hasLevel)
            {
                this.Level = level;
            }
        }

        [JsonConstructor]
        public Skill(string Name, int? Level, int? Stat, int? Cost, List<Skill>? SubSkills)
        {
            this.Name = Name;
            this.Level = Level;
            this.Stat = Stat;
            this.Cost = Cost;
            this.SubSkills = SubSkills;
        }

        public Skill(string name, int level) // Subskill
        {
            this.Name = name;
            this.Level = level;
        }
        

        public void AddSubskill(Skill subskill)
        {
            if(this.SubSkills == null)
            {
                this.SubSkills = new List<Skill>();
            }
            this.SubSkills.Add(subskill);
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
                .AddField($"Current Stat: {names.stats[num]}", $"lvl: {character.Stats[num]}")
                .AddField("Available points:", character.DistrPoints[0])
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
                throw new FileNotFoundException();
            }
            else
            {
                if(component.Data.CustomId == "charCreateRole")
                {
                    character.Role = component.Data.Values.First();

                    await messageStatDistributor(character, component, 0);
                
                }
                else
                {
                    string[] id = component.Data.CustomId.Split("_").ToArray();
                    string action = id[1];
                    int num;
                    int.TryParse(id[2], out num);


                    switch(action)
                    {
                        case "minus":
                            character.AddLevelStat(num, -1);
                            await messageStatDistributor(character, component, num);
                            break;
                        case "plus":
                            if(character.DistrPoints[0] > 0 && character.Stats[num] < 8 )
                            {
                                character.AddLevelStat(num);
                            }
                            await messageStatDistributor(character, component, num);
                            break;
                        case "back":
                            await messageStatDistributor(character, component, num == 0? 9 : num-1);
                            break;
                        case "next":
                            await messageStatDistributor(character, component, num == 9? 0 : num+1);
                            break;
                        case "confirm":

                            break;
                    }

                }
            }
        }

    }

}