using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Inquisition.Handlers
{
    public class CommandHandler
    {
        private DiscordSocketClient Client;
        private CommandService Commands;
        private IServiceProvider Services;

        public CommandHandler(DiscordSocketClient c)
        {
            Client = c;

            Commands = new CommandService();

            Services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .BuildServiceProvider();

            Commands.AddModulesAsync(Assembly.GetEntryAssembly());

            Client.MessageReceived += HandleCommands;
        }

        private async Task HandleCommands(SocketMessage msg)
        {
            var message = msg as SocketUserMessage;

            if (message is null || message.Author.IsBot)
                return;

            string prefix = "rip ";

            int argPos = 0;

            if (message.HasMentionPrefix(Client.CurrentUser, ref argPos) ||
                message.HasStringPrefix(prefix, ref argPos) ||
                message.Channel.GetType() == typeof(SocketDMChannel))
            {
                SocketCommandContext context = new SocketCommandContext(Client, message);
                IResult result = await Commands.ExecuteAsync(context, argPos, Services);

                if (!result.IsSuccess)
                {
                    Console.WriteLine($"{DateTimeOffset.UtcNow} - {result.ErrorReason}");
                }
            }
        }
    }
}
