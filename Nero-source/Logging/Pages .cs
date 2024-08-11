using Discord;

namespace Nero.Logging;
class Pages
{
    public List<EmbedBuilder> CreateLogEmbeds(List<string> logEntries)
    {
        var embeds = new List<EmbedBuilder>();
        var totalLogs = logEntries.Count; // включает ли каждый embeedbuilder по 25 строк или сущностей
        var logsPerPage = 25;
        var totalPages = (int)Math.Ceiling((double)totalLogs / logsPerPage);
        
        for (int i = 0; i < totalPages; i++)
        {
            var embed = new EmbedBuilder()
                .WithTitle($"Logs - Page {i + 1}/{totalPages}")
                .WithColor(Color.Blue)
                .WithCurrentTimestamp();

            for (int j = 0; j < logsPerPage; j++)
            {
                int logIndex = i * logsPerPage + j;
                if (logIndex >= totalLogs) break;

                embed.AddField($"Log {logIndex + 1}", logEntries[logIndex]);
            }

            embeds.Add(embed);
        }

        return embeds;
    }
}