using System;
using Discord.Rest;
using Discord.WebSocket;

namespace Auditor.Handlers
{
    public class EventHandler
    {
        protected static bool GetRestTextChannel(DiscordShardedClient shard,ulong? channelId, out RestTextChannel textChannel)
        {
            if (channelId != null && channelId != 0 && shard.Rest.GetChannelAsync(channelId.GetValueOrDefault()).Result is RestTextChannel t)
            {
                textChannel = t;
                return true;
            }

            textChannel = null;
            return false;
        }
    }
}