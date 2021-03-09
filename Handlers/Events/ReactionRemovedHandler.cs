using System.Threading.Tasks;
using Auditor.Services;
using Discord;
using Discord.WebSocket;
using Serilog;

namespace Auditor.Handlers.Events
{
    public class ReactionRemovedHandler: EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<ReactionRemovedHandler>();

        public ReactionRemovedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.ReactionRemoved += ShardOnReactionRemoved;
            logger.Information("Registered");
        }

        private Task ShardOnReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel textChannel, SocketReaction reaction)
        {
            throw new System.NotImplementedException();
        }
    }
}