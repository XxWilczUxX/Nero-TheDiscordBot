using Newtonsoft.Json;

namespace Nero.Data.SessionData;

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

    public void Save() {
        var path = Path.Combine(AppData.botDataPath, "guilds", GuildID.ToString(), "sessions", $"{ChannelID}.json");
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