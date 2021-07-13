using ActivityListener.Domain;
using System.Threading.Tasks;

namespace ActivityListener.Gateway.Interfaces
{
    public interface IDynamoDbGateway
    {
        Task SaveAsync(ActivityHistoryEntity activityHistory);
    }
}
