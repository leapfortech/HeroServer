using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeroServer
{
    public static class TaskHelper
    {
        public static void Forget(this Task task)
        {
            task.ContinueWith(task => { Console.WriteLine(task.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public static void Forget(this Task task, Action<Task> ContinuationAction)
        {
            task.ContinueWith(ContinuationAction, TaskContinuationOptions.OnlyOnFaulted);
        }

        public static async void Forget(this Task task, Action<Task> ContinuationAction, List<Type> acceptableExceptions)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!acceptableExceptions.Contains(ex.GetType()))    // Consider whether derived types are also acceptable or not
                    throw;

                ContinuationAction(task);
            }
        }
    }
}