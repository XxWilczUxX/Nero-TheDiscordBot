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

        async Task messageStatDistributor(Character character, SocketMessageComponent component, string type, int[] pos)
        {

            var names = new Nero.Names();
            var subskills = character.Skills[pos[0]].SubSkills;
            var level = 0;
            var cost = 1;
            
            string name;
            if (type == "stat")
            {
                name = names.stats[pos[0]];
                level = character.Stats[pos[0]];
            }
            else if (subskills != null && pos[1] > 0)
            {
                name = subskills[pos[1] - 1].Name;
                level = subskills[pos[1] - 1]?.Level ?? 0;
            }
            else
            {
                name = character.Skills[pos[0]].Name;
                level = character.Skills[pos[0]].Level ?? -1;
                cost = character.Skills[pos[0]].Cost ?? 1;
            }
            

            var embed = new EmbedBuilder()
                .WithTitle($"S{type.Substring(1)} Distribution")
                .WithDescription($"Distribute {type} points.")
                .AddField($"Current {type}: {name}", $"lvl: {(level == -1? "none" : level)}\nCost: {cost}")
                .AddField("Available points:", character.DistrPoints[type == "stat"? 0 : 1])
            ;
            var buttonMinus = new ButtonBuilder()
                .WithCustomId($"{type}_add_{pos[0]}.{pos[1]}_-1")
                .WithLabel("-1")
                .WithStyle(ButtonStyle.Danger)
            ;
            var buttonPlus = new ButtonBuilder()
                .WithCustomId($"{type}_add_{pos[0]}.{pos[1]}_1")
                .WithLabel("+1")
                .WithStyle(ButtonStyle.Success)
            ;
            var buttonMin = new ButtonBuilder()
                .WithCustomId($"{type}_add_{pos[0]}.{pos[1]}_-8")
                .WithLabel("min")
                .WithStyle(ButtonStyle.Danger)
            ;
            var buttonMax = new ButtonBuilder()
                .WithCustomId($"{type}_add_{pos[0]}.{pos[1]}_8")
                .WithLabel("max")
                .WithStyle(ButtonStyle.Success)
            ;

            var buttonBack = new ButtonBuilder()
                .WithCustomId($"{type}_move_{pos[0]}.{pos[1]}_-1.0")
                .WithLabel("<")
                .WithStyle(ButtonStyle.Primary)
            ;
            var buttonNext = new ButtonBuilder()
                .WithCustomId($"{type}_move_{pos[0]}.{pos[1]}_1.0")
                .WithLabel(">")
                .WithStyle(ButtonStyle.Primary)
            ;
            var buttonDown = new ButtonBuilder()
                .WithCustomId($"{type}_move_{pos[0]}.{pos[1]}_0.-1")
                .WithLabel("\\/")
                .WithStyle(ButtonStyle.Primary)
                .WithDisabled(type == "stat"? true : subskills == null ? true : false)
            ;
            var buttonUp = new ButtonBuilder()
                .WithCustomId($"{type}_move_{pos[0]}.{pos[1]}_0.1")
                .WithLabel("/\\")
                .WithStyle(ButtonStyle.Primary)
                .WithDisabled(type == "stat"? true : subskills == null ? true : false)
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

                    int[] pos = new int[] {0,0};

                    await messageStatDistributor(character, component, "stat", pos);
                }
                else // buttons
                {
                    string[] customID = component.Data.CustomId.Split("_").ToArray(); 
                    string type = customID[0];
                    string action = customID[1];
                    int[] pos = new int[] {0,0};
                    if(customID.Length > 2){
                        pos = customID[2].Split(".").Select(x => int.Parse(x)).ToArray();
                    }

                    if(type == "stat")
                    {
                        
                        switch(action)
                        {

                            case "move":
                                
                                int[] vec = customID[3].Split(".").Select(x => int.Parse(x)).ToArray();
                                if(vec[0] != 0) {
                                    if(pos[0] + vec[0] < 0)
                                    {
                                        pos[0] = character.Stats.Length - 1;
                                    }
                                    else if(pos[0] + vec[0] > character.Stats.Length - 1)
                                    {
                                        pos[0] = 0;
                                    }
                                    else
                                    {
                                        pos[0] += vec[0];
                                    }
                                }

                                await messageStatDistributor(character, component, "stat", pos);
                                break;

                            case "add":
                                character.DistributePoints(type, pos, int.Parse(customID[3]), new int[] {2,8});
                                await messageStatDistributor(character, component, "stat", pos);
                                break;
                            case "confirm":
                                pos = new int[] {0,0};
                                await messageStatDistributor(character, component, "skill", pos );
                                break;
                            
                        }
                    }
                    else if(type == "skill")
                    {
                        switch(action)
                        {
                            case "move":
                                
                                int[] vec = customID[3].Split(".").Select(x => int.Parse(x)).ToArray();
                                if(vec[0] != 0) {
                                    if(pos[0] + vec[0] < 0)
                                    {
                                        pos[0] = character.Skills.Length - 1;
                                    }
                                    else if(pos[0] + vec[0] > character.Skills.Length - 1)
                                    {
                                        pos[0] = 0;
                                    }
                                    else
                                    {
                                        pos[0] += vec[0];
                                    }
                                    pos[1] = 0;
                                }
                                if (character.Skills[pos[0]].SubSkills != null)
                                {
                                    var subskills = character.Skills[pos[0]].SubSkills;
                                    if(subskills != null)
                                    {
                                        if (vec[1] != 0)
                                        {
                                            if (pos[1] + vec[1] < 0)
                                            {
                                                pos[1] = subskills.Count - 1;
                                            }
                                            else if (pos[1] + vec[1] > subskills.Count)
                                            {
                                                pos[1] = 0;
                                            }
                                            else
                                            {
                                                pos[1] += vec[1];
                                            }
                                        }
                                    }
                                }
                                await messageStatDistributor(character, component, "skill", pos);
                                break;


                            case "add":
                                int[] range = new int[] {0,8};
                                if(pos[0] == 6 || pos[0] == 9 || (pos[0] == 14 && pos[1] == 1) || (pos[0] == 25 && pos[1] == 1) || pos[0] == 26 || pos[0] == 32 || pos[0] == 33 || pos[0] == 48 || pos[0] == 53 || pos[0] == 55 || pos[0] == 56 || pos[0] == 60 || pos[0] == 63){
                                    range[0] = 2;
                                }
                                character.DistributePoints(type, pos, int.Parse(customID[3]), range);
                                await messageStatDistributor(character, component, "skill", pos);
                                break;
                            case "confirm":
                                await messageStatDistributor(character, component, "skill", pos);
                                break;
                            
                        }
                    }
                }
            }
        }

    }

}