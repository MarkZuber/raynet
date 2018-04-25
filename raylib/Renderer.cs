namespace raylib
{
  public class Renderer
  {
    private readonly RenderData _renderData;

    public Renderer(RenderData renderData)
    {
      _renderData = renderData;
    }

    public PixelArray Render(Camera camera, Scene scene)
    {
      if (_renderData.NumThreads <= 1)
      {
        return RenderSingleThreaded(camera, scene);
      }

      return RenderMultiThreaded(camera, scene);
    }

    private PixelArray RenderSingleThreaded(Camera camera, Scene scene)
    {
      var pixelArray = new PixelArray(_renderData.Width, _renderData.Height);
      var tracer = new RayTracer(camera, _renderData, scene);

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

    private PixelArray RenderMultiThreaded(Camera camera, Scene scene)
    {
      return PerLineThreadedRenderer.Render(new RayTracer(camera, _renderData, scene));
    }
  }
}