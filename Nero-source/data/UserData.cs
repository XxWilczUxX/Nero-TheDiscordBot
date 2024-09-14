using Newtonsoft.Json;
using Nero.Data;
using Nero.Data.CharacterData;


namespace Nero.Data.UserData;

public class User {
    public ulong UserID { get; set; }
    private List<ulong> Sessions { get; set; } = new List<ulong>();

    public User(ulong userID) {
        UserID = userID;
    }

    private static string GetUserPath(ulong userID) {
        return Path.Combine(AppData.botDataPath, "users", $"{userID}");
    }

    public void Save() {
        var path = Path.Combine(GetUserPath(UserID), "user.json");
        File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    public void Load() {
        var path = GetUserPath(UserID);
        if(Directory.Exists(path) == false) {
            Directory.CreateDirectory(path);
        }
        if(File.Exists(Path.Combine(path, "user.json")) == false) {
            File.Create(Path.Combine(path, "user.json")).Close();
        }
        var user = JsonConvert.DeserializeObject<User>(File.ReadAllText(Path.Combine(path, "user.json")));
        if(user != null) {
            Sessions = user.Sessions;
        }
    }

    public void Load(string path) {
        if(File.Exists(path) == false) {
            File.Create(path).Close();
        }
        var user = JsonConvert.DeserializeObject<User>(File.ReadAllText(path));
        if(user != null) {
            UserID = user.UserID;
            Sessions = user.Sessions;
        }
    }

    public bool CanAddSession() {
        return Sessions.Count < AppData.settings.MaxSessionsPerUser;
    }

    public bool AddSession(ulong sessionID) {
        if(Sessions.Count >= AppData.settings.MaxSessionsPerUser) {
            return false;
        }
        Sessions.Add(sessionID);
        return true;
    }

    public static List<Character> getCharacters(ulong userID)
    {
        var characterFolder = Path.Combine(GetUserPath(userID), "characters");
        List<Character> characters = new List<Character>();
        if(Directory.Exists(characterFolder) == false) {
            return characters;
        }
        Directory.GetFiles(characterFolder).ToList().ForEach(file => {
            var character = JsonConvert.DeserializeObject<Character>(File.ReadAllText(file));
            if(character != null) {
                characters.Add(character);
            }
        });
        return characters;
    }

    public static bool canSaveCharacter(ulong userID)
    {
        var characterFolder = Path.Combine(GetUserPath(userID), "characters");
        if(Directory.Exists(characterFolder) == false) {
            return true;
        }
        return Directory.GetFiles(characterFolder).Length < AppData.settings.MaxCharactersPerUser;
    }

}