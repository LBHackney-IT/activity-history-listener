using System.Threading.Tasks;
using ActivityListener.Boundary;

namespace ActivityListener.UseCase.Interfaces
{
    public interface IMessageProcessing
    {
        Task ProcessMessageAsync(EntityEventSns message);
    }
}
