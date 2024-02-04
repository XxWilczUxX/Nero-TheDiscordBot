using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nero
{

    class Character
    {
        Names names = new Names();
        public string name;
        public string nickname;
        public string description;
        public int role;
        public int[] stats;
        public int[] health;
        public int[] humanity;
        public Ability[] abilities;

        public Character(string name, string nickname, int role, string desc, int[] mainStats){
            this.abilities = new Ability[66];
            this.name = name;
            this.nickname = nickname;
            this.role = role;
            this.description = desc;
            this.stats = mainStats;
            this.abilities[0] = new Ability(names.roleSuperAbilityGroups[role]);
            this.abilities[0].level = 4;
            this.health = new int[2] {/*somefuckingwierdformulaidontrememberbcidonthaveaguide T-T*/80, 80};
            this.humanity = new int[2];
            if(mainStats != null){
                this.humanity[0] = this.stats[9] * 10;
                this.humanity[1] = this.stats[9] * 10;
            }
            for(int i = 1; i < this.abilities.Length-1; i++){
                this.abilities[i] = new Ability(names.abilities[i]);
            }
        }

    }

    class Ability
    {
        public string name = "";
        public int level = 0;

        public Ability(string name)
        {
            this.name = name;
            this.level = 0;
        }
    }
    
    public class CharacterEditor
    {
        public EmbedBuilder ErrorEmbed(string message)
        {
            var embed = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithTitle("ERROR")
                .WithDescription(message)
                .WithCurrentTimestamp()
            ;
            return embed;
        }

        private List<Character> characters = new List<Character>();
        Names names = new Names();
        public async Task CharacterHandler(SocketSlashCommand command, string action){
            
            try
            {
                Console.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), "Nero-source\\json\\characters.json"));
                characters = JsonConvert.DeserializeObject<List<Character>>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Nero-source\\json\\characters.json")));
                
                if(characters.Count==0){
                    characters = new List<Character>();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}, TL;DR : characters.json doesn't exist, is empty or isn't an array of Character class.");
                characters = new List<Character>();
            }


            switch(action){
                case "list":
                    await CharacterListout(command);
                    break;
                case "create":
                    await CharacterCreate(command);
                    break;
            }


            File.WriteAllText(Path.Combine("Nero-source/json/characters.json"), JsonConvert.SerializeObject(characters, formatting: Formatting.Indented));
        }

        private async Task CharacterListout(SocketSlashCommand command){

            var embed = new EmbedBuilder()
            .WithTitle("Characters:");
            for(int i = 0; i < characters.Count; i++){
                embed.AddField($"{characters.ToArray()[i].name}: \"{characters.ToArray()[i].nickname}\"", $"Role: {names.roles[characters.ToArray()[i].role]}");
            }
            await command.RespondAsync(embed: embed.Build());

        }

        private async Task CharacterCreate(SocketSlashCommand command){
            
            var data = command.Data.Options.First().Options.ToArray();

            string[] statsArrString = data[4].Value.ToString().Split(",");
            int[] statsArr = new int[10];
            if(statsArrString.Length != 10)
            {
                await command.RespondAsync(embed: ErrorEmbed("Number of given stats < 10").Build());
            }
            else
            {
                for(int i = 0; i < 10; i++)
                {
                    if(!int.TryParse(statsArrString[i], out statsArr[i]))
                    {
                        await command.RespondAsync(embed: ErrorEmbed("Stats given in wrong format or they aren't a number").Build());
                    }
                }
                if(statsArr.Sum() != 62){
                    await command.RespondAsync(embed: ErrorEmbed("Sum of stats isn't 62").Build());
                }
            }


            Character character = new Character(data[0].Value.ToString(), data[1].Value.ToString(), Convert.ToInt32(data[2].Value), data[3].Value.ToString(), statsArr);
            characters.Add(character);

            await command.RespondAsync($"Created Character {character.name}: The {names.roles[character.role]}");

        }

    }

}