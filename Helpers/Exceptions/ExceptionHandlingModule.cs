using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Generic.Helpers.Exceptions
{
    public class ExceptionHandlingModule : IHttpModule
    {
        private ILoggingService _logger;
        private IErrorNotificationService _notification;

        public void Init(HttpApplication application)
        {
            application.Error += OnError;
        }

        public void Dispose()
        {
            // Nothing to dispose
        }

        private void OnError(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var context = application.Context;
            var exception = application.Server.GetLastError();

            if (exception == null)
                return;

            // Ensure logger is initialized
            if (_logger == null)
            {
                _logger = DependencyResolver.Current.GetService<ILoggingService>();
            }
            if (_notification == null)
            {
                _notification = DependencyResolver.Current.GetService<IErrorNotificationService>();
            }

            // Log the exception
            _logger.LogError(
                exception,
                "Unhandled exception in application: {Message}",
                exception.Message);
            if (exception is OutOfMemoryException ||
                exception is StackOverflowException ||
                exception is StackOverflowException ||
                exception is AccessViolationException ||
                exception is AppDomainUnloadedException ||
                exception is ThreadAbortException ||
                exception is TypeInitializationException ||
                exception is BadImageFormatException ||
                exception is SEHException)
            {
                _logger.LogCritical(
                    exception,
                    "Critical error occurred: {Message}",
                    exception.Message);
                SendErrorNotification(exception);
            }
        }

        private void SendErrorNotification(Exception exception)
        {
            _notification.NotifyAdministrators(exception, null);
        }
    }
}