// -----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace rayapp.server.Controllers
{
  public class HomeController : Controller
  {
    private readonly IRenderService _renderService;

    public HomeController(IRenderService renderService)
    {
      _renderService = renderService;
    }

    [Route("imgrend")]
    public ActionResult CurrentImage()
    {
      return File(_renderService.GetCurrentImagePngStream(), "image/png");
    }
  }
}