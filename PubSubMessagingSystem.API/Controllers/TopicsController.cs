using Microsoft.AspNetCore.Mvc;
using PubSubMessagingSystem.API.Models;
using PubSubMessagingSystem.API.Models.DTOs;
using PubSubMessagingSystem.API.Services.Interfaces;

namespace PubSubMessagingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicsController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicsController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpPost]
        public async Task<ActionResult<Topic>> CreateTopic([FromBody] CreateTopicRequest request)
        {
            var topic = await _topicService.CreateTopicAsync(request);
            return CreatedAtAction(nameof(GetTopic), new { id = topic.Id }, topic);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Topic>> GetTopic(string id)
        {
            var topic = await _topicService.GetTopicAsync(id);
            if (topic == null)
            {
                return NotFound();
            }
            return topic;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Topic>>> GetAllTopics()
        {
            var topics = await _topicService.GetAllTopicsAsync();
            return Ok(topics);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(string id)
        {
            var result = await _topicService.DeleteTopicAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}