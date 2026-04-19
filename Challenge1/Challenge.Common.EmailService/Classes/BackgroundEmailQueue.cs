using challenge1.Common.EmailService.Classes.Interfaces;
using System.Threading.Channels;

namespace challenge1.Common.EmailService.Classes
{
    public class BackgroundEmailQueue : IBackgroundEmailQueue
    {
        private readonly Channel<Func<CancellationToken, Task>> _queue =
            Channel.CreateUnbounded<Func<CancellationToken, Task>>();

        public void QueueEmail(Func<CancellationToken, Task> workItem) =>
            _queue.Writer.TryWrite(workItem);

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken) =>
            await _queue.Reader.ReadAsync(cancellationToken);
    }
}
