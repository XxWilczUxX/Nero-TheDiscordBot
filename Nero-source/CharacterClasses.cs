using Newtonsoft.Json;

namespace Nero
{

    public class Character
    {
        public string Name;
        public string Description;
        public int Role {get; set;}
        public int[] Stats {get; set;} = new int[10];
        public Skill[] Skills {get; set;} = new Skill[66];
        public int[] DistrPoints {get; set;} = new int[] {42, 58};
        
        public Character(string name, string description)
        {
            Names names = new Names();
            this.Name = name;
            this.Description = description;
            for(int i = 0; i < Stats.Length; i++)
            {
                this.Stats[i] = 2;
            }
            
            int stat;
            int cost;
            int level;
            bool hasLevel;
            Skill? subskill = null;

            for(int i = 1; i < Skills.Length; i++)
            {
                stat = 0;
                cost = 1;
                hasLevel = true;
                level = 0;
                subskill = null;
                if(i == 8 || i == 10 || i == 11 || i == 32) // SW
                {
                    stat = 5;
                }
                else if(i == 53 || i == 55) // EMP
                {
                    stat = 4;
                }
                else if(i < 6 || (26 < i && i < 31)) // REF
                {
                    stat = 1;
                }
                else if((5 < i && i < 10) || (59 < i && i < 64)) // ZW
                {
                    stat = 2;
                }
                else if(i > 11 && i < 36) // INT
                {
                    stat = 0;
                }
                else if((35 < i && i < 51) || i == 66) // TECH
                {
                    stat = 3;
                }
                else if(i != 0) // CHA
                {
                    stat = 9;
                }
                if(i == 1 || i == 5 || i == 28 || i == 37 || i == 42 || i == 50 || i == 62) // Cost
                {
                    cost = 2;
                }

                if(i == 14 || i == 19 || i == 25 || i == 65) // Subskill skills
                {
                    hasLevel = false;
                }

                if(i == 6 || i == 9 || i == 14 || i == 9 || i == 19 || i == 25 || i == 26 || i == 32 || i == 33 || i == 48 || i == 53 || i == 55 || i == 56 || i == 60 || i == 63) // Primary level allocation or creation of primary subskill
                {
                    if(!hasLevel)
                    {
                        switch(i)
                        {
                            case 14: // Language
                                subskill = new Skill(names.subskills[0], 2);
                                break;
                            case 25: // Local Knowlege
                                subskill = new Skill(names.subskills[1], 2);
                                break;
                        }
                    }
                    else
                    {
                        level = 2;
                    }
                }
                
                this.Skills[i] = new Skill(names.skills[i-1], stat, cost, hasLevel, level);
                if(subskill != null)
                {
                    this.Skills[i].AddSubskill(subskill);
                }

            }

        }

        public void SetRole(int role, int level)
        {
            Names names = new Names();
            this.Role = role;
            this.Skills[0] = new Skill(names.roles[1,this.Role], level);
        }

        public void AddLevelStat(int stat, int num = 1)
        {
            if((num > 0 && this.DistrPoints[0] == 0) || (num < 0 && this.Stats[stat] == 2))
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                this.Stats[stat] += num;
                this.DistrPoints[0] -= num;
            }
        }

        public void AddLevelSkill(int skill, int num = 1)
        {
            if((num > 0 && this.DistrPoints[1] - this.Skills[skill].Cost < 0) || (num < 0 && this.Skills[skill].Level == 2 && ( skill == 6 || skill == 9 || skill == 14 || skill == 9 || skill == 19 || skill == 25 || skill == 26 || skill == 32 || skill == 33 || skill == 48 || skill == 53 || skill == 55 || skill == 56 || skill == 60 || skill == 63 )))
            {
                throw new ArgumentOutOfRangeException();
            }
            
            if(this.Skills[skill].Level != null)
            {
                this.Skills[skill].Level += num;
                int? cost = this.Skills[skill].Cost;
                if(cost != null)
                {
                    this.DistrPoints[1] -= num * (int)cost;
                }
            }
        }

        public void MinStat(int stat)
        {
            int? level = this.Stats[stat];
            this.DistrPoints[0] += (int)level- 2;
            this.Stats[stat] = 2;
        }

        public void MaxStat(int stat)
        {
            int num = 8;
            if(this.DistrPoints[0] < num - this.Stats[stat] )
            {
                num = this.DistrPoints[0];
            }
            int? level = this.Stats[stat];
            this.DistrPoints[0] -= num;
            this.Stats[stat] += num;
        }

        public void MinSkill(int skill)
        {
            if( skill == 6 || skill == 9 || skill == 14 || skill == 9 || skill == 19 || skill == 25 || skill == 26 || skill == 32 || skill == 33 || skill == 48 || skill == 53 || skill == 55 || skill == 56 || skill == 60 || skill == 63 )
            {
                int? cost = this.Skills[skill].Cost;
                int? level = this.Skills[skill].Level;
                if(cost != null && level != null)
                {
                    this.DistrPoints[1] += ((int)level- 2) * (int)cost;
                    this.Skills[skill].Level = 2;
                }
            }
            else
            {
                int? cost = this.Skills[skill].Cost;
                int? level = this.Skills[skill].Level;
                if(cost != null && level != null)
                {
                    this.DistrPoints[1] += (int)level * (int)cost;
                    this.Skills[skill].Level = 0;
                }
            }
        }

        public void MaxSkill(int skill)
        {
            int? cost = this.Skills[skill].Cost;
            int? level = this.Skills[skill].Level;
            if(cost != null && level != null)
            {
                int num;
                if((8 - (int)level) * (int)cost > this.DistrPoints[1])
                {                        
                    num = this.DistrPoints[1];    
                }   
                else
                {
                    num = 8;
                }
                this.DistrPoints[1] -= (num - (int)level) * (int)cost;
                this.Skills[skill].Level = num;         
            }
        }

    }

    public class Skill
    {
        public string Name {get;}
        public int? Level {get; set;}
        public int? Stat {get;}
        public int? Cost {get;}
        public List<Skill>? SubSkills {get; set;}

        public Skill(string name, int stat, int cost, bool hasLevel, int level = 0) // Primary Skill
        {
            this.Name = name;
            this.Stat = stat;
            if(hasLevel)
            {
                this.Cost = cost;
                this.Level = level;
            }
        }

        [JsonConstructor]
        public Skill(string Name, int? Level, int? Stat, int? Cost, List<Skill>? SubSkills)
        {
            this.Name = Name;
            this.Level = Level;
            this.Stat = Stat;
            this.Cost = Cost;
            this.SubSkills = SubSkills;
        }

        public Skill(string name, int level) // Subskill
        {
            this.Name = name;
            this.Level = level;
            this.Cost = 1;
        }
        

        public void AddSubskill(Skill subskill)
        {
            if(this.SubSkills == null)
            {
                this.SubSkills = new List<Skill>();
            }
            this.SubSkills.Add(subskill);
        }

    }

}