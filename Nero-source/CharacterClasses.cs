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
        public int[] DistrPoints {get; set;} = new int[] {42, 58}; // stat , skill
        
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

        private int DistrPtsChecks(int level, int num, int[] range, int distrPoints, int cost = 1)
        {
            if(distrPoints < (num < 0 ? (level + num >= range[0] ? num : range[0] - level) : (level + num <= range[1] ? num : range[1] - level)) * cost) // distribution poits range logic
            {
                num = distrPoints / cost;
            }
            else if(num < 0 && level + num < range[0]) // range logic
            {
                num = range[0] - level;
            }
            else if(num > 0 && level + num > range[1]) // range logic
            {
                num = range[1] - level;
            }
            return num;
        }

        public void DistributePoints(string type, int[] pos, int num = 1, int[]? range = null)
        {
            if(range == null)
            {
                range = new int[] {0,int.MaxValue};
            }
            switch(type)
            {

                case "stat":
                    num = DistrPtsChecks(this.Stats[pos[0]], num, range, this.DistrPoints[0]);
                    this.Stats[pos[0]] += num;
                    this.DistrPoints[0] -= num;
                    break;

                case "skill":
                    if (pos[1] > 0)
                    {
                        Skill? subSkill = this.Skills[pos[0]].SubSkills[pos[1]-1];
                        if (subSkill != null)
                        {
                            num = DistrPtsChecks(subSkill.Level ?? 0, num, range, this.DistrPoints[1]);
                            this.Skills[pos[0]].SubSkills[pos[1]-1].Level += num;
                            this.DistrPoints[1] -= num * this.Skills[pos[0]].SubSkills[pos[1]-1].Cost ?? 1;
                        }
                        else
                        {
                            break;
                        }
                        
                    } else
                    {
                        num = DistrPtsChecks(this.Skills[pos[0]].Level ?? 0, num, range, this.DistrPoints[1], this.Skills[pos[0]].Cost ?? 1);
                        this.Skills[pos[0]].Level += num;
                        this.DistrPoints[1] -= num * this.Skills[pos[0]].Cost ?? 1;
                    }
                    break;

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