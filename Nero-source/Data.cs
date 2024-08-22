using System;
using System.Xml.Linq;
using Discord;
using Newtonsoft.Json;

// The data here needs to be standardized for saving and loading with future easier implementation of new things that will need to be saved. (I dont know yet how exacly this should look but i want this on json because i can't afford a database)

namespace Nero.Data;

public class Log {
    public ulong AuthorID { get; set; }
    public string LogMessage { get; set; }

    public Log(ulong authorID, string logMessage) {
        AuthorID = authorID;
        LogMessage = logMessage;
    }

}

public class Session {
    public ulong GuildID { get; set; }
    public ulong ChannelID { get; set; }
    public List<Log> Logs { get; set; } = new List<Log>();
    public Session(ulong guildID, ulong channelID) {
        GuildID = guildID;
        ChannelID = channelID;
    }

    public void Save(string path) {
        File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
    }
    public void Load(string path) {
        var session = JsonConvert.DeserializeObject<Session>(File.ReadAllText(path));
        if(session != null) {
            ChannelID = session.ChannelID;
            Logs = session.Logs;
        }
    }
}

public class User {
    public ulong UserID { get; set; }
    public List<ulong> Sessions { get; set; } = new List<ulong>();

    public User(ulong userID) {
        UserID = userID;
    }

}
public class DataController { 

    private static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);   
    private static string botDataPath = Path.Combine(appDataPath, "CPBot");

    public void CreateLocalFiles(ulong guildID = 0, ulong channelID = 0, ulong userID = 0) {
        var paths = new List<string> {
            botDataPath,
            Path.Combine(botDataPath, "settings"),
            Path.Combine(botDataPath, "guilds"),
            Path.Combine(botDataPath, "users")
        };

        if(guildID != 0) {
            paths.Add(Path.Combine(botDataPath, "guilds", guildID.ToString(), "sessions"));
            
            if(channelID != 0) {
                paths.Add(Path.Combine(botDataPath, "guilds", guildID.ToString(), "sessions", $"{channelID}.json"));
            }
        }

        if(userID != 0) {
            paths.Add(Path.Combine(botDataPath, "users", $"{userID}.json"));
        }

        foreach (var path in paths) {
            if(path.EndsWith(".json")){
                if(!File.Exists(path)) {
                    File.Create(path).Close();
                }
            } 
            else {
                if(!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
            }
        }
    }

    public void SaveLog(ulong guildID, ulong channelID, ulong authorID, string LogMessage) {

        CreateLocalFiles(guildID, channelID);

        var sessionFilePath = Path.Combine(botDataPath, "guilds", guildID.ToString(), "sessions", $"{channelID}.json");

        if(File.Exists(sessionFilePath) == false) {
            File.Create(sessionFilePath).Close();
        }

        string fileContents = File.ReadAllText(sessionFilePath);

        Session session = JsonConvert.DeserializeObject<Session>(fileContents) ?? new Session(guildID, channelID);

        session.Logs.Add(new Log(authorID, LogMessage));

        File.WriteAllText(sessionFilePath, JsonConvert.SerializeObject(session, Formatting.Indented));

    }

    public List<Log> GetLogs(ulong guildID, ulong channelID) {

        CreateLocalFiles(guildID, channelID);

        var sessionFilePath = Path.Combine(botDataPath, "guilds", guildID.ToString(), "sessions", $"{channelID}.json");

        if(File.Exists(sessionFilePath) == false) {
            File.Create(sessionFilePath).Close();
        }

        string fileContents = File.ReadAllText(sessionFilePath);

        Session session = JsonConvert.DeserializeObject<Session>(fileContents) ?? new Session(guildID, channelID);
        
        var logs = session.Logs;

        return logs;

    }

    public void DeleteLog(ulong guildID, ulong channelID, int logIndex) {

        CreateLocalFiles(guildID, channelID);

        var sessionFilePath = Path.Combine(botDataPath, "guilds", guildID.ToString(), "sessions", $"{channelID}.json");

        if(File.Exists(sessionFilePath) == false) {
            File.Create(sessionFilePath).Close();
        }

        string fileContents = File.ReadAllText(sessionFilePath);

        Session session = JsonConvert.DeserializeObject<Session>(fileContents) ?? new Session(guildID, channelID);
        
        var logs = JsonConvert.DeserializeObject<List<Log>>(fileContents) ?? new List<Log>();

        logs.RemoveAt(logIndex);

        File.WriteAllText(sessionFilePath, JsonConvert.SerializeObject(session, Formatting.Indented));

    }

}