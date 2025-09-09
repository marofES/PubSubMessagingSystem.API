namespace PubSubMessagingSystem.API.Models.DTOs
{
    public class SubscriptionRequest
    {
        public string TopicId { get; set; }
        public string SubscriberId { get; set; }
    }
}