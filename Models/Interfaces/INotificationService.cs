using System.Threading.Tasks;

namespace BookSwap.Models.Interfaces
{
    public interface INotificationService
    {
        Task NotifyAsync(string recipientEmail, string message);
    }
}
