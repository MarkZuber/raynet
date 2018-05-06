// -----------------------------------------------------------------------
// <copyright file="Renderer.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace raylib
{
  public class RenderProgressEventArgs : EventArgs
  {
    public RenderProgressEventArgs(double percentComplete)
    {
      PercentComplete = percentComplete;
    }

    public double PercentComplete { get; }
  }

  public class Renderer
  {
    private readonly RenderData _renderData;
    private readonly bool _useExTracer;

    public Renderer(RenderData renderData, bool useEx)
    {
      _renderData = renderData;
      _useExTracer = useEx;
    }

    public event EventHandler<RenderProgressEventArgs> Progress;

    private IRayTracer CreateRayTracer(Camera camera, RenderData renderData, Scene scene, bool useKdTree)
    {
      return _useExTracer
               ? new RayTracerEx(camera, renderData, scene, useKdTree) as IRayTracer
               : new RayTracer(camera, renderData, scene, useKdTree) as IRayTracer;
    }

    public void Render(PixelArray pixelArray, Camera camera, Scene scene, bool useKdTree)
    {
      Progress?.Invoke(this, new RenderProgressEventArgs(0.0));

      if (_renderData.NumThreads <= 1)
      {
        RenderSingleThreaded(pixelArray, camera, scene, useKdTree);
      }

      RenderMultiThreaded(pixelArray, camera, scene, useKdTree);
    }

    private void RenderSingleThreaded(PixelArray pixelArray, Camera camera, Scene scene, bool useKdTree)
    {
      var tracer = CreateRayTracer(camera, _renderData, scene, useKdTree);

      for (var y = 0; y < _renderData.Height; y++)
      {
        for (var x = 0; x < _renderData.Width; x++)
        {
          var color = tracer.GetPixelColor(x, y);
          pixelArray.SetPixelColor(x, y, color);
        }

        Progress?.Invoke(
          this,
          new RenderProgressEventArgs((Convert.ToDouble(y) * 100.0) / Convert.ToDouble(_renderData.Height)));
      }
    }

    private void RenderMultiThreaded(PixelArray pixelArray, Camera camera, Scene scene, bool useKdTree)
    {
      var rayTracer = CreateRayTracer(camera, _renderData, scene, useKdTree);
      ThreadPool.SetMinThreads(rayTracer.RenderData.NumThreads * 3, rayTracer.RenderData.NumThreads * 3);

      var queueDataAvailableEvent = new AutoResetEvent(false);
      var rowQueue = new ConcurrentQueue<int>();
      var resultQueue = new ConcurrentQueue<RenderLineResult>();

      for (var y = 0; y < rayTracer.RenderData.Height; y++)
      {
        rowQueue.Enqueue(y);
      }

      var tasks = new List<Task>();

      for (var thid = 0; thid < rayTracer.RenderData.NumThreads; thid++)
      {
        tasks.Add(Task.Run(() => RenderFunc(rayTracer, rowQueue, resultQueue, queueDataAvailableEvent)));
      }

      tasks.Add(Task.Run(() => ResultFunc(pixelArray, resultQueue, queueDataAvailableEvent)));

      Task.WaitAll(tasks.ToArray());
    }

    private static void RenderFunc(
      IRayTracer rayTracer,
      ConcurrentQueue<int> rowQueue,
      ConcurrentQueue<RenderLineResult> resultQueue,
      AutoResetEvent queueDataAvailableEvent)
    {
      while (rowQueue.TryDequeue(out var y))
      {
        // Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} starting to render row: {y}");
        var rowPixels = new List<ColorVector>();
        for (var x = 0; x < rayTracer.RenderData.Width; x++)
        {
          rowPixels.Add(rayTracer.GetPixelColor(x, y));
        }

        resultQueue.Enqueue(new RenderLineResult(y, rowPixels));
        queueDataAvailableEvent.Set();
      }
    }

    private void ResultFunc(
      PixelArray pixelArray,
      ConcurrentQueue<RenderLineResult> resultQueue,
      AutoResetEvent queueDataAvailableEvent)
    {
      var incompleteRows = new HashSet<int>();
      for (var y = 0; y < pixelArray.Height; y++)
      {
        incompleteRows.Add(y);
      }

      while (incompleteRows.Count > 0)
      {
        queueDataAvailableEvent.WaitOne();

        while (resultQueue.TryDequeue(out var renderLineResult))
        {
          // assert pixelArray.Width == renderLineResult.Count
          for (var x = 0; x < pixelArray.Width; x++)
          {
            pixelArray.SetPixelColor(x, renderLineResult.Y, renderLineResult.RowPixels[x]);
          }

          incompleteRows.Remove(renderLineResult.Y);

          var totalRows = Convert.ToDouble(pixelArray.Height);
          var completeRows = Convert.ToDouble(pixelArray.Height - incompleteRows.Count);
          var percentComplete = completeRows / totalRows * 100.0;
          Console.WriteLine($"Percent Complete: {percentComplete:F}%");

          Progress?.Invoke(
            this,
            new RenderProgressEventArgs(percentComplete));
        }
      }
    }

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
  }
}