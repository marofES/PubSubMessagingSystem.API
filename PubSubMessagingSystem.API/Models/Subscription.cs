namespace PubSubMessagingSystem.API.Models
{
    public class Subscription
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TopicId { get; set; }
        public string SubscriberId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}