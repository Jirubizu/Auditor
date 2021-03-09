using System.Threading.Tasks;
using Auditor.Services;
using Discord;
using Discord.WebSocket;
using Serilog;

namespace Auditor.Handlers.Events
{
    public class ReactionsClearedHandler: EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<RoleDeletedHandler>();

        public ReactionsClearedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.ReactionsCleared += ShardOnReactionsCleared;
            logger.Information("Registered");
        }

        private Task ShardOnReactionsCleared(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel textChannel)
        {
            throw new System.NotImplementedException();
        }
    }
}