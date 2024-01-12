using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;

namespace Nero
{

    class Character
    {
        protected readonly string[] abilityNames = { "Combat Sense", "Interface", "Jurry Rig", "Credibility", "Family", "Authority","Streetdeal", "Resources", "Medical Tech", "Charismatic Leadership", "Heavy Weapons (x2)", "Long Weapons", "Short Weapons", "Archery", "Continous Fire", "Athletics", "Rubber Man", "Torture/Narcotic Tolerancy", "Stealth", "Dance", "Endurance", "Bureaucracy", "Deduction", "Language", "Composing", "Criminology", "Cryptography", "Accounting", "Science", "Animal Care", "Library Searching", "Making Deals", "Art of Survival", "Tactics", "Local Knowlege", "Education", "Riding", "Piloting (x2)", "Car Driving", "Sailing", "Lip Reading", "Concentration", "Perception", "Tracking", "Hiding/Finding an Item", "Cyber Engineering", "Electronics and Security", "Falsification", "Photography", "Pickpocketing", "Art", "Explosives", "Weapon Repair", "Land Vehicle Repair", "Water Vehicle Repair", "Air Vehicle Repair", "Lockpicking", "First Aid", "Basic Repairing", "Paramedics", "Attractiveness", "Trading", "Conversation", "Fashion", "Emotional Inteligence", "Persuasion", "Bribery", "Interrogation", "Semi-Literate Knowlege", "Fighting", "Meele Weapons", "Martial Arts", "Dodgeing", "Acting", "Playing an Instrumet" };
        private string name;
        private string nickname;
        private int role;
        private int[] stats;
        private Ability[] abilities = new Ability[67];

        public Character(string name, string nickname, int role, int[] stats){
            this.name = name;
            this.nickname = nickname;
            this.role = role;
            this.stats = stats;
            this.abilities[0].name = abilityNames[role];
            for(int i = 1; i < this.abilities.Length; i++){
                this.abilities[i].name = abilityNames[i];
            }
        }

    }

    class Ability
    {
        public string name = "bob";
        public int level = 0;
    }
    
    public class CharacterEditor
    {
        
        protected readonly string[] abilityClassNames = { "Long Range Weapons", "Body", "Education", "Controll", "Observation", "Technology", "Social", "Meele", "Performances" };

        public async Task CharacterHandler(SocketSlashCommand command, string action){
            switch(action){
                case "create":
                    await CharacterCreate(command);
                    break;
                
            }
        }

        private async Task CharacterCreate(SocketSlashCommand command){

            

            await command.RespondAsync("it works!!!!");

        }

    }

}