using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace raylib
{
    public class LogTimer : IDisposable
    {
      private readonly Stopwatch _sw;
      private readonly string _message;

      public LogTimer(string message)
      {
        _message = message;
        _sw = Stopwatch.StartNew();
      }

      /// <inheritdoc />
      public void Dispose()
      {
        _sw.Stop();
        Console.WriteLine($"{_message} took {_sw.ElapsedMilliseconds}ms");
      }
    }
}
