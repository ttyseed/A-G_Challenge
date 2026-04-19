namespace challenge1.Common.EmailService.Classes.Interfaces
{
    public interface IBackgroundEmailQueue
    {
        void QueueEmail(Func<CancellationToken, Task> workItem);
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
