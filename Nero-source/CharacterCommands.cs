using Discord;
using Discord.WebSocket;
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

        async Task messageStatDistributor(Character character, SocketMessageComponent component, int num, string type, int sub = 0)
        {

            var names = new Nero.Names();
            
            string name = type == "stat" ? names.stats[num] : names.skills[num-1];
            int? level = type == "stat" ? character.Stats[num] : character.Skills[num].Level;
            int? cost = type == "stat"? 1 : character.Skills[num].Cost;

            switch(type) {
                case "stat":
                    name = names.stats[num];
                    level = character.Stats[num];
                    cost = 1;
                    break;
                case "skill":
                    name = character.Skills[num].Name;
                    level = character.Skills[num].Level;
                    cost = character.Skills[num].Cost;
                    break;
                case "subskill":
                    List<Skill>? subskills = character.Skills[num].SubSkills;
                    if(subskills != null) {
                        name = subskills.ElementAt(sub).Name;
                        level = subskills.ElementAt(sub).Level;
                        cost = subskills.ElementAt(sub).Cost;
                    }
                    break;
            }

            var embed = new EmbedBuilder()
                .WithTitle($"S{type.Substring(1)} Distribution")
                .WithDescription($"Distribute {type} points.")
                .AddField($"Current {type}: {name}", $"lvl: {level}\nCost: {cost}")
                .AddField("Available points:", character.DistrPoints[type == "stat"? 0 : 1])
            ;
            var buttonMinus = new ButtonBuilder()
                .WithCustomId($"{type}_add_{num}_-1")
                .WithLabel("-1")
                .WithStyle(ButtonStyle.Danger)
            ;
            var buttonPlus = new ButtonBuilder()
                .WithCustomId($"{type}_add_{num}_1")
                .WithLabel("+1")
                .WithStyle(ButtonStyle.Success)
            ;
            var buttonMin = new ButtonBuilder()
                .WithCustomId($"{type}_add_{num}_-8")
                .WithLabel("min")
                .WithStyle(ButtonStyle.Danger)
            ;
            var buttonMax = new ButtonBuilder()
                .WithCustomId($"{type}_add_{num}_8")
                .WithLabel("max")
                .WithStyle(ButtonStyle.Success)
            ;

            var buttonBack = new ButtonBuilder()
                .WithCustomId($"{type}_moveHor_{num}_-1")
                .WithLabel("<")
                .WithStyle(ButtonStyle.Primary)
            ;
            var buttonNext = new ButtonBuilder()
                .WithCustomId($"{type}_moveHor_{num}_1")
                .WithLabel(">")
                .WithStyle(ButtonStyle.Primary)
            ;
            var buttonDown = new ButtonBuilder()
                .WithCustomId($"{type}_moveVer_{num}_-1")
                .WithLabel("\\/")
                .WithStyle(ButtonStyle.Primary)
                .WithDisabled(type == "stat"? true : character.Skills[num].SubSkills == null ? true : false)
            ;
            var buttonUp = new ButtonBuilder()
                .WithCustomId($"{type}_moveVer_{num}_1")
                .WithLabel("/\\")
                .WithStyle(ButtonStyle.Primary)
                .WithDisabled(type == "stat"? true : character.Skills[num].SubSkills == null ? true : false)
            ;

            var buttonConfirm = new ButtonBuilder()
                .WithCustomId($"{type}_confirm")
                .WithLabel("----------------- .Confirm. ----------------")
                .WithStyle(ButtonStyle.Secondary)
                .WithDisabled(character.DistrPoints[type == "stat"? 0 : 1] == 0? false : true)
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
                if(component.Data.CustomId == "charCreateRole") // select menu
                {
                    int role;
                    int.TryParse(component.Data.Values.First(),out role);
                    character.SetRole(role, 4);

                    await messageStatDistributor(character, component, 0, "stat");
                }
                else // buttons
                {
                    string[] customID = component.Data.CustomId.Split("_").ToArray(); 
                    string type = customID[0];
                    string action = customID[1];
                    int id;
                    if(type == "stat")
                    {
                        switch(action)
                        {

                            case "moveHor":
                                
                                id = int.Parse(customID[2]);
                                int num = int.Parse(customID[3]);
                                if(id + num < 0)
                                {
                                    id = 9;
                                }
                                else if(id + num > 9)
                                {
                                    id = 0;
                                }
                                else
                                {
                                    id += num;
                                }
                                await messageStatDistributor(character, component, id, "stat");
                                break;

                            case "add":
                                id = int.Parse(customID[2]);
                                character.DistributePoints(type, id, int.Parse(customID[3]), new int[] {2,8});
                                await messageStatDistributor(character, component, id, "stat");
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
                            case "moveHor":
                                
                                id = int.Parse(customID[2]);
                                int num = int.Parse(customID[3]);
                                if(id + num < 1)
                                {
                                    id = 65;
                                }
                                else if(id + num > 65)
                                {
                                    id = 1;
                                }
                                else
                                {
                                    id += num;
                                }
                                await messageStatDistributor(character, component, id, "skill");
                                break;

                            case "add":
                                id = int.Parse(customID[2]);
                                int[] range = new int[] {0,8};
                                if(id == 6 || id == 9 || id == 14 || id == 9 || id == 19 || id == 25 || id == 26 || id == 32 || id == 33 || id == 48 || id == 53 || id == 55 || id == 56 || id == 60 || id == 63){
                                    range[0] = 2;
                                }
                                character.DistributePoints(type, id, int.Parse(customID[3]), range);
                                await messageStatDistributor(character, component, id, "skill");
                                break;
                            case "confirm":
                                await messageStatDistributor(character, component, 1, "skill");
                                break;
                            
                        }
                    }
                    else if(type == "subskill")
                    {
                        switch(action)
                        {
                            case "add": // this and vertical movement
                                id = int.Parse(customID[2]);
                                int[] range = new int[] {0,8};
                                if(id == 19){
                                    range[0] = 2;
                                }
                                character.DistributePoints(type, id, int.Parse(customID[3]), range);
                                await messageStatDistributor(character, component, id, "skill");
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