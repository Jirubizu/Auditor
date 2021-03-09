using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Auditor.Structures;
using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Auditor.Services
{
    public class DatabaseService
    {
        private readonly IMongoDatabase mongoDatabase;

        private readonly Dictionary<ulong, GuildBson> auditorCache = new();

        public DatabaseService(DiscordShardedClient shardedClient)
        {
            MongoClient client = new("mongodb://localhost:27017");
            mongoDatabase = client.GetDatabase("auditor");

            shardedClient.JoinedGuild += OnJoinedGuild;
            shardedClient.LeftGuild += OnLeftGuild;
        }

        // Loading Records
        public async Task<List<GuildBson>> LoadRecords()
        {
            var collection = mongoDatabase.GetCollection<GuildBson>("guilds");
            return await collection.Find(new BsonDocument()).ToListAsync();
        }


        public async Task<GuildBson> LoadRecordsByGuildId(ulong guildId)
        {
            if (auditorCache.TryGetValue(guildId, out GuildBson cacheGuild))
            {
                return cacheGuild;
            }

            var collection = mongoDatabase.GetCollection<GuildBson>("guilds");
            var filter = Builders<GuildBson>.Filter.Eq("guild_id", guildId);

            GuildBson guild;

            try
            {
                guild = await collection.Find(filter).FirstAsync();
            }
            catch
            {
                guild = new GuildBson {GuildId = guildId};
                await InsertRecord("guilds", guild);
            }

            auditorCache.Add(guild.GuildId, guild);
            return guild;
        }

        // Inserting Records
        public async Task InsertRecord(string table, GuildBson record)
        {
            auditorCache.Add(record.GuildId, record);
            var collection = mongoDatabase.GetCollection<GuildBson>(table);
            await collection.InsertOneAsync(record);
        }

        // Updating Records

        public async Task UpdateGuild(GuildBson record)
        {
            auditorCache[record.GuildId] = record;
            var collection = mongoDatabase.GetCollection<GuildBson>("guilds");
            var filter = Builders<GuildBson>.Filter.Eq("guild_id", record.GuildId);
            await collection.ReplaceOneAsync(filter, record);
        }

        // Delete Records

        private async Task DeleteGuildRecord(ulong guildId)
        {
            auditorCache.Remove(guildId);
            var collection = mongoDatabase.GetCollection<GuildBson>("guilds");
            var filter = Builders<GuildBson>.Filter.Eq("guild_id", guildId);
            await collection.DeleteOneAsync(filter);
        }

        // Events

        private async Task OnJoinedGuild(SocketGuild arg)
        {
            await InsertRecord("guilds", new GuildBson
            {
                GuildId = arg.Id
            });
        }

        private async Task OnLeftGuild(SocketGuild arg)
        {
            await DeleteGuildRecord(arg.Id);
        }
    }
}