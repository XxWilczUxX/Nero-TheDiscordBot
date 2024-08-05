using Discord;

namespace Nero
{

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
            )
        ;

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
            )
        ;

        

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

            )
        ;

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
        ;


    }

}