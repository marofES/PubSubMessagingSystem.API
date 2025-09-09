namespace PubSubMessagingSystem.API.Models
{
    public class Message
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TopicId { get; set; }
        public string Payload { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string MessageType { get; set; } = "string";
    }
}