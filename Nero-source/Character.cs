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
        protected readonly string[] abilityNames = { "Combat Sense", "Interface", "Jurry Rig", "Credibility", "Family", "Authority", "Connections", "Resources", "Medical Tech", "Charismatic Leadership", "Heavy Weapons (x2)", "Long Weapons", "Short Weapons", "Archery", "Continous Fire", "Athletics", "Rubber Man", "Torture/Narcotic Tolerancy", "Stealth", "Dance", "Endurance", "Bureaucracy", "Deduction", "Language", "Composing", "Criminology", "Cryptography", "Accounting", "Science", "Animal Care", "Library Searching", "Making Deals", "Art of Survival", "Tactics", "Local Knowlege", "Education", "Riding", "Piloting (x2)", "Car Driving", "Sailing", "Lip Reading", "Concentration", "Perception", "Tracking", "Hiding/Finding an Item", "Cyber Engineering", "Electronics and Security", "Falsification", "Photography", "Pickpocketing", "Art", "Explosives", "Weapon Repair", "Land Vehicle Repair", "Water Vehicle Repair", "Air Vehicle Repair", "Lockpicking", "First Aid", "Basic Repairing", "Paramedics", "Attractiveness", "Trading", "Conversation", "Fashion", "Emotional Inteligence", "Persuasion", "Bribery", "Interrogation", "Semi-Literate Knowlege", "Fighting", "Meele Weapons", "Martial Arts", "Dodgeing", "Acting", "Playing an Instrument" };
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
            this.abilities[0] = new Ability(abilityNames[role]);
            this.abilities[0].level = 4;
            this.health = new int[2] {/*somefuckingwierdformulaidontrememberbcidonthaveaguide T-T*/80, 80};
            this.humanity = new int[2] {this.stats[9]*10, this.stats[9]*10};
            for(int i = 0; i < this.abilities.Length-1; i++){
                this.abilities[i+1] = new Ability(abilityNames[i+10]);
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
        
        protected readonly string[] abilityClassNames = { "Long Range Weapons", "Body", "Education", "Controll", "Observation", "Technology", "Social", "Meele", "Performances" };
        protected readonly string[] roleNames = { "Solo", "Netrunner", "Techie", "Media", "Cop", "Nomad", "Fixer", "Corporate", "Medtech", "Rockerboy / Rockergirl"};
        private List<Character> characters = new List<Character>();

        public async Task CharacterHandler(SocketSlashCommand command, string action){
            try
            {
                characters = JsonConvert.DeserializeObject<List<Character>>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Nero-source/json/characters.json")));
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
                embed.AddField($"{characters.ToArray()[i].name}: \"{characters.ToArray()[i].nickname}\"", $"Role: {roleNames[characters.ToArray()[i].role]}");
            }
            await command.RespondAsync(embed: embed.Build());

        }

        private async Task CharacterCreate(SocketSlashCommand command){
            
            var data = command.Data.Options.First().Options.ToArray();

            string[] statsArrString = data[4].Value.ToString().Split(",");
            int[] statsArr = new int[10];
            if(statsArrString.Length != 10)
            {
                await command.RespondAsync("Number of given stats < 10");
            }
            else
            {
                for(int i = 0; i < 10; i++)
                {
                    if(!int.TryParse(statsArrString[i], out statsArr[i]))
                    {
                        await command.RespondAsync("Stats given in wrong format or aren't a number");
                    }
                }
                if(statsArr.Sum() != 62){
                    await command.RespondAsync("Sum of stats isn't 62");
                }
            }

            Character character = new Character(data[0].Value.ToString(), data[1].Value.ToString(), Convert.ToInt32(data[2].Value), data[3].Value.ToString(), statsArr);
            characters.Add(character);

            await command.RespondAsync($"Created Character {character.name}: The {roleNames[character.role]}");

        }

    }

}