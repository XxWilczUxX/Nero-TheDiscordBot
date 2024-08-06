using System;
using Discord;
using Newtonsoft.Json;

namespace Nero;

class Log {
    public ulong ChannelID { get; set; }
    public ulong AuthorID { get; set; }
    public string LogMessage { get; set; }

    public Log(ulong channelID, ulong authorID, string logMessage) {
        ChannelID = channelID;
        AuthorID = authorID;
        LogMessage = logMessage;
    }

}
public class DataController {

    private static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);   
    private static string botDataPath = Path.Combine(appDataPath, "CPBot");

    private void PathCheck(ulong guildID, ulong channelID) {

        if (Directory.Exists(botDataPath) == false) {
            Directory.CreateDirectory(botDataPath);
        }

        if(Directory.Exists(Path.Combine(botDataPath, guildID.ToString(), channelID.ToString())) == false) {
            Directory.CreateDirectory(Path.Combine(botDataPath, guildID.ToString(), channelID.ToString()));
        }

    }

    public void SaveLog(ulong guildID, ulong channelID, ulong authorID, string LogMessage) {

        PathCheck(guildID, channelID);

        var logFilePath = Path.Combine(botDataPath, guildID.ToString(), channelID.ToString(), "log.json");

        if(File.Exists(logFilePath) == false) {
            File.Create(logFilePath).Close();
        }

        string fileContents = File.ReadAllText(logFilePath);

        List<Log> log;
        
        log = JsonConvert.DeserializeObject<List<Log>>(fileContents) ?? new List<Log>();

        log.Add(new Log(channelID, authorID, LogMessage));

        File.WriteAllText(logFilePath, JsonConvert.SerializeObject(log, Formatting.Indented));

    }

}