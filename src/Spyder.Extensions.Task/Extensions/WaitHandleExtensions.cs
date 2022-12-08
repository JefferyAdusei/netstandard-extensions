namespace Spyder.Extensions.Task.Extensions
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="WaitHandle"/>
    /// </summary>
    public static class WaitHandleExtensions
    {
        /// <summary>
        /// Allows awaiting a <see cref="WaitHandle"/> which
        /// <inheritdoc cref="WaitHandle"/>
        /// </summary>
        /// <param name="handle">The <see cref="WaitHandle"/> to await.</param>
        /// <param name="timeout">The timeout period in milliseconds to return false if timed out.
        ///     <code>
        ///         // In order to use timeouts and infinitely wait till a resource is free use
        ///         (int)timeout.Infinite
        ///     </code></param>
        /// <param name="token">The cancellation token to use to throw a <see cref="TaskCanceledException"/>
        ///                     if this token gets cancelled</param>
        /// <returns>True if the handle is free, false if it is not</returns>
        /// <exception cref="TaskCanceledException">Throws if the cancellation token is invoked</exception>
        /// <example>
        ///     handle.WaitHandleAsync((int)Timeout.Infinite, cancellationToken);
        /// </example>
        public static async Task<bool> WaitHandleAsync(this WaitHandle handle, int timeout, CancellationToken? token)
        {
            // Create a handle that awaits the original wait handle
            RegisteredWaitHandle? registeredWaitHandle = null;

            // Store the token
            CancellationTokenRegistration? tokenRegistration = null;

            try
            {
                // Create a new task completion source to await
                TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

                /* Use RegisterWaitForSingleObject so we get a callback
                 * once the wait handle has finished, and set the taskCompletionSource result in that callback.
                 */
                registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
                      // The handle to wait
                      handle,
                      // When it is finished, set the taskCompletionSource
                      (state, timedOut) =>
                          ((TaskCompletionSource<bool>)state)
                          .TrySetResult(!timedOut),
                      // Pass the taskCompletion source as the state so we don't have a reference to the parent
                      // taskCompletionSource (optimization)
                      taskCompletionSource,
                      // Set timeout if passed in
                      timeout,
                      // Run once don't keep resetting timeout
                      true);

                /*
                 * Register to run the action and set the taskCompletionSource as cancelled
                 * if the cancellation token itself is cancelled; which will throw a TaskCancelledException
                 * up to the caller.
                 */
                if (token.HasValue)
                {
                    tokenRegistration = token.Value.Register((state) =>
                                                                 ((TaskCompletionSource<bool>)state)
                                                                 .TrySetCanceled(), taskCompletionSource);
                }

                // Await the handle or the cancellation token
                return await taskCompletionSource.Task;
            }
            finally
            {
                // Clean up registered wait handle
                registeredWaitHandle?.Unregister(null);

                // Dispose of the token we had to create to register for the cancellation token callback
                tokenRegistration?.DisposeAsync();
            }
        }
    }
}