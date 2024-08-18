using Discord;
using Discord.WebSocket;
using Microsoft.VisualBasic;

namespace Nero;

public class Embeds
{
    public EmbedBuilder Error(string errorMessage)
    {
        var embed = new EmbedBuilder()
            .WithTitle("Error")
            .WithDescription(errorMessage)
            .WithColor(Color.Red)
            .WithCurrentTimestamp();
        
        return embed;
    }

    public EmbedBuilder Info(string title, string infoMessage)
    {
        var embed = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(infoMessage)
            .WithColor(Color.Green)
            .WithCurrentTimestamp();
        
        return embed;
    }

    public EmbedBuilder DebugExecuted()
    {
        var embed = new EmbedBuilder()
            .WithTitle("Done")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();

        return embed;
    }

    public EmbedBuilder Rolls(int dice, int[] rolls)
    {
        var embed = new EmbedBuilder()
            .WithTitle($"Rolled D{dice}, {rolls.Length} times:\n\nSum: {rolls.Sum()} out of {dice*rolls.Length}")
            .WithDescription($"Breakdown of rolls:")
            .WithColor(Color.Orange);

        for(int i = 0; i < rolls.Length; i++) {

            var field = new EmbedFieldBuilder()
                .WithName($"Roll no. {i+1}")
                .WithValue(rolls[i])
                .WithIsInline(false);

            embed.AddField(field);
        }

        return embed;
    }

    public EmbedBuilder Log(List<Data.Log> log, int page = 1) 
    {
        var embed = new EmbedBuilder()
            .WithTitle($"Logs")
            .WithDescription($"Page {page+1} of {log.Count/25+1}")
            .WithColor(Color.Gold)
            .WithCurrentTimestamp();

        for(int i = 25 * page; i < 25 * (1+page); i++) {

            if(i >= log.Count) {
                break;
            }

            var entry = log[i];

            var field = new EmbedFieldBuilder()
                .WithName($"Log entry no. {i+1}")
                .WithValue(entry.LogMessage)
                .WithIsInline(false);

            embed.AddField(field);
        }

        return embed;
    }

}