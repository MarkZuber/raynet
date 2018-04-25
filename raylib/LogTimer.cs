using System;
using System.Diagnostics;

namespace raylib
{
  public class LogTimer : IDisposable
  {
    private readonly string _message;
    private readonly Stopwatch _sw;

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