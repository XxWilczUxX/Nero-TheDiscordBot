using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace Nero.Data;

public static class AppData
{
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

public static class DataController
{

    private static void EnsurePathExists(string path)
    {
        if (Path.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }
    }
    private static void EnsureFileExists(string path)
    {
        if (File.Exists(path) == false)
        {
            File.Create(path).Close();
        }
    }

    public static void CreateLocalJsonFiles()
    {

        EnsurePathExists(AppData.botDataPath);

        var files = new List<string> {
            Path.Combine(AppData.botDataPath, "settings.json"),
            Path.Combine(AppData.botDataPath, "token.json"),
        };

        foreach (var file in files)
        {
            EnsureFileExists(file);
        }
    }

}

public class Database
{
    private readonly string _connectionString;

    public Database(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SqliteConnection GetConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    public void Initialize()
    {
        using var connection = GetConnection();

    }

}

public class Secret
{
    public string Token { get; private set; } = string.Empty;
    public ulong TestplaceID { get; }
    public ulong HeadAdminID { get; }

    public Secret()
    {
        DataController.CreateLocalJsonFiles();

        var filePath = Path.Combine(AppData.botDataPath, "token.json");
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
        DataController.CreateLocalJsonFiles();

        var filePath = Path.Combine(AppData.botDataPath, "settings.json");
        var settings = DataHelper.LoadData<Settings>(filePath);
        if (settings != null)
        {
            MaxSessionsPerUser = settings.MaxSessionsPerUser;
            MaxCharactersPerUser = settings.MaxCharactersPerUser;
        }
    }
}