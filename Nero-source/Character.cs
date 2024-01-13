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
        public int role;
        public int[] stats;
        public Ability[] abilities;

        public Character(string name, string nickname, int role){
            this.abilities = new Ability[66];
            this.name = name;
            this.nickname = nickname;
            this.role = role;
            this.stats = new int[10] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
            this.abilities[0] = new Ability(abilityNames[role]);
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
                Console.WriteLine(ex.Message);
                characters = new List<Character>();
            }
            switch(action){
                case "create":
                    await CharacterCreate(command);
                    break;
                
            }
            File.WriteAllText(Path.Combine("Nero-source/json/characters.json"), JsonConvert.SerializeObject(characters));
        }

        private async Task CharacterCreate(SocketSlashCommand command){

            var data = command.Data.Options.ToArray();
            Character character = new Character(data[0].Value.ToString(), data[1].Value.ToString(), Convert.ToInt32(data[2].Value));
            characters.Add(character);
            Console.WriteLine(characters.Count);

            await command.RespondAsync("Done");

        }

    }

}