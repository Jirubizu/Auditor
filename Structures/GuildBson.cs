using System.Collections.Generic;
using Auditor.Utilities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Auditor.Structures
{
    public class GuildBson
    {
        [BsonId]
        public ObjectId CollectionId { get; set; }

        [BsonElement("guild_id")]
        public ulong GuildId { get; set; }

        [BsonElement("prefix")] 
        public string Prefix { get; set; } = "a.";
        
        [BsonElement("category_id")]
        public ulong CategoryId { get; set; }

        [BsonElement("channel_created_event")] 
        public MutableKeyValuePair<ulong?, bool> ChannelCreatedEvent { get; set; } = new (null, false);
        
        [BsonElement("channel_destroyed_event")] 
        public MutableKeyValuePair<ulong?, bool> ChannelDestroyedEvent { get; set; } = new (null, false);
        
        [BsonElement("channel_updated_event")] 
        public MutableKeyValuePair<ulong?, bool> ChannelUpdatedEvent { get; set; } = new (null, false);
        
        [BsonElement("current_user_updated_event")] 
        public MutableKeyValuePair<ulong?, bool> CurrentUserUpdatedEvent { get; set; } = new (null, false);
        
        [BsonElement("guild_available_event")] 
        public MutableKeyValuePair<ulong?, bool> GuildAvailableEvent { get; set; } = new (null, false);
        
        [BsonElement("guild_members_downloaded_event")] 
        public MutableKeyValuePair<ulong?, bool> GuildMembersDownloadedEvent { get; set; } = new (null, false);
        
        [BsonElement("guild_member_updated_event")] 
        public MutableKeyValuePair<ulong?, bool> GuildMemberUpdatedEvent { get; set; } = new (null, false);
        
        [BsonElement("guild_unavailable_event")] 
        public MutableKeyValuePair<ulong?, bool> GuildUnavailableEvent { get; set; } = new (null, false);
        
        [BsonElement("guild_updated_event")] 
        public MutableKeyValuePair<ulong?, bool> GuildUpdatedEvent { get; set; } = new (null, false);
        
        [BsonElement("joined_guild_event")] 
        public MutableKeyValuePair<ulong?, bool> JoinedGuildEvent { get; set; } = new (null, false);
        
        [BsonElement("left_guild_event")] 
        public MutableKeyValuePair<ulong?, bool> LeftGuildEvent { get; set; } = new (null, false);
        
        [BsonElement("message_deleted_event")] 
        public MutableKeyValuePair<ulong?, bool> MessageDeletedEvent { get; set; } = new (null, false);
        
        [BsonElement("message_received_event")] 
        public MutableKeyValuePair<ulong?, bool> MessageReceivedEvent { get; set; } = new (null, false);
        
        [BsonElement("message_bulk_deleted_event")] 
        public MutableKeyValuePair<ulong?, bool> MessageBulkDeletedEvent { get; set; } = new (null, false);
        
        [BsonElement("message_updated_event")] 
        public MutableKeyValuePair<ulong?, bool> MessageUpdatedEvent { get; set; } = new (null, false);
        
        [BsonElement("reaction_added_event")] 
        public MutableKeyValuePair<ulong?, bool> ReactionAddedEvent { get; set; } = new (null, false);
        
        [BsonElement("reaction_removed_event")] 
        public MutableKeyValuePair<ulong?, bool> ReactionRemovedEvent { get; set; } = new (null, false);
        
        [BsonElement("reaction_cleared_event")] 
        public MutableKeyValuePair<ulong?, bool> ReactionClearedEvent { get; set; } = new (null, false);
        
        [BsonElement("recipient_added_event")] 
        public MutableKeyValuePair<ulong?, bool> RecipientAddedEvent { get; set; } = new (null, false);
        
        [BsonElement("recipient_removed_event")] 
        public MutableKeyValuePair<ulong?, bool> RecipientRemovedEvent { get; set; } = new (null, false);
        
        [BsonElement("role_created_event")] 
        public MutableKeyValuePair<ulong?, bool> RoleCreatedEvent { get; set; } = new (null, false);
        
        [BsonElement("role_deleted_event")] 
        public MutableKeyValuePair<ulong?, bool> RoleDeletedEvent { get; set; } = new (null, false);
        
        [BsonElement("role_updated_event")] 
        public MutableKeyValuePair<ulong?, bool> RoleUpdatedEvent { get; set; } = new (null, false);
        
        [BsonElement("user_banned_event")] 
        public MutableKeyValuePair<ulong?, bool> UserBannedEvent { get; set; } = new (null, false);
        
        [BsonElement("user_is_typing_event")] 
        public MutableKeyValuePair<ulong?, bool> UserIsTypingEvent { get; set; } = new (null, false);
        
        [BsonElement("user_joined_event")] 
        public MutableKeyValuePair<ulong?, bool> UserJoinedEvent { get; set; } = new (null, false);
        
        [BsonElement("user_left_event")] 
        public MutableKeyValuePair<ulong?, bool> UserLeftEvent { get; set; } = new (null, false);
        
        [BsonElement("user_unbanned_event")] 
        public MutableKeyValuePair<ulong?, bool> UserUnbannedEvent { get; set; } = new (null, false);
        
        [BsonElement("user_updated_event")] 
        public MutableKeyValuePair<ulong?, bool> UserUpdatedEvent { get; set; } = new (null, false);
        
        [BsonElement("user_voice_state_updated_event")] 
        public MutableKeyValuePair<ulong?, bool> UserVoiceStateUpdatedEvent { get; set; } = new (null, false);
        
        [BsonElement("voice_server_updated_event")] 
        public MutableKeyValuePair<ulong?, bool> VoiceServerUpdatedEvent { get; set; } = new (null, false);
        
        
        
    }
}