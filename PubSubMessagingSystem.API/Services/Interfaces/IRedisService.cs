using StackExchange.Redis;

namespace PubSubMessagingSystem.API.Services.Interfaces
{
    public interface IRedisService
    {
        IDatabase GetDatabase();
        ISubscriber GetSubscriber();
        Task<bool> KeyExistsAsync(string key);
        Task<bool> HashSetAsync(string key, string hashField, string value);
        Task<string> HashGetAsync(string key, string hashField);
        Task<HashEntry[]> HashGetAllAsync(string key);
        Task<bool> HashDeleteAsync(string key, string hashField);
        Task<bool> KeyDeleteAsync(string key);
        Task<long> ListLeftPushAsync(string key, string value);
        Task<string> ListRightPopAsync(string key);
        Task<string[]> ListRangeAsync(string key, long start = 0, long stop = -1);
        Task<long> ListLengthAsync(string key);
        Task PublishAsync(string channel, string message);
        void Subscribe(string channel, Action<RedisChannel, RedisValue> handler);
    }
}