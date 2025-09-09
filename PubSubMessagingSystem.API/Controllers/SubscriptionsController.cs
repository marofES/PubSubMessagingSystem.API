using Microsoft.AspNetCore.Mvc;
using PubSubMessagingSystem.API.Models;
using PubSubMessagingSystem.API.Models.DTOs;
using PubSubMessagingSystem.API.Services.Interfaces;

namespace PubSubMessagingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost]
        public async Task<ActionResult<Subscription>> Subscribe([FromBody] SubscriptionRequest request)
        {
            var subscription = await _subscriptionService.SubscribeAsync(request);
            return CreatedAtAction(nameof(GetSubscription), new { id = subscription.Id }, subscription);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Subscription>> GetSubscription(string id)
        {
            // This would require a new method in ISubscriptionService
            // For simplicity, we'll return a placeholder
            return NotFound();
        }

        [HttpGet("topic/{topicId}")]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptionsForTopic(string topicId)
        {
            var subscriptions = await _subscriptionService.GetSubscriptionsForTopicAsync(topicId);
            return Ok(subscriptions);
        }

        [HttpGet("subscriber/{subscriberId}")]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptionsForSubscriber(string subscriberId)
        {
            var subscriptions = await _subscriptionService.GetSubscriptionsForSubscriberAsync(subscriberId);
            return Ok(subscriptions);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Unsubscribe(string id)
        {
            var result = await _subscriptionService.UnsubscribeAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}