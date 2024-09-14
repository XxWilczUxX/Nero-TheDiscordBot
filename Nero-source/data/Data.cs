using Newtonsoft.Json;
using Nero.Data.SessionData;
using Nero.Data.UserData;

namespace Nero.Data;

public class AppData {
    private static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);   
    public static readonly string botDataPath = Path.Combine(appDataPath, "CPBot");

    public static readonly Settings settings = new Settings();
}

public static class DataHelper
{
    public static T? LoadData<T>(string filePath) where T : class
    {
        try
        {
            var fileContents = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(fileContents);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }
}

public class Secret
{
    public string Token { get; private set; } = string.Empty;
    public ulong TestplaceID { get; }
    public ulong HeadAdminID { get; }

    public Secret()
    {
        var dataController = new DataController();
        dataController.CreateLocalFiles();

        var filePath = Path.Combine(AppData.botDataPath, "settings", "token.json");
        var deserializedInfo = DataHelper.LoadData<DeserializedInfo>(filePath);
        if (deserializedInfo != null)
        {
            Token = deserializedInfo.Token;
            TestplaceID = deserializedInfo.TestplaceID;
            HeadAdminID = deserializedInfo.HeadAdminID;
        }
    }

    private class DeserializedInfo
    {
        public string Token { get; set; } = string.Empty;
        public ulong TestplaceID { get; set; }
        public ulong HeadAdminID { get; set; }
    }
}

public class Settings
{
    public int MaxSessionsPerUser { get; private set; } = 5;
    public int MaxCharactersPerUser { get; private set; } = 5;

    public Settings()
    {
        var dataController = new DataController();
        dataController.CreateLocalFiles();

        var filePath = Path.Combine(AppData.botDataPath, "settings", "settings.json");
        var settings = DataHelper.LoadData<Settings>(filePath);
        if (settings != null)
        {
            MaxSessionsPerUser = settings.MaxSessionsPerUser;
            MaxCharactersPerUser = settings.MaxCharactersPerUser;
        }
    }
}

public class DataController { 

    public void CreateLocalFiles(ulong guildID = 0, ulong channelID = 0, ulong userID = 0) {
        var paths = new List<string> {
            AppData.botDataPath,
            Path.Combine(AppData.botDataPath, "settings"),
            Path.Combine(AppData.botDataPath, "settings", "settings.json"),
            Path.Combine(AppData.botDataPath, "settings", "token.json"),
            Path.Combine(AppData.botDataPath, "guilds"),
            Path.Combine(AppData.botDataPath, "users"),
        };

        if(guildID != 0) {
            paths.Add(Path.Combine(AppData.botDataPath, "guilds", guildID.ToString(), "sessions"));
            
            if(channelID != 0) {
                paths.Add(Path.Combine(AppData.botDataPath, "guilds", guildID.ToString(), "sessions", $"{channelID}.json"));
            }
        }

        if(userID != 0) {
            paths.Add(Path.Combine(AppData.botDataPath, "users", $"{userID}", $"{userID}.json"));
            paths.Add(Path.Combine(AppData.botDataPath, "users", $"{userID}", "characters"));
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

    public void SaveSession(ulong guildID, ulong channelID, ulong userID) {

        CreateLocalFiles(guildID, channelID, userID);

        var sessionFilePath = Path.Combine(AppData.botDataPath, "guilds", guildID.ToString(), "sessions", $"{channelID}.json");

        if(File.Exists(sessionFilePath) == false) {
            File.Create(sessionFilePath).Close();
        }

        Session session = new Session(guildID, channelID);
        session.Load(sessionFilePath);

        session.Save();

        var userFilePath = Path.Combine(AppData.botDataPath, "users", $"{userID}.json");

        if(File.Exists(userFilePath) == false) {
            File.Create(userFilePath).Close();
        }

        User user = new User(userID);
        user.Load(userFilePath);

        user.Save();

    }

    public void SaveLog(ulong guildID, ulong channelID, ulong authorID, string LogMessage) {

        CreateLocalFiles(guildID, channelID);

        var sessionFilePath = Path.Combine(AppData.botDataPath, "guilds", guildID.ToString(), "sessions", $"{channelID}.json");

        if(File.Exists(sessionFilePath) == false) {
            File.Create(sessionFilePath).Close();
        }

        string fileContents = File.ReadAllText(sessionFilePath);

        Session session = new Session(guildID, channelID);
        session.Load(sessionFilePath);

        session.Logs.Add(new Log(authorID, LogMessage));

        session.Save();

    }

    public List<Log> GetLogs(ulong guildID, ulong channelID) {

        CreateLocalFiles(guildID, channelID);

        var sessionFilePath = Path.Combine(AppData.botDataPath, "guilds", guildID.ToString(), "sessions", $"{channelID}.json");

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

        var sessionFilePath = Path.Combine(AppData.botDataPath, "guilds", guildID.ToString(), "sessions", $"{channelID}.json");

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