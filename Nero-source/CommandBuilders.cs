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


    }

}