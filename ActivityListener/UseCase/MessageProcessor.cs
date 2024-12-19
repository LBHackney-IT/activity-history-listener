using ActivityListener.Boundary;
using ActivityListener.Factories;
using ActivityListener.Gateway.Interfaces;
using ActivityListener.UseCase.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ActivityListener.UseCase
{
    public class MessageProcessor : IMessageProcessing
    {
        private readonly IDynamoDbGateway _dbGateway;
        private readonly ILogger<MessageProcessor> _logger;

        public MessageProcessor(IDynamoDbGateway dbGateway)
        {
            _dbGateway = dbGateway;
            _logger = new LoggerFactory().CreateLogger<MessageProcessor>();
        }

        public async Task ProcessMessageAsync(EntityEventSns message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            var domainObject = message.ToDomain();
            if (domainObject is null)
            {
                _logger.LogWarning($"Will not process message of type: {message.EventType}");
                return;
            }

            await _dbGateway.SaveAsync(domainObject).ConfigureAwait(false);
        }
    }
}
