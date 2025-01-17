using Newtonsoft.Json;
using Nero.Data.SessionData;

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
        DataController.CreateLocalFiles();

        var filePath = Path.Combine(AppData.botDataPath, "settings", "token.json");
        var deserializedInfo = DataHelper.LoadData<DeserializedSecret>(filePath);
        if (deserializedInfo != null)
        {
            Token = deserializedInfo.Token;
            TestplaceID = deserializedInfo.TestplaceID;
            HeadAdminID = deserializedInfo.HeadAdminID;
        }
    }

    private class DeserializedSecret
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
        DataController.CreateLocalFiles();

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

    private static void EnsureDirectoryExists(string path) {
        if(Directory.Exists(path) == false) {
            Directory.CreateDirectory(path);
        }
    }
    private static void EnsureFileExists(string path) {
        if(File.Exists(path) == false) {
            File.Create(path).Close();
        }
    }

    public static void CreateLocalFiles(ulong guildID = 0, ulong channelID = 0, ulong userID = 0) {
        var paths = new List<string> {
            AppData.botDataPath,
            Path.Combine(AppData.botDataPath, "settings"),
            Path.Combine(AppData.botDataPath, "guilds"),
            Path.Combine(AppData.botDataPath, "users"),
        };

        var files = new List<string> {
            Path.Combine(paths.ElementAt(1), "settings.json"),
            Path.Combine(paths.ElementAt(1), "token.json"),
        };

        if (guildID != 0) {
            var guildPath = Path.Combine(paths.ElementAt(2), guildID.ToString(), "sessions");
            paths.Add(guildPath);

            if (channelID != 0) {
                files.Add(Path.Combine(guildPath, $"{channelID}.json"));
            }
        }

        if (userID != 0) {
            var userPath = Path.Combine(paths.ElementAt(3), $"{userID}");
            paths.Add(userPath);
            paths.Add(Path.Combine(userPath, "characters"));
            files.Add(Path.Combine(userPath, $"user.json"));
        }

        foreach (var path in paths) {
            EnsureDirectoryExists(path);
        }

        foreach (var file in files) {
            EnsureFileExists(file);
        }
    }

}