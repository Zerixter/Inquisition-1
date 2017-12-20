using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Inquisition.Handlers
{
    public class EventHandler
    {
        private DiscordSocketClient Client;

        public EventHandler(DiscordSocketClient client)
        {
            Client = client;
            Client.Log += Log;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
