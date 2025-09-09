using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PubSubMessagingSystem.API.Hubs;
using PubSubMessagingSystem.API.Models;
using PubSubMessagingSystem.API.Models.DTOs;
using PubSubMessagingSystem.API.Services.Interfaces;

namespace PubSubMessagingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IRedisService _redisService;

        public MessagesController(
            IMessageService messageService,
            IHubContext<NotificationHub> hubContext,
            IRedisService redisService)
        {
            _messageService = messageService;
            _hubContext = hubContext;
            _redisService = redisService;
        }

        [HttpPost]
        public async Task<ActionResult<Message>> PublishMessage([FromBody] MessageRequest request)
        {
            var message = await _messageService.PublishMessageAsync(request);

            // Send real-time notification via SignalR
            await _hubContext.Clients.Group(request.TopicId).SendAsync("ReceiveMessage", message);

            return CreatedAtAction(nameof(GetMessages), new { topicId = request.TopicId }, message);
        }

        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<Message>>> PublishBatchMessages([FromBody] IEnumerable<MessageRequest> requests)
        {
            var messages = await _messageService.PublishBatchMessagesAsync(requests);

            // Send real-time notifications for each message
            foreach (var message in messages)
            {
                await _hubContext.Clients.Group(message.TopicId).SendAsync("ReceiveMessage", message);
            }

            return CreatedAtAction(nameof(GetMessages), new { topicId = messages.First().TopicId }, messages);
        }

        [HttpGet("topic/{topicId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages(string topicId, [FromQuery] int count = 10)
        {
            var messages = await _messageService.GetMessagesFromTopicAsync(topicId, count);
            return Ok(messages);
        }

        [HttpGet("count/{topicId}")]
        public async Task<ActionResult<long>> GetMessageCount(string topicId)
        {
            var count = await _messageService.GetMessageCountForTopicAsync(topicId);
            return Ok(count);
        }
    }
}