using PubSubMessagingSystem.API.Models;
using PubSubMessagingSystem.API.Models.DTOs;
using PubSubMessagingSystem.API.Services.Interfaces;
using System.Text.Json;

namespace PubSubMessagingSystem.API.Services.Implementations
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IRedisService _redisService;
        private const string SubscriptionPrefix = "subscription:";
        private const string TopicSubscriptionsPrefix = "subscriptions:";
        private const string SubscriberSubscriptionsPrefix = "subscriber:";

        public SubscriptionService(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task<Subscription> SubscribeAsync(SubscriptionRequest request)
        {
            var subscription = new Subscription
            {
                TopicId = request.TopicId,
                SubscriberId = request.SubscriberId
            };

            // Store subscription details
            await _redisService.HashSetAsync(
                $"{SubscriptionPrefix}{subscription.Id}",
                "data",
                JsonSerializer.Serialize(subscription));

            // Add to topic subscriptions
            await _redisService.HashSetAsync(
                $"{TopicSubscriptionsPrefix}{request.TopicId}",
                subscription.Id,
                request.SubscriberId);

            // Add to subscriber subscriptions
            await _redisService.HashSetAsync(
                $"{SubscriberSubscriptionsPrefix}{request.SubscriberId}",
                subscription.Id,
                request.TopicId);

            return subscription;
        }

        public async Task<bool> UnsubscribeAsync(string subscriptionId)
        {
            // Get subscription details
            var subscriptionData = await _redisService.HashGetAsync($"{SubscriptionPrefix}{subscriptionId}", "data");
            if (string.IsNullOrEmpty(subscriptionData))
                return false;

            var subscription = JsonSerializer.Deserialize<Subscription>(subscriptionData);

            // Remove from topic subscriptions
            await _redisService.HashDeleteAsync($"{TopicSubscriptionsPrefix}{subscription.TopicId}", subscriptionId);

            // Remove from subscriber subscriptions
            await _redisService.HashDeleteAsync($"{SubscriberSubscriptionsPrefix}{subscription.SubscriberId}", subscriptionId);

            // Delete subscription
            await _redisService.KeyDeleteAsync($"{SubscriptionPrefix}{subscriptionId}");

            return true;
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsForTopicAsync(string topicId)
        {
            var subscriptions = new List<Subscription>();
            var subscriptionIds = await _redisService.HashGetAllAsync($"{TopicSubscriptionsPrefix}{topicId}");

            foreach (var subscriptionId in subscriptionIds)
            {
                var subscriptionData = await _redisService.HashGetAsync($"{SubscriptionPrefix}{subscriptionId.Name}", "data");
                if (!string.IsNullOrEmpty(subscriptionData))
                {
                    subscriptions.Add(JsonSerializer.Deserialize<Subscription>(subscriptionData));
                }
            }

            return subscriptions;
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsForSubscriberAsync(string subscriberId)
        {
            var subscriptions = new List<Subscription>();
            var subscriptionIds = await _redisService.HashGetAllAsync($"{SubscriberSubscriptionsPrefix}{subscriberId}");

            foreach (var subscriptionId in subscriptionIds)
            {
                var subscriptionData = await _redisService.HashGetAsync($"{SubscriptionPrefix}{subscriptionId.Name}", "data");
                if (!string.IsNullOrEmpty(subscriptionData))
                {
                    subscriptions.Add(JsonSerializer.Deserialize<Subscription>(subscriptionData));
                }
            }

            return subscriptions;
        }
    }
}