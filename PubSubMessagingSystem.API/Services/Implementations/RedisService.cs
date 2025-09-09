using Microsoft.Extensions.Options;
using PubSubMessagingSystem.API.Services.Interfaces;
using StackExchange.Redis;

namespace PubSubMessagingSystem.API.Services.Implementations
{
    public class RedisService : IRedisService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly ISubscriber _subscriber;

        public RedisService(IOptions<RedisConfiguration> redisConfig)
        {
            var configurationOptions = ConfigurationOptions.Parse(redisConfig.Value.ConnectionString);
            _redis = ConnectionMultiplexer.Connect(configurationOptions);
            _database = _redis.GetDatabase();
            _subscriber = _redis.GetSubscriber();
        }

        public IDatabase GetDatabase() => _database;
        public ISubscriber GetSubscriber() => _subscriber;

        public async Task<bool> KeyExistsAsync(string key) => await _database.KeyExistsAsync(key);
        public async Task<bool> HashSetAsync(string key, string hashField, string value) =>
            await _database.HashSetAsync(key, hashField, value);
        public async Task<string> HashGetAsync(string key, string hashField) =>
            (await _database.HashGetAsync(key, hashField)).ToString();
        public async Task<HashEntry[]> HashGetAllAsync(string key) =>
            await _database.HashGetAllAsync(key);
        public async Task<bool> HashDeleteAsync(string key, string hashField) =>
            await _database.HashDeleteAsync(key, hashField);
        public async Task<bool> KeyDeleteAsync(string key) =>
            await _database.KeyDeleteAsync(key);
        public async Task<long> ListLeftPushAsync(string key, string value) =>
            await _database.ListLeftPushAsync(key, value);
        public async Task<string> ListRightPopAsync(string key) =>
            (await _database.ListRightPopAsync(key)).ToString();
        public async Task<string[]> ListRangeAsync(string key, long start = 0, long stop = -1) =>
            (await _database.ListRangeAsync(key, start, stop)).Select(x => x.ToString()).ToArray();
        public async Task<long> ListLengthAsync(string key) =>
            await _database.ListLengthAsync(key);
        public async Task PublishAsync(string channel, string message) =>
            await _subscriber.PublishAsync(channel, message);
        public void Subscribe(string channel, Action<RedisChannel, RedisValue> handler) =>
            _subscriber.Subscribe(channel, handler);
    }

    public class RedisConfiguration
    {
        public string ConnectionString { get; set; } = "localhost:6379";
    }
}