// -----------------------------------------------------------------------
// <copyright file="AppLayout.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using DotNetify;
using DotNetify.Routing;
using DotNetify.Security;

namespace rayapp
{
  [Authorize]
  public class AppLayout : BaseVM,
                           IRoutable
  {
    public AppLayout(IPrincipalAccessor principalAccessor)
    {
      var userIdentity = principalAccessor.Principal.Identity as ClaimsIdentity;

      UserName = userIdentity.Name;
      UserAvatar = userIdentity.Claims.FirstOrDefault(i => i.Type == ClaimTypes.Uri)?.Value;

      this.RegisterRoutes(
        "/",
        new List<RouteTemplate>
        {
          new RouteTemplate(nameof(Route.Home))
          {
            UrlPattern = "",
            ViewUrl = nameof(Route.Dashboard)
          },
          new RouteTemplate(nameof(Route.Dashboard)),
          new RouteTemplate(nameof(Route.FormPage))
          {
            UrlPattern = $"{FormPagePath}(/:id)"
          },
          new RouteTemplate(nameof(Route.TablePage))
        });
    }

    public static string FormPagePath => "Form";

    public object Menus =>
      new List<object>()
      {
        new
        {
          Title = "Dashboard",
          Icon = "assessment",
          Route = this.GetRoute(nameof(Route.Dashboard))
        }
      };

    public string UserName { get; set; }
    public string UserAvatar { get; set; }

    public RoutingState RoutingState { get; set; }

    private enum Route
    {
      Home,
      Dashboard,
      FormPage,
      TablePage
    };
  }
}