using System;
using System.Threading.Tasks;
using Auditor.Services;
using Auditor.Structures;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;

namespace Auditor.Handlers.Events
{
    public class MessageReceivedHandler : EventHandler
    {
        private readonly DatabaseService database;
        private readonly DiscordShardedClient shard;
        private readonly ILogger logger = Log.ForContext<MessageReceivedHandler>();

        public MessageReceivedHandler(DiscordShardedClient s, DatabaseService d)
        {
            this.database = d;
            this.shard = s;
            this.shard.MessageReceived += ShardOnMessageReceived;
            logger.Information("Registered");
        }

        private async Task ShardOnMessageReceived(SocketMessage message)
        {
            if (message.Source == MessageSource.Bot)
            {
                return;
            }
            
            GuildBson guild = await this.database.LoadRecordsByGuildId(((SocketTextChannel) message.Channel).Guild.Id);

            if (GetRestTextChannel(this.shard, guild.MessageReceivedEvent.Key, out RestTextChannel restTextChannel))
            {
                EmbedBuilder embedBuilder = new()
                {
                    Author = new EmbedAuthorBuilder{Name = message.Author.Mention, IconUrl = message.Author.GetAvatarUrl(), Url = message.GetJumpUrl()},
                    Title = $"Message Sent in #{message.Channel.Name} | {message.Channel.Id}",
                    Description = message.Content,
                    Color = Color.Blue,
                    Footer = new EmbedFooterBuilder{Text = $"User ID: {message.Author.Id}, Message ID: {message.Id}, sent at {DateTime.UtcNow} UTC"}
                };

                await restTextChannel.SendMessageAsync("", false, embedBuilder.Build());
            }
            
        }
    }
}