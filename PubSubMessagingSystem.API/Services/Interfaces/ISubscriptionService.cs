using PubSubMessagingSystem.API.Models;
using PubSubMessagingSystem.API.Models.DTOs;

namespace PubSubMessagingSystem.API.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<Subscription> SubscribeAsync(SubscriptionRequest request);
        Task<bool> UnsubscribeAsync(string subscriptionId);
        Task<IEnumerable<Subscription>> GetSubscriptionsForTopicAsync(string topicId);
        Task<IEnumerable<Subscription>> GetSubscriptionsForSubscriberAsync(string subscriberId);
    }
}