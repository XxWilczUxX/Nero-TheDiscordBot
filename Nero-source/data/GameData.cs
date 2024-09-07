

namespace Nero.Data.GameData;

public static class DefaultNames
{
    public static string[] Stats { get; } = 
    { 
        "Intelligence", "Reflex", "Agility", "Technology", "Charisma", "Will", "Luck", "Movement", "Body", "Empathy" 
    };

    public static string[,] Skills { get; } = 
    {
        {"Heavy Weapons", "Long Weapons", "Short Weapons", "Archery", "Continous Fire", "Athletics", "Rubber Man", "Torture/Narcotic Tolerancy", "Stealth", "Dance", "Endurance", "Bureaucracy", "Deduction", "Language", "Composing", "Criminology", "Cryptography", "Accounting", "Science", "Animal Care", "Library Searching", "Making Deals", "Art of Survival", "Tactics", "Local Knowlege", "Education", "Riding", "Piloting", "Car Driving", "Sailing", "Lip Reading", "Concentration", "Perception", "Tracking", "Hiding/Finding an Item", "Cyber Engineering", "Electronics and Security", "Falsification", "Photography", "Pickpocketing", "Art", "Explosives", "Weapon Repair", "Land Vehicle Repair", "Water Vehicle Repair", "Air Vehicle Repair", "Lockpicking", "First Aid", "Basic Repairing", "Paramedics", "Attractiveness", "Trading", "Conversation", "Fashion", "Emotional Inteligence", "Persuasion", "Bribery", "Interrogation", "Semi-Literate Knowlege", "Fighting", "Meele Weapons", "Martial Arts", "Dodgeing", "Acting", "Playing an Instrument"},
        {"", "", "", "", "", "", "", "", "", "", "", "", "", "Street Slang", "", "", "", "", "", "", "", "", "", "", "Your Area", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""}
    };

    public static string[] SkillTypes { get; } = 
    { 
        "Long Range Weapons", "Body", "Education", "Control", "Observation", "Technology", "Social", "Meele", "Performances" 
    };

    public static string[] Roles { get; } = 
    { 
        "Solo", "Netrunner", "Techie", "Media", "Cop", "Nomad", "Fixer", "Corporate", "Medtech", "Rockerboy / Rockergirl" 
    };

    public static string[] Abilities { get; } = 
    { 
        "Combat Sense", "Interface", "Jurry Rig", "Credibility", "Family", "Authority", "Connections", "Resources", "Medical Tech", "Charismatic Leadership" 
    };
}
