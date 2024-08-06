using System;
using Discord;

namespace Nero;

class Log {
    public ulong GuildID { get; set; }
    public ulong ChannelID { get; set; }
    public string AuthorID { get; set; }
    public string LogMessage { get; set; }

    public Log(ulong guildID, ulong channelID, string authorID, string logMessage) {
        GuildID = guildID;
        ChannelID = channelID;
        AuthorID = authorID;
        LogMessage = logMessage;
    }
}
public class DataController {

    private static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);   
    private static string botDataPath = Path.Combine(appDataPath, "CPBot");

    private void PathCheck(ulong guildID) {

        if (Directory.Exists(botDataPath) == false) {
            Directory.CreateDirectory(botDataPath);
        }

        if(Directory.Exists(Path.Combine(botDataPath, guildID.ToString())) == false) {
            Directory.CreateDirectory(Path.Combine(botDataPath, guildID.ToString()));
        }

    }

    public void SaveLog(ulong guildID, ulong channelID, string author, string LogMessage) {
        PathCheck(guildID);

        

    }

}