using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Nero
{

    public class Character
    {
        public string Name;
        public string Description;
        public int Role {get; set;}
        public int[] Stats {get; set;} = new int[10];
        public Skill[] Skills {get; set;} = new Skill[66];
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

            for(int i = 1; i < Skills.Length; i++)
            {
                stat = 0;
                cost = 1;
                hasLevel = true;
                level = 0;
                subskill = null;
                if(i == 8 || i == 10 || i == 11 || i == 32) // SW
                {
                    stat = 5;
                }
                else if(i == 53 || i == 55) // EMP
                {
                    stat = 4;
                }
                else if(i < 6 || (26 < i && i < 31)) // REF
                {
                    stat = 1;
                }
                else if((5 < i && i < 10) || (59 < i && i < 64)) // ZW
                {
                    stat = 2;
                }
                else if(i > 11 && i < 36) // INT
                {
                    stat = 0;
                }
                else if((35 < i && i < 51) || i == 66) // TECH
                {
                    stat = 3;
                }
                else if(i != 0) // CHA
                {
                    stat = 9;
                }
                if(i == 1 || i == 5 || i == 28 || i == 37 || i == 42 || i == 50 || i == 62) // Cost
                {
                    cost = 2;
                }

                if(i == 14 || i == 19 || i == 25 || i == 65) // Subskill skills
                {
                    hasLevel = false;
                }

                if(i == 6 || i == 9 || i == 14 || i == 9 || i == 19 || i == 25 || i == 26 || i == 32 || i == 33 || i == 48 || i == 53 || i == 55 || i == 56 || i == 60 || i == 63) // Primary level allocation or creation of primary subskill
                {
                    if(!hasLevel)
                    {
                        switch(i)
                        {
                            case 14: // Language
                                subskill = new Skill(names.subskills[0], 2);
                                break;
                            case 25: // Local Knowlege
                                subskill = new Skill(names.subskills[1], 2);
                                break;
                        }
                    }
                    else
                    {
                        level = 2;
                    }
                }
                
                this.Skills[i] = new Skill(names.skills[i-1], stat, cost, hasLevel, level);
                if(subskill != null)
                {
                    this.Skills[i].AddSubskill(subskill);
                }

            }

        }

        public void SetRole(int role, int level)
        {
            Names names = new Names();
            this.Role = role;
            this.Skills[0] = new Skill(names.roles[1,this.Role], level);
        }

        public void AddLevelStat(int stat, int num = 1)
        {
            if((num > 0 && this.DistrPoints[0] == 0) || (num < 0 && this.Stats[stat] == 2))
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                this.Stats[stat] += num;
                this.DistrPoints[0] -= num;
            }
        }

        public void AddLevelSkill(int skill, int num = 1)
        {
            if((num > 0 && this.DistrPoints[1] - this.Skills[skill].Cost < 0) || (num < 0 && this.Skills[skill].Level == 2 && ( skill == 6 || skill == 9 || skill == 14 || skill == 9 || skill == 19 || skill == 25 || skill == 26 || skill == 32 || skill == 33 || skill == 48 || skill == 53 || skill == 55 || skill == 56 || skill == 60 || skill == 63 )))
            {
                throw new ArgumentOutOfRangeException();
            }
            
            if(this.Skills[skill].Level != null)
            {
                this.Skills[skill].Level += num;
                int? cost = this.Skills[skill].Cost;
                if(cost != null)
                {
                    this.DistrPoints[1] -= num * (int)cost;
                }
            }
        }

        public void MinStat(int stat)
        {
            int? level = this.Stats[stat];
            this.DistrPoints[0] += (int)level- 2;
            this.Stats[stat] = 2;
        }

        public void MaxStat(int stat)
        {
            int num = 8;
            if(this.DistrPoints[0] < num - this.Stats[stat] )
            {
                num = this.DistrPoints[0];
            }
            int? level = this.Stats[stat];
            this.DistrPoints[0] -= num;
            this.Stats[stat] += num;
        }

        public void MinSkill(int skill)
        {
            if( skill == 6 || skill == 9 || skill == 14 || skill == 9 || skill == 19 || skill == 25 || skill == 26 || skill == 32 || skill == 33 || skill == 48 || skill == 53 || skill == 55 || skill == 56 || skill == 60 || skill == 63 )
            {
                int? cost = this.Skills[skill].Cost;
                int? level = this.Skills[skill].Level;
                if(cost != null && level != null)
                {
                    this.DistrPoints[1] += ((int)level- 2) * (int)cost;
                    this.Skills[skill].Level = 2;
                }
            }
            else
            {
                int? cost = this.Skills[skill].Cost;
                int? level = this.Skills[skill].Level;
                if(cost != null && level != null)
                {
                    this.DistrPoints[1] += (int)level * (int)cost;
                    this.Skills[skill].Level = 0;
                }
            }
        }

        public void MaxSkill(int skill)
        {
            int? cost = this.Skills[skill].Cost;
            int? level = this.Skills[skill].Level;
            if(cost != null && level != null)
            {
                int num;
                if((8 - (int)level) * (int)cost > this.DistrPoints[1])
                {                        
                    num = this.DistrPoints[1];    
                }   
                else
                {
                    num = 8;
                }
                this.DistrPoints[1] -= (num - (int)level) * (int)cost;
                this.Skills[skill].Level = num;         
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
            if(hasLevel)
            {
                this.Cost = cost;
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
            this.Cost = 1;
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

        async Task messageStatDistributor(Character character, SocketMessageComponent component, int num, string action)
        {

            var names = new Nero.Names();

            var embed = new EmbedBuilder()
                .WithTitle($"S{action.Substring(1)} Distribution")
                .WithDescription($"Distribute {action} points of your character from 2 to 8")
                .AddField($"Current {action}: { (action == "stat"? names.stats[num] : names.skills[num-1]) }", $"lvl: {(action == "stat"? character.Stats[num] : character.Skills[num].Level)}\nCost: {(action == "stat"? 1 : character.Skills[num].Cost)}")
                .AddField("Available points:", character.DistrPoints[action == "stat"? 0 : 1])
            ;
            var buttonMinus = new ButtonBuilder()
                .WithCustomId($"{action}_minus_{num}")
                .WithLabel("-1")
                .WithStyle(ButtonStyle.Danger)
            ;
            var buttonPlus = new ButtonBuilder()
                .WithCustomId($"{action}_plus_{num}")
                .WithLabel("+1")
                .WithStyle(ButtonStyle.Success)
            ;
            var buttonBack = new ButtonBuilder()
                .WithCustomId($"{action}_back_{num}")
                .WithLabel("<")
                .WithStyle(ButtonStyle.Primary)
            ;
            var buttonNext = new ButtonBuilder()
                .WithCustomId($"{action}_next_{num}")
                .WithLabel(">")
                .WithStyle(ButtonStyle.Primary)
            ;
            var buttonMin = new ButtonBuilder()
                .WithCustomId($"{action}_min_{num}")
                .WithLabel("min")
                .WithStyle(ButtonStyle.Danger)
            ;
            var buttonMax = new ButtonBuilder()
                .WithCustomId($"{action}_max_{num}")
                .WithLabel("max")
                .WithStyle(ButtonStyle.Success)
            ;

            var buttonDown = new ButtonBuilder()
                .WithCustomId($"{action}_down_{num}")
                .WithLabel("\\/")
                .WithStyle(ButtonStyle.Primary)
            ;

            var buttonUp = new ButtonBuilder()
                .WithCustomId($"{action}_up_{num}")
                .WithLabel("/\\")
                .WithStyle(ButtonStyle.Primary)
            ;

            var buttonConfirm = new ButtonBuilder()
                .WithCustomId($"{action}_confirm")
                .WithLabel("----------------- .Confirm. ----------------")
                .WithStyle(ButtonStyle.Secondary)
                .WithDisabled(character.DistrPoints[action == "stat"? 0 : 1] == 0? false : true)
            ;

            var builder = new ComponentBuilder()
                .WithButton(buttonDown, 0)
                .WithButton(buttonBack, 0)
                .WithButton(buttonNext, 0)
                .WithButton(buttonUp, 0)
                .WithButton(buttonMin, 1)
                .WithButton(buttonMinus,1)
                .WithButton(buttonPlus,1)
                .WithButton(buttonMax, 1)
                .WithButton(buttonConfirm, 2)
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
                .AddOption("Solo", "0", "Lethal combat specialists skilled in weaponry, tactics, and survival.")
                .AddOption("Netrunner", "1", "Quick-hacking experts who manipulate cyberspace and infiltrate computer systems.")
                .AddOption("Techie", "2", "Inventive mechanics and engineers who excel at crafting and repairing technology.")
                .AddOption("Media", "3", "Hard-hitting journalists and media personalities who uncover secrets and shape public opinion.")
                .AddOption("Cop", "4", "Duty-bound enforcers of the law, maintaining order in a chaotic world.")
                .AddOption("Nomad", "5", "Range-riding wanderers who thrive in the wastelands between cities, skilled in survival and more.")
                .AddOption("Fixer", "6", "Clever negotiators and dealmakers who connect people and facilitate transactions.")
                .AddOption("Corporate", "7", "Scheming corporate executives who wield power and influence behind the scenes.")
                .AddOption("Medtech", "8", "Lifesaving healers and medics who can also take lives when necessary.")
                .AddOption("Rockerboy / Rockergirl", "9", "Charismatic rebels who use music, art, and charisma to inspire and lead others.")
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
                    int role;
                    int.TryParse(component.Data.Values.First(),out role);
                    character.SetRole(role, 4);

                    await messageStatDistributor(character, component, 0, "stat");
                
                }
                else
                {
                    string[] id = component.Data.CustomId.Split("_").ToArray(); 
                    string type = id[0];
                    string action = id[1];
                    int num = 0;
                    if(id.Length > 2){
                        int.TryParse(id[2], out num);
                    }

                    if(type == "stat")
                    {
                        switch(action)
                        {
                            case "minus":
                                try{ character.AddLevelStat(num, -1); }
                                catch(Exception ex){
                                    Console.WriteLine(ex.Message);
                                    await messageStatDistributor(character, component, num, "stat");
                                    break;
                                }
                                await messageStatDistributor(character, component, num, "stat");
                                break;

                            case "min":
                                try{ character.MinStat(num); }
                                catch(Exception ex){
                                    Console.WriteLine(ex.Message);
                                    await messageStatDistributor(character, component, num, "stat");
                                    break;
                                }
                                await messageStatDistributor(character, component, num, "stat");
                                break;

                            case "plus":
                                if(character.DistrPoints[0] > 0 && character.Stats[num] < 8 )
                                {
                                    character.AddLevelStat(num);
                                }
                                await messageStatDistributor(character, component, num, "stat");
                                break;

                            case "max":
                                character.MaxStat(num);
                                await messageStatDistributor(character, component, num, "stat");
                                break;

                            case "back":
                                await messageStatDistributor(character, component, num == 0? 9 : num-1, "stat");
                                break;

                            case "next":
                                await messageStatDistributor(character, component, num == 9? 0 : num+1, "stat");
                                break;

                            case "confirm":
                                await messageStatDistributor(character, component, 1, "skill");
                                break;
                        }
                    }
                    else if(type == "skill")
                    {
                        switch(action)
                        {
                            case "minus":
                                try{ character.AddLevelSkill(num, -1); }
                                catch(Exception ex){
                                    Console.WriteLine(ex.Message);
                                    await messageStatDistributor(character, component, num, "skill");
                                    break;
                                }
                                await messageStatDistributor(character, component, num, "skill");
                                break;

                            case "min":
                                try{ character.MinSkill(num); }
                                catch(Exception ex){
                                    Console.WriteLine(ex.Message);
                                    await messageStatDistributor(character, component, num, "skill");
                                    break;
                                }
                                await messageStatDistributor(character, component, num, "skill");
                                break;

                            case "plus":
                                if(character.DistrPoints[1] - character.Skills[num].Cost >= 0 && character.Skills[num].Level < 8 )
                                {
                                    character.AddLevelSkill(num);
                                }
                                await messageStatDistributor(character, component, num, "skill");
                                break;

                            case "max":
                                try{ character.MaxSkill(num); }
                                catch(Exception ex){
                                    Console.WriteLine(ex.Message);
                                    await messageStatDistributor(character, component, num, "skill");
                                    break;
                                }
                                await messageStatDistributor(character, component, num, "skill");
                                break;

                            case "back":
                                await messageStatDistributor(character, component, num == 1? 65 : num-1, "skill");
                                break;

                            case "next":
                                await messageStatDistributor(character, component, num == 65? 1 : num+1, "skill");
                                break;

                            case "confirm":
                                await messageStatDistributor(character, component, 1, "skill");
                                break;
                        }
                    }
                }
            }
        }

    }

}