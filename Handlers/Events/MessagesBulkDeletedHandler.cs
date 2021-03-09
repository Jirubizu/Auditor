using System.Collections.Generic;
using System.Threading.Tasks;
using Auditor.Services;
using Discord;
using Discord.WebSocket;
using Serilog;

namespace Auditor.Handlers.Events
{
    public class MessagesBulkDeletedHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<MessagesBulkDeletedHandler>();

        public MessagesBulkDeletedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.MessagesBulkDeleted += ShardOnMessagesBulkDeleted;
            logger.Information("Registered");
        }

        private Task ShardOnMessagesBulkDeleted(IReadOnlyCollection<Cacheable<IMessage, ulong>> messages, ISocketMessageChannel textChannel)
        {
            throw new System.NotImplementedException();
        }
    }
}