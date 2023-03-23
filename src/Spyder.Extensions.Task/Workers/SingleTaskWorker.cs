namespace Spyder.Extensions.Task.Workers;

using Spyder.Extensions.Logging;
using Extensions;
using Locks;
using Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// <para>
///     Provides a thread-safe mechanism for starting and stopping the execution
///     of a task that can only have one instance of itself running regardless of the
///     amount of times start/stop is called and from what thread.
/// </para>
/// <para>
///     Supports cancellation request via the <see cref="StopAsync"/> and the given
///     task will be provided with a cancellation token to monitor for when it should
///     "stop"
/// </para>
/// </summary>
public abstract class SingleTaskWorker
{
    #region Protected Members

    /// <summary>
    /// A flag indicating if the worker task is still running.
    /// </summary>
    protected ManualResetEvent WorkerFinishedEvent = new ManualResetEvent(true);

    /// <summary>
    /// The token used to cancel any ongoing work in order to shutdown
    /// </summary>
    protected CancellationTokenSource CancellationToken = new CancellationTokenSource();

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets a unique ID for locking the starting and stopping calls of this class
    /// </summary>
    public string LockingKey { get; } = nameof(SingleTaskWorker) + Guid.NewGuid();

    /// <summary>
    /// Gets The name that identifies the worker (used in unhandled exception logs to report
    /// source of an issue)
    /// </summary>
    public abstract string WorkerName { get; }

    /// <summary>
    /// Gets a value indicating whether the service is shutting down, and whether it should finish
    /// what it's doing and save any important information or progress.
    /// </summary>
    public bool Stopping => CancellationToken.IsCancellationRequested;

    /// <summary>
    /// Gets or sets a value indicating whether the main worker task is running
    /// </summary>
    public bool IsRunning
    {
        // We are running if we haven't finished
        get => !WorkerFinishedEvent.WaitOne(0);
        set
        {
            // If we are running...
            if (value)
            {
                // Then we have not finished
                WorkerFinishedEvent.Reset();
            }
            else
            {
                // Stop
                Task.Run(async () => await StopAsync()
                    .ConfigureAwait(false));
            }
        }
    }

    #endregion

    #region Startup and Shutdown

    /// <summary>
    /// Starts the given task running if it is not already running
    /// </summary>
    /// <returns></returns>
    public async Task<bool> StartAsync()
    {
        // Log it
        StaticLogger.Get().LogTraceSource($"Start requested...");

        // Make sure only one start or stop call runs at a time.
        return await AsyncLock.LockAsync(LockingKey, () =>
        {
            // If we are already running...
            if (IsRunning)
            {
                // Log it
                StaticLogger.Get().LogTraceSource($"Already started. Ignoring...");

                // We are not starting a new task
                return Task.FromResult(false);
            }

            // New cancellation token
            CancellationToken = new CancellationTokenSource();

            // Flag that we are running
            IsRunning = true;

            // Log it
            StaticLogger.Get().LogTraceSource($"Starting worker process...");

            // Start the main task
            RunWorkerTaskNoAwait();

            // We have started a new task
            return Task.FromResult(true);
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Requests that the given task should stop running, and waits for it
    /// to finish
    /// </summary>
    /// <returns></returns>
    public async Task<bool> StopAsync()
    {
        // Make sure only  one startup or shutdown call runs at a time...
        return await AsyncLock.LockAsync(LockingKey, async () =>
        {
            // If we are not running
            if (!IsRunning)
            {
                // Log it
                StaticLogger.Get()?.LogTraceSource($"Already stopped. Ignoring...");

                // Ignore
                return false;
            }

            // Log it
            StaticLogger.Get()?.LogTraceSource($"Stop requested...");

            // Flag main worker to shut down
            CancellationToken.Cancel();

            // Wait for it to stop running
            return await WorkerFinishedEvent.WaitHandleAsync(0, null).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    #endregion

    #region Worker Task

    /// <summary>
    /// Runs the worker task and sets the IsRunning to false once complete
    /// </summary>
    /// <returns>Return once the worker task has completed</returns>
    protected void RunWorkerTaskNoAwait()
    {
        /* IMPORTANT: Not awaiting a Task leads to swallowed exceptions so
         *            we try/catch the entire task and report any unhandled
         *            exception to the log
         */
        Task.Run(async () =>
        {
            try
            {
                // Log
                StaticLogger.Get()?.LogTraceSource($"Worker task started...");

                // Run given task
                await WorkerTaskAsync(CancellationToken.Token).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception exception)
            {
                // Log unhandled exception
                StaticLogger.Get()?.LogCriticalSource($"Unhandled exception in {nameof(SingleTaskWorker)},'{WorkerName}'",
                    exception: exception);
            }
            finally
            {
                // Log it
                StaticLogger.Get().LogTraceSource($"Worker task finished");

                // Set finished event informing waiters we are finished working
                WorkerFinishedEvent.Set();
            }
        });
    }

    /// <summary>
    /// The task that will be run by this worker
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>An un-awaitable task</returns>
    protected virtual Task WorkerTaskAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
    #endregion
}