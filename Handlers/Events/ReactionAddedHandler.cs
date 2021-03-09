using System.Threading.Tasks;
using Auditor.Services;
using Discord;
using Discord.WebSocket;
using Serilog;

namespace Auditor.Handlers.Events
{
    public class ReactionAddedHandler: EventHandler
    {
private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<ReactionAddedHandler>();

        public ReactionAddedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.ReactionAdded += ShardOnReactionAdded;
            logger.Information("Registered");
        }

        private Task ShardOnReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel textChannel, SocketReaction reaction)
        {
            throw new System.NotImplementedException();
        }
    }
}