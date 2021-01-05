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
        public bool ChannelCreatedEvent { get; set; } = false;
        
        [BsonElement("channel_destroyed_event")] 
        public bool ChannelDestroyedEvent { get; set; } = false;
        
        [BsonElement("channel_updated_event")] 
        public bool ChannelUpdatedEvent { get; set; } = false;
        
        [BsonElement("current_user_updated_event")] 
        public bool CurrentUserUpdatedEvent { get; set; } = false;
        
        [BsonElement("guild_available_event")] 
        public bool GuildAvailableEvent { get; set; } = false;
        
        [BsonElement("guild_members_downloaded_event")] 
        public bool GuildMembersDownloadedEvent { get; set; } = false;
        
        [BsonElement("guild_member_updated_event")] 
        public bool GuildMemberUpdatedEvent { get; set; } = false;
        
        [BsonElement("guild_unavailable_event")] 
        public bool GuildUnavailableEvent { get; set; } = false;
        
        [BsonElement("guild_updated_event")] 
        public bool GuildUpdatedEvent { get; set; } = false;
        
        [BsonElement("joined_guild_event")] 
        public bool JoinedGuildEvent { get; set; } = false;
        
        [BsonElement("left_guild_event")] 
        public bool LeftGuildEvent { get; set; } = false;
        
        [BsonElement("message_deleted_event")] 
        public bool MessageDeletedEvent { get; set; } = false;
        
        [BsonElement("message_received_event")] 
        public bool MessageReceivedEvent { get; set; } = false;
        
        [BsonElement("message_bulk_deleted_event")] 
        public bool MessageBulkDeletedEvent { get; set; } = false;
        
        [BsonElement("message_updated_event")] 
        public bool MessageUpdatedEvent { get; set; } = false;
        
        [BsonElement("reaction_added_event")] 
        public bool ReactionAddedEvent { get; set; } = false;
        
        [BsonElement("reaction_removed_event")] 
        public bool ReactionRemovedEvent { get; set; } = false;
        
        [BsonElement("reaction_cleared_event")] 
        public bool ReactionClearedEvent { get; set; } = false;
        
        [BsonElement("recipient_added_event")] 
        public bool RecipientAddedEvent { get; set; } = false;
        
        [BsonElement("recipient_removed_event")] 
        public bool RecipientRemovedEvent { get; set; } = false;
        
        [BsonElement("role_created_event")] 
        public bool RoleCreatedEvent { get; set; } = false;
        
        [BsonElement("role_deleted_event")] 
        public bool RoleDeletedEvent { get; set; } = false;
        
        [BsonElement("role_updated_event")] 
        public bool RoleUpdatedEvent { get; set; } = false;
        
        [BsonElement("user_banned_event")] 
        public bool UserBannedEvent { get; set; } = false;
        
        [BsonElement("user_is_typing_event")] 
        public bool UserIsTypingEvent { get; set; } = false;
        
        [BsonElement("user_joined_event")] 
        public bool UserJoinedEvent { get; set; } = false;
        
        [BsonElement("user_left_event")] 
        public bool UserLeftEvent { get; set; } = false;
        
        [BsonElement("user_unbanned_event")] 
        public bool UserUnbannedEvent { get; set; } = false;
        
        [BsonElement("user_updated_event")] 
        public bool UserUpdatedEvent { get; set; } = false;
        
        [BsonElement("user_voice_state_updated_event")] 
        public bool UserVoiceStateUpdatedEvent { get; set; } = false;
        
        [BsonElement("voice_server_updated_event")] 
        public bool VoiceServerUpdatedEvent { get; set; } = false;
        
        
        
    }
}