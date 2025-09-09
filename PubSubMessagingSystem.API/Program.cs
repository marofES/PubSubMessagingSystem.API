using Microsoft.AspNetCore.SignalR;
using PubSubMessagingSystem.API.Hubs;
using PubSubMessagingSystem.API.Services.Implementations;
using PubSubMessagingSystem.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Redis
builder.Services.Configure<RedisConfiguration>(builder.Configuration.GetSection("Redis"));
builder.Services.AddSingleton<IRedisService, RedisService>();

// Register application services
//builder.Services.AddScoped<ITopicService, TopicService>();
//builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
//builder.Services.AddScoped<IMessageService, MessageService>();

// Register application services as singletons
builder.Services.AddSingleton<ITopicService, TopicService>();
builder.Services.AddSingleton<ISubscriptionService, SubscriptionService>();
builder.Services.AddSingleton<IMessageService, MessageService>();

// Add SignalR
builder.Services.AddSignalR();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Map SignalR hub
app.MapHub<NotificationHub>("/notificationHub");

// Set up Redis subscriptions for real-time messaging
var redisService = app.Services.GetRequiredService<IRedisService>();
var hubContext = app.Services.GetRequiredService<IHubContext<NotificationHub>>();

// Subscribe to all topic channels
var topicService = app.Services.GetRequiredService<ITopicService>();
var topics = await topicService.GetAllTopicsAsync();

foreach (var topic in topics)
{
    redisService.Subscribe($"topic:{topic.Id}", (channel, message) =>
    {
        // Forward Redis message to SignalR clients
        hubContext.Clients.Group(topic.Id).SendAsync("ReceiveMessage", message.ToString());
    });
}

app.Run();