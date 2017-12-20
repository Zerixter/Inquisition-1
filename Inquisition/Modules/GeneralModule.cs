using Discord.Commands;
using System.Threading.Tasks;

namespace Inquisition.Modules
{
    public class GaneralModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Replies with Pong!")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong!");
        }
    }
}
