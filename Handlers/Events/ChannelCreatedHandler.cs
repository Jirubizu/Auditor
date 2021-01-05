using System.Threading.Tasks;
using Discord.WebSocket;

namespace Auditor.Handlers.Events
{
    public class ChannelCreatedHandler : EventHandler
    {
        public ChannelCreatedHandler(BaseSocketClient shard)
        {
            shard.ChannelCreated += ShardOnChannelCreated;
        }

        private Task ShardOnChannelCreated(SocketChannel arg)
        {
            SocketGuildChannel channel = (SocketGuildChannel) arg;
            
            throw new System.NotImplementedException();
        }
    }
}