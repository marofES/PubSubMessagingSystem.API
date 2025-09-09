using PubSubMessagingSystem.API.Models;
using PubSubMessagingSystem.API.Models.DTOs;
using PubSubMessagingSystem.API.Services.Interfaces;
using System.Text.Json;

namespace PubSubMessagingSystem.API.Services.Implementations
{
    public class TopicService : ITopicService
    {
        private readonly IRedisService _redisService;
        private const string TopicsKey = "topics";
        private const string TopicPrefix = "topic:";

        public TopicService(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task<Topic> CreateTopicAsync(CreateTopicRequest request)
        {
            var topic = new Topic { Name = request.Name };

            // Store topic details
            await _redisService.HashSetAsync(
                $"{TopicPrefix}{topic.Id}",
                "data",
                JsonSerializer.Serialize(topic));

            // Add to topics set
            await _redisService.HashSetAsync(TopicsKey, topic.Id, topic.Name);

            return topic;
        }

        public async Task<bool> DeleteTopicAsync(string topicId)
        {
            // Check if topic exists
            if (!await _redisService.KeyExistsAsync($"{TopicPrefix}{topicId}"))
                return false;

            // Remove from topics set
            await _redisService.HashDeleteAsync(TopicsKey, topicId);

            // Delete topic data
            await _redisService.KeyDeleteAsync($"{TopicPrefix}{topicId}");

            // Delete messages for this topic
            await _redisService.KeyDeleteAsync($"messages:{topicId}");

            // Delete subscriptions for this topic
            await _redisService.KeyDeleteAsync($"subscriptions:{topicId}");

            return true;
        }

        public async Task<Topic?> GetTopicAsync(string topicId)
        {
            var topicData = await _redisService.HashGetAsync($"{TopicPrefix}{topicId}", "data");
            return string.IsNullOrEmpty(topicData) ? null : JsonSerializer.Deserialize<Topic>(topicData);
        }

        public async Task<IEnumerable<Topic>> GetAllTopicsAsync()
        {
            var topics = new List<Topic>();
            var allTopics = await _redisService.HashGetAllAsync(TopicsKey);

            foreach (var topic in allTopics)
            {
                var topicData = await _redisService.HashGetAsync($"{TopicPrefix}{topic.Name}", "data");
                if (!string.IsNullOrEmpty(topicData))
                {
                    topics.Add(JsonSerializer.Deserialize<Topic>(topicData));
                }
            }

            return topics;
        }
    }
}