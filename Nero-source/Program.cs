﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Nero;

class Program
{
    static void Main(string[] args)
        => new Program().MainAsync(args).GetAwaiter().GetResult();

    private DiscordSocketClient _client = new DiscordSocketClient();
    private CommandService _commands = new CommandService();
    private Data.Info info = new Data.Info();
    public async Task MainAsync(string[] args)
    {

        Data.DataController dataController = new Data.DataController();
        dataController.CreateLocalFiles();

        _client = new DiscordSocketClient();
        _commands = new CommandService();

        _client.Log += Log;
        _client.Ready += Client_Ready;

        _client.ButtonExecuted += ButtonHandler;

        string Token = info.Token;

        if(Token == string.Empty)
        {
            Console.WriteLine("\nNo /safe/safe.json config file or token was unset.\n");
        }

        await _client.LoginAsync(TokenType.Bot, Token);
        await _client.StartAsync();

        await Task.Delay(-1);
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    public async Task Client_Ready()
    {
        var guild = _client.GetGuild(info.TestplaceID);

        var commandBuilders = new CommandBuilders();

        var guildCommand = commandBuilders.Session;

        try
        {
            await guild.CreateApplicationCommandAsync(guildCommand.Build());
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.Message);
        }

        _client.SlashCommandExecuted += SlashCommandHandler;
    }

    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        Console.WriteLine($"Executed: {command.Data.Name}, ID: {command.Data.Id} \n");

        switch(command.CommandName)
        {
            case "debug":
                var debug = new DebugCommands();

                await debug.CommandHandler(command, _client.GetGuild(info.TestplaceID), _client);
                return;
            case "roll":
                var rollCommand = new RollCommand().Roll;

                await rollCommand(command);
                return;
            case "session":
                var sessionCommandHandler = new SessionCommand().CommandHandler;

                await sessionCommandHandler(command);
                return;
            default:
                var embeds = new Embeds();

                await command.RespondAsync(embed: embeds.Error("Not implemented yet.").Build());
                return;
        }
    }

    private async Task ButtonHandler(SocketMessageComponent component)
    {

        var idParts = component.Data.CustomId.Split('-');

        switch(idParts[0])
        {
            case "log":
                var logCommands = new LogSubCommand();

                await logCommands.LogHandler(component, idParts[1], int.Parse(idParts[2]));
                return;
            default:
                var embeds = new Embeds();
                await component.RespondAsync(embed: embeds.Error("Not implemented yet.").Build());
                return;
        }
    }
}