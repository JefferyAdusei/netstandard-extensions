namespace Spyder.Extensions.Task.Locks
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// <para>
    ///     Adds the ability to do the same as lock(...) { } but for async
    ///     Tasks and awaits
    /// </para>
    /// <para>
    ///     This lock uses the lightweight semaphore slim to prevent any chance
    ///     of a deadlock.
    /// </para>
    /// </summary>
    /// <example>
    ///     <code>
    ///         await AsyncLock.LockAsync("key", () => Work());
    ///     </code>
    /// </example>
    public static class AsyncLock
    {
        #region Private Members

        /// <summary>
        /// A semaphore to lock the semaphore list
        /// </summary>
        private static readonly SemaphoreSlim SelfLock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// A list of all semaphore locks (one per key)
        /// </summary>
        private static readonly Dictionary<string, SemaphoreSlim> Semaphores = new Dictionary<string, SemaphoreSlim>();

        #endregion

        #region Lock Async

        /// <summary>
        /// Awaits for any outstanding tasks to complete that are accessing
        /// the same key, then runs the given task, returning its value.
        /// </summary>
        /// <typeparam name="T">The type of the task that will be returned</typeparam>
        /// <param name="key">The key to await</param>
        /// <param name="task">The task to perform inside the semaphore lock</param>
        /// <param name="maxAccessCount">Sets the maximum number of tasks that can access this task before waiting</param>
        /// <returns>The result of the task</returns>
        public static async Task<T> LockAsync<T>(string key, Func<Task<T>> task, int maxAccessCount = 1)
        {
            /*
             * Asynchronously wait to enter the semaphore
             *
             * If no one has been granted access to the semaphore,
             * code execution will proceed.
             *
             * Otherwise this thread waits here until the semaphore is released.
             */
            await SelfLock.WaitAsync();

            try
            {
                // If the semaphore with this key does not exist...
                if (!Semaphores.ContainsKey(key))
                {
                    // Create it
                    Semaphores.Add(key, new SemaphoreSlim(maxAccessCount, maxAccessCount));
                }
            }
            finally
            {
                /*
                 * When the task is ready, release the semaphore
                 *
                 * It is vital to ALWAYS release the semaphore when we are ready
                 * or else we will end up with a semaphore that is forever locked.
                 *
                 * This is why it is important to do the release within a try ... finally
                 * clause.
                 *
                 * Program execution may crash or take a different path, this way you are guaranteed
                 * execution.
                 */
                SelfLock.Release();
            }

            /* Now use this semaphore and perform the desired task inside its lock.
             *
             * NOTE: We never remove semaphores after creating them, so this will
             * never be null.
             */
            SemaphoreSlim semaphore = Semaphores[key];

            // Await this semaphore
            await semaphore.WaitAsync();

            try
            {
                return await task();
            }
            finally
            {
                // Release the semaphore
                semaphore.Release();
            }
        }

        #endregion
    }
}