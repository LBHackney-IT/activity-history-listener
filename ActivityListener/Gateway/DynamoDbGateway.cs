using ActivityListener.Domain;
using ActivityListener.Factories;
using ActivityListener.Gateway.Interfaces;
using Amazon.DynamoDBv2.DataModel;
using Hackney.Core.Logging;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ActivityListener.Gateway
{
    public class DynamoDbGateway : IDynamoDbGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ILogger<DynamoDbGateway> _logger;

        public DynamoDbGateway(IDynamoDBContext dynamoDbContext, ILogger<DynamoDbGateway> logger)
        {
            _logger = logger;
            _dynamoDbContext = dynamoDbContext;
        }

        [LogCall]
        public async Task SaveAsync(ActivityHistoryEntity activityHistory)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync for target id {activityHistory.TargetId}, id {activityHistory.Id}");
            await _dynamoDbContext.SaveAsync(activityHistory.ToDatabase()).ConfigureAwait(false);
        }
    }
}
