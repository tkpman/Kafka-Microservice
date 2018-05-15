using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HostedServices
{
    public class JobHostingService
        : IHostedService
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly List<Task> _tasks;
        private readonly List<IJob> _jobs;
        private readonly IServiceProvider _serviceProvider;

        public JobHostingService(IServiceProvider serviceProvider)
        {
            this._cancellationTokenSource = new CancellationTokenSource();
            this._tasks = new List<Task>();
            this._jobs = new List<IJob>();
            this._serviceProvider = serviceProvider;
        }

        public void AddJob(IJob job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            this._jobs.Add(job);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach(var job in this._jobs)
                Task.Factory.StartNew(
                    () => job.Run(this._cancellationTokenSource.Token),
                    TaskCreationOptions.LongRunning);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._cancellationTokenSource.Cancel();

            var tasks = Task.WhenAll(this._tasks);

            return Task.WhenAny(tasks, Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }

    public interface IJob
    {
        void Run(CancellationToken cancellationToken);
    }

    public abstract class Job
        : IJob
    {
        public abstract void Run(CancellationToken cancellationToken);
    }
}
