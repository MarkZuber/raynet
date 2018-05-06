// -----------------------------------------------------------------------
// <copyright file="MockLiveDataService.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace rayapp
{
  public interface ILiveDataService
  {
    IObservable<RenderDataModel> ImageRender { get; }
  }

  public class MockLiveDataService : ILiveDataService
  {
    private readonly Random _random = new Random();

    public MockLiveDataService(IRenderService renderService)
    {
      var images = new List<string>
      {
        "https://s3.envato.com/files/131529625/15_Confetti_backgrounds%20-screen/Confetti_background-11.jpg",
        "https://tse2.mm.bing.net/th?id=OIP.dqMe78yE0C3cFnVoGQa_aQHaFj&pid=Api",
        "http://3.bp.blogspot.com/-xLgRRD58ifc/U7K0D1XEWqI/AAAAAAAAOEs/_RQEMUnV2A0/s1600/Desktop+Background+wallpapers+(23).jpg"
      };

      ImageRender = Observable.Interval(TimeSpan.FromSeconds(1)).StartWith(0)
                              .Select(_ => GetRenderStatus(renderService)).Select(
                                renderDataModel => new RenderDataModel
                                {
                                  RenderName = renderDataModel.RenderName,
                                  IsRendering = renderDataModel.IsRendering,
                                  PercentComplete = _random.Next(1, 100),
                                  ImageRelativeUrl = "http://localhost:59248/imgrend" // images[_random.Next(0, images.Count)]
                                });
    }

    public IObservable<RenderDataModel> ImageRender { get; }

    private RenderDataModel GetRenderStatus(IRenderService renderService)
    {
      RenderDataModel renderDataModel = renderService.GetRenderDataModel();
      return renderDataModel;
    }
  }
}