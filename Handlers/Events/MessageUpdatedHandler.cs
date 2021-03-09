using System.Threading.Tasks;
using Auditor.Services;
using Discord;
using Discord.WebSocket;
using Serilog;

namespace Auditor.Handlers.Events
{
    public class MessageUpdatedHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<MessageUpdatedHandler>();

        public MessageUpdatedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.MessageUpdated += ShardOnMessageUpdated;
            logger.Information("Registered");
        }

        private Task ShardOnMessageUpdated(Cacheable<IMessage, ulong> oldMessage, SocketMessage newMessage, ISocketMessageChannel textChannel)
        {
            throw new System.NotImplementedException();
        }
    }
}