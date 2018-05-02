namespace raylib
{
  public class Renderer
  {
    private readonly RenderData _renderData;
    private readonly bool _useExTracer;

    public Renderer(RenderData renderData, bool useEx)
    {
      _renderData = renderData;
      _useExTracer = useEx;
    }

    private IRayTracer CreateRayTracer(Camera camera, RenderData renderData, Scene scene, bool useKdTree)
    {
      return _useExTracer
               ? new RayTracerEx(camera, renderData, scene, useKdTree) as IRayTracer
               : new RayTracer(camera, renderData, scene, useKdTree) as IRayTracer;
    }

    public PixelArray Render(Camera camera, Scene scene, bool useKdTree)
    {
      if (_renderData.NumThreads <= 1)
      {
        return RenderSingleThreaded(camera, scene, useKdTree);
      }

      return RenderMultiThreaded(camera, scene, useKdTree);
    }

    private PixelArray RenderSingleThreaded(Camera camera, Scene scene, bool useKdTree)
    {
      var pixelArray = new PixelArray(_renderData.Width, _renderData.Height);
      var tracer = CreateRayTracer(camera, _renderData, scene, useKdTree);

      for (var y = 0; y < _renderData.Height; y++)
      {
        for (var x = 0; x < _renderData.Width; x++)
        {
          var color = tracer.GetPixelColor(x, y);
          pixelArray.SetPixelColor(x, y, color);
        }
      }

      return pixelArray;
    }

    private PixelArray RenderMultiThreaded(Camera camera, Scene scene, bool useKdTree)
    {
      return PerLineThreadedRenderer.Render(CreateRayTracer(camera, _renderData, scene, useKdTree));
    }
  }
}