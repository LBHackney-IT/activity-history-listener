using ActivityListener.Boundary;
using ActivityListener.Factories;
using ActivityListener.Gateway.Interfaces;
using ActivityListener.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace ActivityListener.UseCase
{
    public class MessageProcessor : IMessageProcessing
    {
        private readonly IDynamoDbGateway _dbGateway;

        public MessageProcessor(IDynamoDbGateway dbGateway)
        {
            _dbGateway = dbGateway;
        }

        public async Task ProcessMessageAsync(EntityEventSns message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            var domainObject = message.ToDomain();
            await _dbGateway.SaveAsync(domainObject).ConfigureAwait(false);
        }
    }
}
