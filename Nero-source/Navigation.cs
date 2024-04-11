using System;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Nero {

    public interface INavigation {
        public object Right();
        public object Left();
        public object Up();
        public object Down();

        public string GetContents();
    }

    public interface Saveable {
        public INavigation Navigation { get; set; }
        public void Save(ulong guildID, ulong clientID, bool isTemp = false);
        public void Load(ulong guildID, ulong clientID, bool isTemp = false);
    }

    
    public static class SaveableFactory {
        public static Saveable Load(ulong guildID, ulong clientID) {
            if(File.Exists($"temp\\{guildID}\\{clientID}.json")) {
                var json = File.ReadAllText($"temp\\{guildID}\\{clientID}.json");
                var saveable = JsonConvert.DeserializeObject<Saveable>(json);
                if (saveable != null)
                {
                    return saveable;
                }
            }

            if(File.Exists($"json\\guilds\\{guildID}\\networks\\{clientID}.json")) { // Note to myself: im dumb and i have to remake save and load methods and overall redesign saving system
                var json = File.ReadAllText($"data/{guildID}/{clientID}.json");
                var saveable = JsonConvert.DeserializeObject<Saveable>(json);
                if (saveable != null)
                {
                    return saveable;
                }
            }

            return null;
        }
    }


    public class Actions { // Maybe i can make a inherited class from Saveable, so i don't fuck up the interface idk...

        public async Task NavigationHandler(SocketMessageComponent component) {
            
            var customID = component.Data.CustomId.Split("_");

            
            
        }

        public async Task CreateComponent(INavigation content, string type) {

            var builder = new ComponentBuilder()
                .WithButton("Left", $"nav_left_{type}", ButtonStyle.Primary)
                .WithButton("Right", $"nav_right_{type}", ButtonStyle.Primary)
                .WithButton("Up", $"nav_up_{type}", ButtonStyle.Primary)
                .WithButton("Down", $"nav_down_{type}", ButtonStyle.Primary)
            ;
        }

    }

}