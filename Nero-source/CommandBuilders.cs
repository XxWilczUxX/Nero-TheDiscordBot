using Discord;

namespace Nero
{

    public class CommandBuilders
    {

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
        ;

        public readonly SlashCommandBuilder Character = new SlashCommandBuilder()
            .WithName("character")
            .WithDescription("Character edition command tree")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("create")
                .WithDescription("Starts a character creation procedure.")
                .WithType(ApplicationCommandOptionType.SubCommand)
            )
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("edit")
                .WithDescription("Allows to edit/delete characters created by user.")
                .WithType(ApplicationCommandOptionType.SubCommand)
            )
        ;
        
        public readonly SlashCommandBuilder Roll = new SlashCommandBuilder()
            .WithName("roll")
            .WithDescription("Rolls a dice with given number of sides")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("sides")
                .WithDescription("type in number of sides")
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.Integer)
            )
        ;

        public readonly SlashCommandBuilder Network = new SlashCommandBuilder()
            .WithName("network")
            .WithDescription("Network command tree")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("create")
                .WithDescription("Starts a network creation procedure.")
                .WithType(ApplicationCommandOptionType.SubCommand)
            )
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("delete")
                .WithDescription("Deletes a network.")
                .WithType(ApplicationCommandOptionType.SubCommand)
            )
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("list")
                .WithDescription("Lists out networks in the guild.")
                .WithType(ApplicationCommandOptionType.SubCommand)
            )
        ;


    }

}