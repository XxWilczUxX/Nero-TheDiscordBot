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
        public ulong ID { get; set; }
        public string Type { get; }
        public INavigation Navigation { get; set; }
    }

    
    public static class SaveableFactory {
        public static void Save(Saveable saveable) {
            var path = $"./saves/{saveable.Type}.json";

            
            var json = JsonConvert.SerializeObject(saveable);
            Console.WriteLine(json);
        }
    }


    public class Actions { // Maybe i can make a inherited class from Saveable, so i don't fuck up the interface idk...

        public async Task NavigationHandler(SocketMessageComponent component) {
            
            
            
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