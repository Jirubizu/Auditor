using Auditor.Attributes.Preconditions;

namespace Auditor.Enumerators
{
    [ParameterEnum]
    public enum Events
    {
        ChannelCreated,
        ChannelDestroyed,
        ChannelUpdated,
        MessageReceived,
        MessageDeleted,
        MessagesBulkDeleted,
        MessageUpdated,
        ReactionAdded,
        ReactionRemoved,
        ReactionsCleared,
        RoleCreated,
        RoleDeleted,
        RoleUpdated,
        JoinedGuild,
        LeftGuild,
        GuildAvailable,
        GuildUnavailable,
        GuildMembersDownloaded,
        GuildUpdated,
        UserJoined,
        UserLeft,
        UserBanned,
        UserUnbanned,
        UserUpdated,
        GuildMemberUpdated,
        UserVoiceStateUpdated,
        VoiceServerUpdated,
        CurrentUserUpdated,
        UserIsTyping,
        RecipientAdded,
        RecipientRemoved
    }
}