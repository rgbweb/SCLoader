﻿using SCLoaderShared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCLoader
{
    class InstanceLock
    {

        public const int RetryIntervalMinutes = 5;

        public TimeSpan Lifetime;
        public IStorageProvider StorageProvider;
        public ILogger Logger;

        public CancellationTokenSource CancellationTokenSource;
        public Action OnLockAppliedCallback;

        private Task lockApplyTask;

        public InstanceLock(TimeSpan lifetime, IStorageProvider storageProvider, ILogger logger)
        {

            this.Lifetime = lifetime;
            this.StorageProvider = storageProvider;
            this.Logger = logger;

            this.CancellationTokenSource = new CancellationTokenSource();

        }

        /// <summary>
        /// Sets the instance lock in the specified storage
        /// Waits for an existing lock to be released or timed out
        /// </summary>
        /// <param name="onLockAppliedCallback">Action is called after the lock was applied</param>
        public void ApplyAsync(Action onLockAppliedCallback)
        {

            this.OnLockAppliedCallback = onLockAppliedCallback;

            this.lockApplyTask = new Task((state) =>
            {
                try
                {
                    ApplyLockTask(state).Wait();
                }
                catch (AggregateException ex)
                {
                    // Mark the exception thrown by an CancellationToken as handled
                    ex.Handle((innerEx) =>
                    {
                        return innerEx is TaskCanceledException;
                    });
                }
            }, this, this.CancellationTokenSource.Token);

            // Check if the Task crashed with an exception
            this.lockApplyTask.ContinueWith((task) =>
            {
                if (task.Exception != null)
                {
                    // The state object is an instance of the current class
                    var instance = (InstanceLock)task.AsyncState;
                    instance.Logger.LogException("Failed to apply InstanceLock. See exception for details.", task.Exception);
                }
            });

            this.lockApplyTask.Start();

        }

        /// <summary>
        /// Releases the lock from the specified storage
        /// Stops trying to apply a lock if not applied yet
        /// </summary>
        public void Release()
        {

            if (!this.lockApplyTask.IsCompleted)
            {
                // Lock was not applied yet
                this.CancellationTokenSource.Cancel();
                this.lockApplyTask.Wait(1000);
            }
            else
            {
                this.StorageProvider.ReleaseInstanceLock();
            }

        }


        private async Task ApplyLockTask(object state)
        {

            // The state object is an instance of the current class
            var instance = (InstanceLock)state;

            while (!instance.CancellationTokenSource.Token.IsCancellationRequested && !instance.StorageProvider.TryApplyInstanceLock(instance.Lifetime))
            {

                instance.Logger.LogVerbose("Failed to apply InstanceLock: Another instance of SCLoader is running. Wait {0} minutes until recheck...",
                    InstanceLock.RetryIntervalMinutes);

                await Task.Delay(TimeSpan.FromMinutes(InstanceLock.RetryIntervalMinutes), instance.CancellationTokenSource.Token);

            }

            instance.CancellationTokenSource.Token.ThrowIfCancellationRequested();

            instance.Logger.LogVerbose("InstanceLock applied.");

            instance.OnLockAppliedCallback.Invoke();

        }


    }
}
