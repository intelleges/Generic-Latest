using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Helpers
{
      public class SerilogLoggingService : ILoggingService
        {
           
            public void LogTrace(string message, params object[] args)
            {
                Log.Verbose(message, args);
            }

            public void LogDebug(string message, params object[] args)
            {
                Log.Debug(message, args);
            }

            public void LogInformation(string message, params object[] args)
            {
                Log.Information(message, args);
            }

            public void LogWarning(string message, params object[] args)
            {
                Log.Warning(message, args);
            }

            public void LogError(Exception exception, string message, params object[] args)
            {
                Log.Error(exception, message, args);
            }

            public void LogCritical(Exception exception, string message, params object[] args)
            {
                Log.Fatal(exception, message, args);
            }
        }
    
}