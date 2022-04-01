using System;
using System.Diagnostics;

namespace ProxySharp
{
    internal static class ExceptionExtensions
    {
        public static void LogError(this Exception error)
        {
            Trace.WriteLine(DateTime.Now.ToString("G"));
            Trace.WriteLine(error.Message);
            Trace.WriteLine(error.StackTrace);
            Trace.WriteLine("---END---");
            Trace.WriteLine(string.Empty);
        }
    }
}
