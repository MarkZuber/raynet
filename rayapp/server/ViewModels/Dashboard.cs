// -----------------------------------------------------------------------
// <copyright file="Dashboard.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reactive.Linq;
using DotNetify;
using DotNetify.Routing;
using DotNetify.Security;

namespace rayapp
{
  [Authorize]
  public class Dashboard : BaseVM,
                           IRoutable
  {
    private readonly IDisposable _subscription;

    public Dashboard(ILiveDataService liveDataService)
    {
      AddProperty<RenderDataModel>("ImageRender").SubscribeTo(liveDataService.ImageRender);

      // Regulate data update interval to no less than every 200 msecs.
      _subscription = Observable.Interval(TimeSpan.FromMilliseconds(200)).StartWith(0)
                                .Subscribe(_ => PushUpdates());
    }

    public RoutingState RoutingState { get; set; }

    public override void Dispose()
    {
      _subscription?.Dispose();
      base.Dispose();
    }
  }
}