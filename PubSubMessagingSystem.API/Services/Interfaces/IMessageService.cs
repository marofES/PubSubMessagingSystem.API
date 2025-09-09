using PubSubMessagingSystem.API.Models;
using PubSubMessagingSystem.API.Models.DTOs;

namespace PubSubMessagingSystem.API.Services.Interfaces
{
    public interface IMessageService
    {
        Task<Message> PublishMessageAsync(MessageRequest request);
        Task<IEnumerable<Message>> PublishBatchMessagesAsync(IEnumerable<MessageRequest> requests);
        Task<IEnumerable<Message>> GetMessagesFromTopicAsync(string topicId, int count = 10);
        Task<long> GetMessageCountForTopicAsync(string topicId);
    }
}