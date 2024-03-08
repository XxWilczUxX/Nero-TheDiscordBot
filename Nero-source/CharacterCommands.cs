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