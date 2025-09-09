using PubSubMessagingSystem.API.Models;
using PubSubMessagingSystem.API.Models.DTOs;
using PubSubMessagingSystem.API.Services.Interfaces;
using System.Text.Json;

namespace PubSubMessagingSystem.API.Services.Implementations
{
    public class MessageService : IMessageService
    {
        private readonly IRedisService _redisService;
        private const string MessagesPrefix = "messages:";

        public MessageService(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task<Message> PublishMessageAsync(MessageRequest request)
        {
            var message = new Message
            {
                TopicId = request.TopicId,
                Payload = request.GetSerializedPayload(),
                MessageType = request.MessageType
            };

            // Store message in Redis list
            await _redisService.ListLeftPushAsync($"{MessagesPrefix}{request.TopicId}", JsonSerializer.Serialize(message));

            // Publish to Redis channel for real-time delivery
            await _redisService.PublishAsync($"topic:{request.TopicId}", JsonSerializer.Serialize(message));

            return message;
        }

        public async Task<IEnumerable<Message>> PublishBatchMessagesAsync(IEnumerable<MessageRequest> requests)
        {
            var messages = new List<Message>();

            foreach (var request in requests)
            {
                var message = await PublishMessageAsync(request);
                messages.Add(message);
            }

            return messages;
        }

        public async Task<IEnumerable<Message>> GetMessagesFromTopicAsync(string topicId, int count = 10)
        {
            var messages = new List<Message>();
            var messageValues = await _redisService.ListRangeAsync($"{MessagesPrefix}{topicId}", 0, count - 1);

            foreach (var messageValue in messageValues)
            {
                if (!string.IsNullOrEmpty(messageValue))
                {
                    messages.Add(JsonSerializer.Deserialize<Message>(messageValue));
                }
            }

            return messages;
        }

        public async Task<long> GetMessageCountForTopicAsync(string topicId)
        {
            return await _redisService.ListLengthAsync($"{MessagesPrefix}{topicId}");
        }
    }
}