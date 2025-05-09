using Generic.Helpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Generic.Helpers.Utility
{
    public static class ExceptionHandler
    {
        public static T Execute<T>(Func<T> action, ILoggingService logger, string errorMessage = null)
        {
            try
            {
                return action();
            }
            catch (BusinessException ex)
            {
                // Log business exceptions as warnings
                logger.LogWarning(errorMessage ?? ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                // Log other exceptions as errors
                logger.LogError(ex, errorMessage ?? "An error occurred while executing the operation.");
                throw;
            }
        }
        

        public static void Execute(Action action, ILoggingService logger, string errorMessage = null)
        {
            try
            {
                action();
            }
            catch (BusinessException ex)
            {
                // Log business exceptions as warnings
                logger.LogWarning(errorMessage ?? ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                // Log other exceptions as errors
                logger.LogError(ex, errorMessage ?? "An error occurred while executing the operation.");
                throw;
            }
        }

        public static async Task<T> ExecuteAsync<T>(Func<Task<T>> action, ILoggingService logger, string errorMessage = null)
        {
            try
            {
                return await action();
            }
            catch (BusinessException ex)
            {
                // Log business exceptions as warnings
                logger.LogWarning(errorMessage ?? ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                // Log other exceptions as errors
                logger.LogError(ex, errorMessage ?? "An error occurred while executing the operation.");
                throw;
            }
        }

        public static async Task ExecuteAsync(Func<Task> action, ILoggingService logger, string errorMessage = null)
        {
            try
            {
                await action();
            }
            catch (BusinessException ex)
            {
                // Log business exceptions as warnings
                logger.LogWarning(errorMessage ?? ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                // Log other exceptions as errors
                logger.LogError(ex, errorMessage ?? "An error occurred while executing the operation.");
                throw;
            }
        }
     }
}