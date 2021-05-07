using System.Collections.Generic;
using Auditor.Utilities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Auditor.Structures
{
    public class GuildBson
    {
        [BsonId]
        public ObjectId CollectionId { get; set; }

        [BsonElement("guild_id")]
        public ulong GuildId { get; set; }

        [BsonElement("prefix"),JsonProperty("prefix")]
        public string Prefix { get; set; } = "a.";

        [BsonElement("category_id"),JsonProperty("category_id")] 
        public ulong? CategoryId { get; set; }

        [BsonElement("channel_created_event"),JsonProperty("channel_created_event")] 
        public ulong? ChannelCreatedEvent { get; set; }
        
        [BsonElement("channel_destroyed_event"),JsonProperty("channel_destroyed_event")] 
        public ulong? ChannelDestroyedEvent { get; set; }
        
        [BsonElement("channel_updated_event"),JsonProperty("channel_updated_event")] 
        public ulong? ChannelUpdatedEvent { get; set; }
        
        [BsonElement("guild_member_updated_event"),JsonProperty("guild_member_updated_event")] 
        public ulong? GuildMemberUpdatedEvent { get; set; }
        
        [BsonElement("guild_updated_event"),JsonProperty("guild_updated_event")] 
        public ulong? GuildUpdatedEvent { get; set; }
        
        [BsonElement("message_deleted_event"),JsonProperty("message_deleted_event")] 
        public ulong? MessageDeletedEvent { get; set; }
        
        [BsonElement("message_received_event"),JsonProperty("message_received_event")] 
        public ulong? MessageReceivedEvent { get; set; }
        
        [BsonElement("messages_bulk_deleted_event"),JsonProperty("messages_bulk_deleted_event")] 
        public ulong? MessagesBulkDeletedEvent { get; set; }
        
        [BsonElement("message_updated_event"),JsonProperty("message_updated_event")] 
        public ulong? MessageUpdatedEvent { get; set; }
        
        [BsonElement("reaction_added_event"),JsonProperty("reaction_added_event")] 
        public ulong? ReactionAddedEvent { get; set; }
        
        [BsonElement("reaction_removed_event"),JsonProperty("reaction_removed_event")] 
        public ulong? ReactionRemovedEvent { get; set; }
        
        [BsonElement("reactions_cleared_event"),JsonProperty("reactions_cleared_event")] 
        public ulong? ReactionsClearedEvent { get; set; }
        
        [BsonElement("role_created_event"),JsonProperty("role_created_event")] 
        public ulong? RoleCreatedEvent { get; set; }
        
        [BsonElement("role_deleted_event"),JsonProperty("role_deleted_event")] 
        public ulong? RoleDeletedEvent { get; set; }
        
        [BsonElement("role_updated_event"),JsonProperty("role_updated_event")] 
        public ulong? RoleUpdatedEvent { get; set; }
        
        [BsonElement("user_banned_event"),JsonProperty("user_banned_event")] 
        public ulong? UserBannedEvent { get; set; }
        
        [BsonElement("user_joined_event"),JsonProperty("user_joined_event")] 
        public ulong? UserJoinedEvent { get; set; }
        
        [BsonElement("user_left_event"),JsonProperty("user_left_event")] 
        public ulong? UserLeftEvent { get; set; }
        
        [BsonElement("user_unbanned_event"),JsonProperty("user_unbanned_event")] 
        public ulong? UserUnbannedEvent { get; set; }
        
        [BsonElement("user_updated_event"),JsonProperty("user_updated_event")] 
        public ulong? UserUpdatedEvent { get; set; }
        
        [BsonElement("user_voice_state_updated_event"),JsonProperty("user_voice_state_updated_event")] 
        public ulong? UserVoiceStateUpdatedEvent { get; set; }
    }
}