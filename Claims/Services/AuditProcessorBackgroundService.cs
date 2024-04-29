using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Claims.Services
{
    public class AuditProcessorBackgroundService : BackgroundService, IAuditProcessingBackgroundService
    {
        private readonly ConcurrentQueue<Task> _auditQueue;
        private readonly IServiceScopeFactory _scopeFactory;

        public AuditProcessorBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _auditQueue = new ConcurrentQueue<Task>();
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_auditQueue.TryDequeue(out var auditTask))
                {
                    try
                    {
                        await auditTask;
                    }
                    catch
                    {
                        // Handle exceptions or requeue the task as necessary
                    }
                }
                else
                {
                    await Task.Delay(1000, stoppingToken); // Wait for more tasks
                }
            }
        }

        public void EnqueueAudit(Task auditTask)
        {
            _auditQueue.Enqueue(auditTask);
        }
    }
}
