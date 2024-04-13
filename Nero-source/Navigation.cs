using System;
using System.Net.NetworkInformation;
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
        public static void SaveTemp(Saveable saveable, ulong guildID, ulong userID) {
            var path = Path.Combine( Directory.GetCurrentDirectory(), $"\\temp\\{guildID}" );

            Directory.CreateDirectory(path);

            var settings = new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            File.WriteAllText( $"{path}\\{userID}.json" , JsonConvert.SerializeObject(saveable, settings: settings) );
        }

        public static Saveable LoadTemp(ulong guildID, ulong userID) {
            var path = Path.Combine( Directory.GetCurrentDirectory(), $"\\temp\\{guildID}" );

            if (Directory.Exists(path))
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented
                };

                var filePath = $"{path}\\{userID}.json";

                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var saveable = JsonConvert.DeserializeObject<Saveable>(json, settings: settings);
                    if (saveable != null)
                    {
                        return saveable;
                    }
                    else
                    {
                        throw new Exception("Failed to deserialize object.");
                    }
                }
                else
                {
                    throw new Exception("No file found");
                }
            }
            else {
                throw new Exception("No file found");
            }
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