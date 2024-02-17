using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord.Interactions;
using Discord.Net;

namespace Nero
{

    public class Commands
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
                    .WithDescription("Starts Character Creation Process")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                )
            ;
        
    }

}