using Nero.Data.GameData;
using Newtonsoft.Json;


namespace Nero.Data.CharacterData;

public class Stat
{
    public string Name { get; private set; } = string.Empty;
    public byte Level { get; private set; } = 0;

    public Stat(string name)
    {
        Name = name;
    }

    public void SetLevel(byte level)
    {
        Level = level;
    }
}

public class Skill
{
    public string Name { get; private set; } = string.Empty;
    public List<Skill>? Subskills { get; private set; }
    public byte Level { get; private set; } = 0;
    public byte Cost { get; private set; } = 1;

    public Skill(string name)
    {
        Name = name;
    }
    public Skill(string name, byte cost)
    {
        Name = name;
        Cost = cost;
    }

    public void AddSubskill(string subskill_name)
    {
        if(Subskills == null)
        {
            Subskills = new List<Skill>();
        }

        Subskills.Add(new Skill(subskill_name));
    }

    public void SetLevel(byte level)
    {
        Level = level;
    }
}

public class Role
{
    public string Name { get; private set; } = string.Empty;
    public byte Level { get; private set; } = 0;

    public Role()
    {
        Name = "Unassigned";
    }

    public Role(byte role_id)
    {
        Name = DefaultNames.Roles[role_id];
    }

    public void SetLevel(byte level)
    {
        Level = level;
    }
}

public class Character
{
    public string Name { get; private set; } = string.Empty;
    public List<Stat> Stats { get; private set; } = new List<Stat>();
    public List<Skill> Skills { get; private set; } = new List<Skill>();
    public Role Role { get; private set; } = new Role();
    public int Experience { get; private set; } = 0;
    
    public Character(string name, byte role_id = 255)
    {
        Name = name;

        if(role_id != 255)
        {
            Role = new Role(role_id);
        }

        foreach(string stat_name in DefaultNames.Stats)
        {
            Stats.Add(new Stat(stat_name));
        }
        for(byte i = 0; i < DefaultNames.Skills.GetLength(1)*2; i++)
        {
            if(i < DefaultNames.Skills.GetLength(1))
            {
                Skills.Add(new Skill(DefaultNames.Skills[0, i]));
                //Console.WriteLine($"Added skill: {Skills.Last().Name}");
            }
            else
            {
                if(DefaultNames.Skills[1, i-DefaultNames.Skills.GetLength(1)] != "")
                {
                    Skills.ElementAt(i-DefaultNames.Skills.GetLength(1)).AddSubskill(DefaultNames.Skills[1, i-DefaultNames.Skills.GetLength(1)]); 
                    //Console.WriteLine($"Added subskill: {Skills.ElementAt(i-DefaultNames.Skills.GetLength(1)).Subskills?.Last().Name}");   
                }
            }
        }
    }
    

    private byte getStatIndex(string stat_name)
    {
        for(byte i = 0; i < Stats.Count; i++)
        {
            if(Stats.ElementAt(i).Name == stat_name)
            {
                return i;
            }
        }

        throw new Exception("Stat not found.");
    }
    private byte getSkillIndex(string skill_name) // returns skill_index
    {
        
        for(byte i = 0; i < Skills.Count; i++)
        {
            if(Skills.ElementAt(i).Name == skill_name)
            {
                return i;
            }
        }

        throw new Exception("Skill not found.");
    }

    private byte[] getSubskillIndex(string subskill_name) // returns [skill_index, subskill_index]
    {
        for(byte skill_index = 0; skill_index < Skills.Count; skill_index++)
        {
            List<Skill>? subskills = Skills.ElementAt(skill_index).Subskills;
            if(subskills != null)
            {
                for(byte subskill_index = 0; subskill_index < subskills.Count; subskill_index++)
                {
                    if(subskills.ElementAt(subskill_index).Name == subskill_name)
                    {
                        return [ skill_index, subskill_index ];
                    }
                }
            }
        }

        throw new Exception("Subskill not found.");
    }

    public void SetStatLevel(byte stat_index, byte level)
    {
        Stats[stat_index].SetLevel(level);

        throw new Exception("Stat not found.");
    }
    public void SetStatLevel(string stat_name, byte level)
    {
        byte stat_index = getStatIndex(stat_name);

        throw new Exception("Stat not found.");
    }

    public void AddSubskill(string skill_name, string subskill_name)
    {
        byte skill_index = getSkillIndex(skill_name);

        Skills[skill_index].AddSubskill(subskill_name);
    }
    public void AddSubskill(byte skill_index, string subskill_name)
    {
        Skills[skill_index].AddSubskill(subskill_name);
    }

    public void SetSkillLevel(string skill_name, byte level)
    {
        byte skill_index = getSkillIndex(skill_name);

        Skills[skill_index].SetLevel(level);
    }

    public void SetSkillLevel(byte skill_index, byte level)
    {
        Skills[skill_index].SetLevel(level);
    }

    public void SetSubskillLevel(string subskill_name, byte level)
    {
        byte[] indexes = getSubskillIndex(subskill_name);

        if(Skills[indexes[0]].Subskills != null)
        {
            Skills[indexes[0]].Subskills?.ElementAt(indexes[1]).SetLevel(level);
        }

        
    }

    public void SetSubskillLevel(byte skill_index, byte subskill_index, byte level)
    {

        if(Skills[skill_index].Subskills != null)
        {
            Skills[skill_index].Subskills?.ElementAt(subskill_index).SetLevel(level);
        }

        
    }

    
}