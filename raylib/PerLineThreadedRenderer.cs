using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace raylib
{
  public static class PerLineThreadedRenderer
  {
    private class RenderLineResult
    {
      public RenderLineResult(int y, List<ColorVector> rowPixels)
      {
        Y = y;
        RowPixels = rowPixels;
      }

      public List<ColorVector> RowPixels { get; }
      public int Y { get; }      
    }

    public static PixelArray Render(RayTracer rayTracer)
    {
      ThreadPool.SetMinThreads(rayTracer.RenderData.NumThreads * 3, rayTracer.RenderData.NumThreads * 3);

      var queueDataAvailableEvent = new AutoResetEvent(false);
      var rowQueue = new ConcurrentQueue<int>();
      var resultQueue = new ConcurrentQueue<RenderLineResult>();

      for (int y = 0; y < rayTracer.RenderData.Height; y++)
      {
        rowQueue.Enqueue(y);
      }

      var tasks = new List<Task>();

      for (int thid = 0; thid < rayTracer.RenderData.NumThreads; thid++)
      {
        tasks.Add(Task.Run(() => RenderFunc(rayTracer, rowQueue, resultQueue, queueDataAvailableEvent)));
      }

      var pixelArray = new PixelArray(rayTracer.RenderData.Width, rayTracer.RenderData.Height);

      tasks.Add(Task.Run(() => ResultFunc(pixelArray, resultQueue, queueDataAvailableEvent)));

      Task.WaitAll(tasks.ToArray());

      return pixelArray;
    }

    private static void RenderFunc(RayTracer rayTracer, ConcurrentQueue<int> rowQueue,
      ConcurrentQueue<RenderLineResult> resultQueue, AutoResetEvent queueDataAvailableEvent)
    {
      while (rowQueue.TryDequeue(out int y))
      {
        // Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} starting to render row: {y}");
        var rowPixels = new List<ColorVector>();
        for (int x = 0; x < rayTracer.RenderData.Width; x++)
        {
          rowPixels.Add(rayTracer.GetPixelColor(x, y));
        }
        resultQueue.Enqueue(new RenderLineResult(y, rowPixels));
        queueDataAvailableEvent.Set();
      }
    }

    private static void ResultFunc(PixelArray pixelArray, ConcurrentQueue<RenderLineResult> resultQueue, AutoResetEvent queueDataAvailableEvent)
    {
      var incompleteRows = new HashSet<int>();
      for (int y = 0; y < pixelArray.Height; y++)
      {
        incompleteRows.Add(y);
      }

      while (incompleteRows.Count > 0)
      {
        queueDataAvailableEvent.WaitOne();

        while (resultQueue.TryDequeue(out var renderLineResult))
        {
          // assert pixelArray.Width == renderLineResult.Count
          for (int x = 0; x < pixelArray.Width; x++)
          {
            pixelArray.SetPixelColor(x, renderLineResult.Y, renderLineResult.RowPixels[x]);
          }

          incompleteRows.Remove(renderLineResult.Y);

          var totalRows = Convert.ToDouble(pixelArray.Height);
          var completeRows = Convert.ToDouble(pixelArray.Height - incompleteRows.Count);
          double percentComplete = (completeRows / totalRows) * 100.0;
          Console.WriteLine($"Percent Complete: {percentComplete:F}%");
        }
      }
    }
  }
}
