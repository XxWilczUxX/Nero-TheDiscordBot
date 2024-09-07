namespace Nero.Data.Character;

public class Character
{
    public string Name { get; private set; } = string.Empty;
    public byte[] StatLevels { get; private set; } = new byte[GameData.DefaultNames.Stats.Length];
    public byte[,] SkillLevels { get; private set; } = new byte[GameData.DefaultNames.Skills.GetLength(0), GameData.DefaultNames.Skills.GetLength(1)];
    public string[,] Skills { get; private set; } = new string[GameData.DefaultNames.Skills.GetLength(0), GameData.DefaultNames.Skills.GetLength(1)];
    public byte Role { get; private set; } = 0;
    public int Experience { get; private set; } = 0;

    public Character(string name)
    {
        Name = name;
    }

    
}