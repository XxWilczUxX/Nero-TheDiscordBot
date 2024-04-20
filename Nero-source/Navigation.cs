using System;
using System.Net.NetworkInformation;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            var path = $"{Directory.GetCurrentDirectory()}\\Nero-source\\temp\\{guildID}";

            Directory.CreateDirectory(path);

            var settings = new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            File.WriteAllText( $"{path}\\{userID}.json" , JsonConvert.SerializeObject(saveable, settings: settings) );
        }

        public static T LoadTemp<T>(ulong guildID, ulong userID) {
            var path = $"{Directory.GetCurrentDirectory()}\\Nero-source\\temp\\{guildID}";

            if (Directory.Exists(path))
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented,
                    Converters = { new NeroNavigationConverter() { ParentType = typeof(NetworkArchitecture)} }
                };

                var filePath = $"{path}\\{userID}.json";


                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var saveable = JsonConvert.DeserializeObject<T>(json, settings: settings);
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


    public class NeroNavigationConverter : JsonConverter {

        public Type ParentType { get; set; }
        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(Nero.INavigation));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            JObject jo = JObject.Load(reader);
            // Based on the contents of the JObject, instantiate the correct concrete class that implements Nero.INavigation
            // For example, if your concrete class is Nero.Navigation, you might do:
            if(ParentType == typeof(NetworkArchitecture)) {
                return jo.ToObject<Nero.Floor>(serializer)!;
            }
            //else if(ParentType == typeof(OtherClass)) {
            //    return jo.ToObject<Nero.NextClass>(serializer)!;
            //}
            else {
                throw new Exception("Failed to deserialize object.");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }   


    public class NavigationActions { // Maybe i can make a inherited class from Saveable, so i don't fuck up the interface idk...

        public async Task NavigationHandler(SocketMessageComponent component) {
            
            
            
        }

        public static ComponentBuilder CreateComponent(INavigation content) {

            var builder = new ComponentBuilder()
                .WithButton("Left", $"nav_left", ButtonStyle.Primary, disabled: content.Left()==content)
                .WithButton("Right", $"nav_right", ButtonStyle.Primary, disabled: content.Right()==content)
                .WithButton("Up", $"nav_up", ButtonStyle.Primary, disabled: content.Up()==content)
                .WithButton("Down", $"nav_down", ButtonStyle.Primary, disabled: content.Down()==content)
            ;

            return builder;
        }

        public async Task Respond(SocketMessageComponent component, INavigation content) {
            
            var componentBuilder = CreateComponent(content);
            var embeds = new Embeds();
            var embedBuilder = embeds.Navigation(content);

            await component.UpdateAsync( message =>
                {
                    message.Components = componentBuilder.Build();
                    message.Embed = embedBuilder.Build();
                }
            );

        }

        public async Task Respond(SocketSlashCommand command, INavigation content) {
                
                var componentBuilder = CreateComponent(content);
                var embeds = new Embeds();
                var embedBuilder = embeds.Navigation(content);

                await command.RespondAsync(components: componentBuilder.Build(), embed: embedBuilder.Build());
        }

    }

}