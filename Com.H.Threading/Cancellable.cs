﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Com.H.Threading
{
    public static class Cancellable
    {
        public static Task<T> CancellableWait<T>(this Task<T> task,
            int? timeout = null,
            CancellationToken? token = null,
            Action actionOnTimeout = null
            )
        {
            timeout = timeout ?? -1;
            var delayTask = token == null ?
                Task.Delay((int)timeout) :
                Task.Delay((int)timeout, (CancellationToken)token);
            var result = Task.WhenAny(task, delayTask).Result;

            if (actionOnTimeout != null
                && result == delayTask
                && delayTask.IsCompleted) actionOnTimeout();

            return task;
        }
        public static void CancellableRun(Action action, CancellationTokenSource cts)
        {
            CancellableRun(action, cts.Token);
        }
        private static int action_count = 0;
        public static void CancellableRun(Action action, CancellationToken token)
        {
            try
            {
                Task.Run(() =>
                {
                    using (var reg = token.Register(() =>
                    {
                        try
                        {
                            Thread.CurrentThread.Interrupt();
                        }
                        catch { }

                        try
                        {
                            // hard unsafe exit supported by older .net framework runtimes
                            Thread.CurrentThread.Abort();
                        }
                        catch { }
                    }))
                        try
                        {
                            action();
                        }
                        catch (ObjectDisposedException)
                        {
                        }
                        catch (TaskCanceledException)
                        {
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        catch (ThreadAbortException)
                        {
                        }
                        catch
                        {
                            throw;
                        }

                }, token).GetAwaiter().GetResult();
            }
            catch (ObjectDisposedException)
            {
            }
            catch (TaskCanceledException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (ThreadAbortException)
            {
            }
            catch
            {
                throw;
            }
        }


        public static void CancellableRunAsync(Action action, CancellationTokenSource cts)
        {
            CancellableRunAsync(action, cts.Token);
        }
        private static int async_action_count = 0;
        public static void CancellableRunAsync(Action action, CancellationToken token)
        {
            try
            {
                Task.Run(() =>
                {
                    using (var reg = token.Register(() =>
                    {
                        try
                        {
                            Thread.CurrentThread.Interrupt();
                        }
                        catch { }

                        try
                        {
                            // hard unsafe exit supported by older .net framework runtimes
                            Thread.CurrentThread.Abort();
                        }
                        catch 
                        {
                        }
                    }))

                        try
                    {
                        action();
                    }
                    catch (ObjectDisposedException)
                        {
                        }
                        catch (TaskCanceledException)
                        {
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        catch (ThreadAbortException)
                        {
                        }
                        catch
                        {
                            throw;
                        }

                }, token).ConfigureAwait(false);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (TaskCanceledException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (ThreadAbortException)
            {
            }
            catch
            {
                throw;
            }
        }


    }
}
