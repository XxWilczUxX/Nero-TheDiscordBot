using Discord;
using Nero.Data.GameData;

namespace Nero;
public class CommandBuilders
{

    /* Command Builder Template:

    public readonly SlashCommandBuilder Template = new SlashCommandBuilder()
        .WithName("name")
        .WithDescription("description")
        .WithDefaultMemberPermissions(GuildPermission.Permission)
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("option name")
            .WithDescription("option description")
            .WithType(ApplicationCommandOptionType.OptionType)
        );

    */

    public readonly SlashCommandBuilder Debug = new SlashCommandBuilder()
        .WithName("debug")
        .WithDescription("debug commands that are only for impactfull management of the bot")
        .WithDefaultMemberPermissions(GuildPermission.Administrator)
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("list")
            .WithDescription("Gives a list of all commands")
            .WithType(ApplicationCommandOptionType.SubCommand)
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("type")
                .WithDescription("Choose a type")
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.Integer)
                .AddChoice("guild", 0)
                .AddChoice("global", 1)
            )
        )
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("delete")
            .WithDescription("Deletes all commands of given type")
            .WithType(ApplicationCommandOptionType.SubCommand)
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("type")
                .WithDescription("Choose a type")
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.Integer)
                .AddChoice("guild", 0)
                .AddChoice("global", 1)
            )
        )
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("shutdown")
            .WithDescription("Shuts down the bot, can only be used by set user")
            .WithType(ApplicationCommandOptionType.SubCommand)
        );

    public readonly SlashCommandBuilder Roll = new SlashCommandBuilder()
        .WithName("roll")
        .WithDescription("Roll a dice")
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("sides")
            .WithDescription("State a number of sides of the dice")
            .WithType(ApplicationCommandOptionType.Integer)
            .WithMinValue(2)
            .WithRequired(true)

        )
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("times")
            .WithDescription("State a number of dices to roll (max 25)")
            .WithType(ApplicationCommandOptionType.Integer)
            .WithRequired(false)
            .WithMinValue(1)

        );

    public readonly SlashCommandBuilder Session = new SlashCommandBuilder()
        .WithName("session")
        .WithDescription("Command family for creation and management of sessions")
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("create")
            .WithDescription("Creates a new game session")
            .WithType(ApplicationCommandOptionType.SubCommand)
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("name")
                .WithDescription("choose the session characteristical name.")
                .WithType(ApplicationCommandOptionType.String)
                .WithRequired(true)
            )
        )
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("log")
            .WithDescription("Creates a log entry in the session")
            .WithType(ApplicationCommandOptionType.SubCommandGroup)
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("create")
                .WithDescription("Creates a new log entry")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("text")
                    .WithDescription("The log description")
                    .WithType(ApplicationCommandOptionType.String)
                    .WithRequired(true)
                )
            )
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("read")
                .WithDescription("Reads the log entries")
                .WithType(ApplicationCommandOptionType.SubCommand)
            )
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("delete")
                .WithDescription("Deletes a log entry at given number")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("index")
                    .WithDescription("The log number")
                    .WithType(ApplicationCommandOptionType.Integer)
                    .WithRequired(true)
                )
            )
        );

    public readonly SlashCommandBuilder Character = new SlashCommandBuilder()
        .WithName("character")
        .WithDescription("Command family for creation and management of characters")
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("create")
            .WithDescription("Creates a new character")
            .WithType(ApplicationCommandOptionType.SubCommand)
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("name")
                .WithDescription("choose the character name.")
                .WithType(ApplicationCommandOptionType.String)
                .WithRequired(true)
            )
        )
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("delete")
            .WithDescription("Deletes a character")
            .WithType(ApplicationCommandOptionType.SubCommand)
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("name")
                .WithDescription("choose the character name.")
                .WithType(ApplicationCommandOptionType.String)
                .WithRequired(true)
            )
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("role")
                .WithDescription("choose the character role.")
                .AddChoice(DefaultNames.Roles[0], 0)
                .AddChoice(DefaultNames.Roles[1], 1)
                .AddChoice(DefaultNames.Roles[2], 2)
                .AddChoice(DefaultNames.Roles[3], 3)
                .AddChoice(DefaultNames.Roles[4], 4)
                .AddChoice(DefaultNames.Roles[5], 5)
                .AddChoice(DefaultNames.Roles[6], 6)
                .AddChoice(DefaultNames.Roles[7], 7)
                .AddChoice(DefaultNames.Roles[8], 8)
                .AddChoice(DefaultNames.Roles[9], 9)
                .WithType(ApplicationCommandOptionType.Integer)
            )
        )
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("list")
            .WithDescription("Lists all characters")
            .WithType(ApplicationCommandOptionType.SubCommand)
        );
        
}