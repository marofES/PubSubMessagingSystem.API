using System.Text.Json;

namespace PubSubMessagingSystem.API.Models.DTOs
{
    public class MessageRequest
    {
        public string TopicId { get; set; }
        public object Payload { get; set; }
        public string MessageType { get; set; } = "string";

        public string GetSerializedPayload()
        {
            return MessageType.ToLower() switch
            {
                "json" => JsonSerializer.Serialize(Payload),
                "dictionary" => JsonSerializer.Serialize((Dictionary<string, object>)Payload),
                _ => Payload?.ToString() ?? string.Empty
            };
        }
    }
}