using PubSubMessagingSystem.API.Models;
using PubSubMessagingSystem.API.Models.DTOs;

namespace PubSubMessagingSystem.API.Services.Interfaces
{
    public interface ITopicService
    {
        Task<Topic> CreateTopicAsync(CreateTopicRequest request);
        Task<bool> DeleteTopicAsync(string topicId);
        Task<Topic?> GetTopicAsync(string topicId);
        Task<IEnumerable<Topic>> GetAllTopicsAsync();
    }
}