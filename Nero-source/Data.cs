using System;
using Discord;
using Newtonsoft.Json;

// The data here needs to be standardized for saving and loading with future easier implementation of new things that will need to be saved. (I dont know yet how exacly this should look but i want this on json because i can't afford a database)

namespace Nero.Data;

public class Log {
    public ulong ChannelID { get; set; }
    public ulong AuthorID { get; set; }
    public string LogMessage { get; set; }

    public Log(ulong channelID, ulong authorID, string logMessage) {
        ChannelID = channelID;
        AuthorID = authorID;
        LogMessage = logMessage;
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
            if(path.EndsWith(".json") == false) {
                if(Directory.Exists(path) == false) {
                    Directory.CreateDirectory(path);
                }
            } else {
                if(File.Exists(path) == false) {
                    File.Create(path).Close();
                }
            }
        }
    }

    public void SaveLog(ulong guildID, ulong channelID, ulong authorID, string LogMessage) {

        CreateLocalFiles(guildID, channelID);

        var logFilePath = Path.Combine(botDataPath, "guilds", guildID.ToString(), "sessions", $"{channelID}.json");

        if(File.Exists(logFilePath) == false) {
            File.Create(logFilePath).Close();
        }

        string fileContents = File.ReadAllText(logFilePath);

        List<Log> log;
        
        log = JsonConvert.DeserializeObject<List<Log>>(fileContents) ?? new List<Log>();

        log.Add(new Log(channelID, authorID, LogMessage));

        File.WriteAllText(logFilePath, JsonConvert.SerializeObject(log, Formatting.Indented));

    }

    public List<Log> GetLogs(ulong guildID, ulong channelID) {

        CreateLocalFiles(guildID, channelID);

        var logFilePath = Path.Combine(botDataPath, "guilds", guildID.ToString(), "sessions", $"{channelID}.json");

        if(File.Exists(logFilePath) == false) {
            File.Create(logFilePath).Close();
        }

        string fileContents = File.ReadAllText(logFilePath);

        List<Log> log;
        
        log = JsonConvert.DeserializeObject<List<Log>>(fileContents) ?? new List<Log>();

        return log;

    }

}