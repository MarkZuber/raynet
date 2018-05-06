// -----------------------------------------------------------------------
// <copyright file="RenderService.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
using raylib;

namespace rayapp
{
  public interface IRenderService
  {
    RenderDataModel GetRenderDataModel();
    MemoryStream GetCurrentImagePngStream();
  }

  public class RenderDataModel
  {
    public string RenderName { get; set; }
    public string ImageRelativeUrl { get; set; }
    public bool IsRendering { get; set; }
    public int PercentComplete { get; set; }
  }

  public class RenderService : IRenderService
  {
    private readonly RenderDataModel _renderDataModel;
    private PixelArray _pixelArray;
    private Renderer _renderer;
    private Scene _scene;
    private Camera _camera;
    private readonly RenderData _renderData = new RenderData(500, 500, 5, 2, true);

    public RenderService()
    {
      _renderDataModel = new RenderDataModel();

      _scene = SceneFactory.CreateBasicScene();
      _camera = new Camera(
        new PosVector(7.5, 7.5, 2.3),
        new PosVector(0.0, 0.0, 0.0),
        new PosVector(0.0, 0.0, 1.0),
        50.0);

      _renderer = new Renderer(_renderData, false);
      _pixelArray = new PixelArray(_renderData.Width, _renderData.Height);
      Task.Run(() =>
      {
        while (true)
        {
          _renderer.Render(_pixelArray, _camera, _scene, true);
        }
      });
    }

    public RenderDataModel GetRenderDataModel()
    {
      return _renderDataModel;
    }

    public MemoryStream GetCurrentImagePngStream()
    {
      return _pixelArray.SaveToStreamAsPng();
    }
  }
}