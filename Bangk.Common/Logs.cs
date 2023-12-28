using Serilog;
using System;
using System.Runtime.CompilerServices;

namespace Bangk.Common
{
    public static class Logs
    {
        public static void WriteInfoLog(string message, [CallerMemberName] string callerName = "")
        {
            Log.Information($"Called Method: [{callerName}] - Message: [{message}].");
        }

        public static void WriteErrorLog(string message, Exception exception = null, [CallerMemberName] string callerName = "")
        {
            Log.Error(exception, $"Called Method: [{callerName}] - Message: [{message}].");
        }

        public static void WriteWarningLog(string message, [CallerMemberName] string callerName = "")
        {
            Log.Warning($"Called Method: [{callerName}] - Message: [{message}].");
        }
    }
}
