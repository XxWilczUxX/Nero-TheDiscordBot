using Nero.Data.GameData;


namespace Nero.Data.Character;

public class Character
{
    public string Name { get; private set; } = string.Empty;
    public byte[] StatLevels { get; private set; } = new byte[DefaultNames.Stats.Length];
    public byte[,] SkillLevels { get; private set; } = new byte[DefaultNames.Skills.GetLength(0), GameData.DefaultNames.Skills.GetLength(1)]; // [0, i] = skill, [1, i] = subskill
    public string[,] Skills { get; private set; } = DefaultNames.Skills; // [0, i] = skill name, [1, i] = subskill name
    public byte Role { get; private set; } = 0;
    public int Experience { get; private set; } = 0;

    public Character(string name)
    {
        Name = name;
    }


    public void SetStatLevel(byte stat_index, byte level)
    {
        StatLevels[stat_index] = level;

        throw new Exception("Stat not found.");
    }

    public byte getSkillIndex(string skill_name)
    {
        
        for(byte i = 0; i < Skills.GetLength(0); i++)
        {
            if(Skills[0, i] == skill_name)
            {
                return i;
            }
        }

        throw new Exception("Skill not found.");
    }

    public byte getSubskillIndex(string subskill_name)
    {
        for(byte i = 0; i < Skills.GetLength(1); i++)
        {
            if(Skills[1, i] == subskill_name)
            {
                return i;
            }
        }

        throw new Exception("Subskill not found.");
    }

    public void AddSubskill(string skill_name, string subskill_name)
    {
        byte skill_index = getSkillIndex(skill_name);

        Skills[1, skill_index] = subskill_name;
    }
    public void AddSubskill(byte skill_index, string subskill_name)
    {
        Skills[1, skill_index] = subskill_name;
    }

    public void SetSkillLevel(string skill_name, byte level)
    {
        byte skill_index = getSkillIndex(skill_name);

        SkillLevels[0, skill_index] = level;
    }

    public void SetSkillLevel(byte skill_index, byte level)
    {
        SkillLevels[0, skill_index] = level;
    }

    
}