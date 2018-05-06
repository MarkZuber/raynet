// -----------------------------------------------------------------------
// <copyright file="RenderService.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

namespace rayapp
{
  public interface IRenderService
  {
    RenderDataModel GetRenderDataModel();
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

    public RenderService()
    {
      _renderDataModel = new RenderDataModel();
    }

    public RenderDataModel GetRenderDataModel()
    {
      return _renderDataModel;
    }
  }
}