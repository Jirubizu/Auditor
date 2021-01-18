using System.Threading.Tasks;
using Auditor.Services;
using Discord.WebSocket;
using Serilog;

namespace Auditor.Handlers.Events
{
    public class CurrentUserUpdatedHandler : IEventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.Logger.ForContext<ChannelUpdatedHandler>();

        public CurrentUserUpdatedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.shard = s;
            this.database = d;
            this.shard.CurrentUserUpdated += ShardOnCurrentUserUpdated;
        }

        private Task ShardOnCurrentUserUpdated(SocketSelfUser arg1, SocketSelfUser arg2)
        {
            logger.Debug("Current user updated");
            return Task.CompletedTask;
        }
    }
}