using Discord;
using Discord.WebSocket;
using Inquisition.Handlers;
using Inquisition.Properties;
using System.Threading.Tasks;

namespace Inquisition
{
    class Program
    {
        private DiscordSocketClient Client;
        private CommandHandler CommandHandler;
        private EventHandler EventHandler;
        private string Token = Resources.Token;

        static void Main(string[] args) 
            => new Program().Run().GetAwaiter().GetResult();

        public async Task Run()
        {
            Client = new DiscordSocketClient();
            CommandHandler = new CommandHandler(Client);
            EventHandler = new EventHandler(Client);

            await Client.LoginAsync(TokenType.Bot, Token);
            await Client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
