using Microsoft.AspNetCore.SignalR;

namespace PubSubMessagingSystem.API.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinTopicGroup(string topicId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, topicId);
        }

        public async Task LeaveTopicGroup(string topicId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, topicId);
        }
    }
}