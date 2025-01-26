
namespace Nero.Data;
public static class DatabaseStructure
{
    public static readonly string CreateGuildsTable = @"
        CREATE TABLE IF NOT EXISTS Guilds (
            GuildID INTEGER PRIMARY KEY
        );
    ";
    public static readonly string CreateUsersTable = @"
        CREATE TABLE IF NOT EXISTS Users (
            UserID INTEGER PRIMARY KEY
        );
    ";
    public static readonly string CreateSessionsTable = @"
        CREATE TABLE IF NOT EXISTS Sessions (
            SessionID INTEGER PRIMARY KEY,
            GuildID INTEGER NOT NULL,
            ChannelID INTEGER NOT NULL,
            GameMasterID INTEGER NOT NULL,
            FOREIGN KEY (GuildID) REFERENCES Guilds(GuildID),
            FOREIGN KEY (GameMasterID) REFERENCES Users(UserID)
        );  
    ";
    public static readonly string CreateSessionEntriesTable = @"
        CREATE TABLE IF NOT EXISTS SessionEntries (
            LogID INTEGER PRIMARY KEY,
            SessionID INTEGER NOT NULL,
            LogText TEXT,
            FOREIGN KEY (SessionID) REFERENCES Sessions(SessionID)
        );
    ";
    public static readonly string CreateSkillTypesTable = @"
        CREATE TABLE IF NOT EXISTS SkillTypes (
            SkillTypeID INTEGER PRIMARY KEY,
            Type TEXT
        );
    ";
    public static readonly string CreateAttributesTable = @"
        CREATE TABLE IF NOT EXISTS Attributes (
            AttributeID INTEGER PRIMARY KEY,
            AttributeName TEXT
        );
    ";
    public static readonly string CreateSkillsTable = @"
        CREATE TABLE IF NOT EXISTS Skills (
            StatSkillID INTEGER PRIMARY KEY,
            StatSkillName TEXT,
            TypeID INTEGER NOT NULL,
            Multiplier INTEGER NOT NULL,
            LeadingAttributeID INTEGER NOT NULL,
            FOREIGN KEY (TypeID) REFERENCES SkillTypes(SkillTypeID),
            FOREIGN KEY (LeadingAttributeID) REFERENCES Attributes(AttributeID)
        );
    ";
    public static readonly string CreateRolesTable = @"
        CREATE TABLE IF NOT EXISTS Roles (
            RoleID INTEGER PRIMARY KEY,
            RoleName TEXT
        );
    ";
    public static readonly string CreateCharactersTable = @" 
        CREATE TABLE IF NOT EXISTS Characters (
            CharacterID INTEGER PRIMARY KEY,
            UserID NOT NULL,
            RoleID NOT NULL,
            CharacterName TEXT,
            CharacterNickName TEXT,

            FOREIGN KEY (UserID) REFERENCES Users(UserID),
            FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
        );
    "; // TO-DO: Add all columns


}