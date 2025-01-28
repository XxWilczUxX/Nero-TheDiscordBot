
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
            Type VARCHAR(50)
        );
    ";
    public static readonly string CreateAttributesTable = @"
        CREATE TABLE IF NOT EXISTS Attributes (
            AttributeID INTEGER PRIMARY KEY,
            AttributeName VARCHAR(50)
        );
    ";
    public static readonly string CreateSkillsTable = @"
        CREATE TABLE IF NOT EXISTS Skills (
            SkillID INTEGER PRIMARY KEY,
            SkillName VARCHAR(50),
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
            RoleName VARCHAR(50)
        );
    ";
    public static readonly string CreateCharactersTable = @" 
        CREATE TABLE IF NOT EXISTS Characters (
            CharacterID INTEGER PRIMARY KEY,
            UserID INTEGER NOT NULL,
            RoleID INTEGER NOT NULL,
            CharacterName VARCHAR(50),
            CharacterNickName VARCHAR(50),
            Original INTEGER DEFAULT 1,
            ImprovementPoints INTEGER, 

            FOREIGN KEY (UserID) REFERENCES Users(UserID),
            FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
        );
    "; // TO-DO: Add all columns
    public static readonly string CreateCharacterDetailsTable = @"
        CREATE TABLE IF NOT EXISTS CharactersDetails (
            CharacterID INTEGER PRIMARY KEY,
            Origin VARCHAR(255),
            Birthplace VARCHAR(255),
            DateOfBirth DATE,
            FamilyBackground TEXT,
            Height DECIMAL(5, 2),
            Weight DECIMAL(5, 2),
            HairDescription VARCHAR(255),
            EyeColor VARCHAR(50),
            DistinguishingFeatures TEXT,
            OverallDescription TEXT,
            PersonalityTraits TEXT,
            Temperament VARCHAR(100),
            EthicsValues TEXT,
            SocialBehavior TEXT,
            Backstory TEXT,
            MajorLifeEvents TEXT,
            GoalsMotivations TEXT,
            FOREIGN KEY (CharacterID) REFERENCES Characters(CharacterID)
        );
    ";
    public static readonly string CreateCharacterSkillsTable = @"
        CREATE TABLE IF NOT EXISTS CharacterSkills (
            CharacterID INTEGER,
            SkillID INTEGER,
            Level INTEGER,
            FOREIGN KEY (CharacterID) REFERENCES Characters(CharacterID),
            FOREIGN KEY (SkillID) REFERENCES Skills(SkillID),
            PRIMARY KEY (CharacterID, SkillID)
        );
    ";
    public static readonly string CreateCharacterAttributesTable = @"
        CREATE TABLE IF NOT EXISTS CharacterAttributes (
            CharacterID INTEGER,
            AttributeID INTEGER,
            Level INTEGER,
            FOREIGN KEY (CharacterID) REFERENCES Characters(CharacterID),
            FOREIGN KEY (AttributeID) REFERENCES Attributes(AttributeID),
            PRIMARY KEY (CharacterID, AttributeID)
        );
    ";
    public static readonly string CreateCharactersInSessionsTable = @"
        CREATE TABLE IF NOT EXISTS CharactersInSessions (
            CharacterID INTEGER,
            SessionID INTEGER,
            FOREIGN KEY (CharacterID) REFERENCES Characters(CharacterID),
            FOREIGN KEY (SessionID) REFERENCES Sessions(SessionID),
            PRIMARY KEY (CharacterID, SessionID)
        );
    ";

}