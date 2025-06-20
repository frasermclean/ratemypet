﻿using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using RateMyPet.Core.Abstractions;
using RateMyPet.Storage.Messaging;

namespace RateMyPet.Storage;

public class MessagePublisher(ILogger<MessagePublisher> logger, QueueServiceClient serviceClient) : IMessagePublisher
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<string> PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : IMessage
    {
        // serialize message
        var data = new BinaryData(message, SerializerOptions);

        // publish message to queue
        var queueClient = GetQueueClient(message);
        var response = await queueClient.SendMessageAsync(data, cancellationToken: cancellationToken);

        logger.LogInformation("Published {Message} to queue: {QueueName} - Message ID: {MessageId}",
            message, queueClient.Name, response.Value.MessageId);

        return response.Value.MessageId;
    }

    private QueueClient GetQueueClient<T>(T message) where T : IMessage => message switch
    {
        ForgottenPasswordMessage => serviceClient.GetQueueClient(QueueNames.ForgotPassword),
        PostAddedMessage => serviceClient.GetQueueClient(QueueNames.PostAdded),
        PostDeletedMessage => serviceClient.GetQueueClient(QueueNames.PostDeleted),
        RegisterConfirmationMessage => serviceClient.GetQueueClient(QueueNames.RegisterConfirmation),
        _ => throw new ArgumentException("Unsupported message type")
    };
}
