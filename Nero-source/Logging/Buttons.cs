using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nero.Logging;

public class Buttons
{
    public MessageComponent CreateNavigationButtons()
    {
        return new ComponentBuilder()
            .WithButton("Previous", "log_prev", ButtonStyle.Primary)
            .WithButton("Next", "log_next", ButtonStyle.Primary)
            .Build();
    }

    public async Task<int> HandleButtonInteraction(SocketMessageComponent component, List<EmbedBuilder> embeds, int currentPage)
    {
        switch (component.Data.CustomId)
        {
            case "log_prev":
                currentPage = (currentPage == 0) ? embeds.Count - 1 : currentPage - 1;
                break;
            case "log_next":
                currentPage = (currentPage + 1) % embeds.Count;
                break;
        }

        await component.UpdateAsync(msg =>
        {
            msg.Embed = embeds[currentPage].Build();
            msg.Components = CreateNavigationButtons();
        });

        return currentPage;
    }
}
