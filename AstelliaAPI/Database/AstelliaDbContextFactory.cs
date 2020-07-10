﻿using System.Threading;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace AstelliaAPI.Database
{
    // Modified version of https://github.com/ppy/osu/blob/master/osu.Game/Database/DatabaseContextFactory.cs under MIT License!
    public sealed class AstelliaDbContextFactory
    {
        private readonly object writeLock = new object();
        private bool currentWriteDidError;

        private bool currentWriteDidWrite;

        private IDbContextTransaction currentWriteTransaction;

        private int currentWriteUsages;
        private ThreadLocal<AstelliaDbContext> threadContexts;

        public AstelliaDbContextFactory()
        {
            recycleThreadContexts();
        }

        /// <summary>
        ///     Get a context for the current thread for read-only usage.
        ///     If a <see cref="DatabaseWriteUsage" /> is in progress, the existing write-safe context will be returned.
        /// </summary>
        public AstelliaDbContext Get()
        {
            return threadContexts.Value;
        }

        /// <summary>
        ///     Request a context for write usage. Can be consumed in a nested fashion (and will return the same underlying
        ///     context).
        ///     This method may block if a write is already active on a different thread.
        /// </summary>
        /// <param name="withTransaction">Whether to start a transaction for this write.</param>
        /// <returns>A usage containing a usable context.</returns>
        public DatabaseWriteUsage GetForWrite(bool withTransaction = true)
        {
            Monitor.Enter(writeLock);
            AstelliaDbContext context;

            try
            {
                if (currentWriteTransaction == null && withTransaction)
                {
                    // this mitigates the fact that changes on tracked entities will not be rolled back with the transaction by ensuring write operations are always executed in isolated contexts.
                    // if this results in sub-optimal efficiency, we may need to look into removing Database-level transactions in favour of running SaveChanges where we currently commit the transaction.
                    if (threadContexts.IsValueCreated)
                        recycleThreadContexts();

                    context = threadContexts.Value;
                    currentWriteTransaction = context.Database.BeginTransaction();
                }
                else
                {
                    // we want to try-catch the retrieval of the context because it could throw an error (in CreateContext).
                    context = threadContexts.Value;
                }
            }
            catch
            {
                // retrieval of a context could trigger a fatal error.
                Monitor.Exit(writeLock);
                throw;
            }

            Interlocked.Increment(ref currentWriteUsages);

            return new DatabaseWriteUsage(context, usageCompleted)
            {
                IsTransactionLeader = currentWriteTransaction != null && currentWriteUsages == 1
            };
        }

        private void usageCompleted(DatabaseWriteUsage usage)
        {
            var usages = Interlocked.Decrement(ref currentWriteUsages);

            try
            {
                currentWriteDidWrite |= usage.PerformedWrite;
                currentWriteDidError |= usage.Errors.Any();

                if (usages == 0)
                {
                    if (currentWriteDidError)
                        currentWriteTransaction?.Rollback();
                    else
                        currentWriteTransaction?.Commit();

                    if (currentWriteDidWrite || currentWriteDidError)
                    {
                        // explicitly dispose to ensure any outstanding flushes happen as soon as possible (and underlying resources are purged).
                        usage.Context.Dispose();

                        // once all writes are complete, we want to refresh thread-specific contexts to make sure they don't have stale local caches.
                        recycleThreadContexts();
                    }

                    currentWriteTransaction = null;
                    currentWriteDidWrite = false;
                    currentWriteDidError = false;
                }
            }
            finally
            {
                Monitor.Exit(writeLock);
            }
        }

        private void recycleThreadContexts()
        {
            // Contexts for other threads are not disposed as they may be in use elsewhere. Instead, fresh contexts are exposed
            // for other threads to use, and we rely on the finalizer inside SoraDbContext to handle their previous contexts
            threadContexts?.Value.Dispose();
            threadContexts = new ThreadLocal<AstelliaDbContext>(CreateContext, true);
        }

        private AstelliaDbContext CreateContext()
        {
            return new AstelliaDbContext {Database = {AutoTransactionsEnabled = false}};
        }

        public void ResetDatabase()
        {
            lock (writeLock)
            {
                recycleThreadContexts();
            }
        }
    }
}