using System;
using raylib;

namespace rayapp
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      Console.WriteLine("RayApp...");

      var scene = SceneFactory.CreateBasicScene();
      var camera = new Camera(new PosVector(7.5, 7.5, 2.3), new PosVector(0.0, 0.0, 0.0), new PosVector(0.0, 0.0, 1.0),
        50.0);
      var renderData = new RenderData(1500, 1500, 5, 1, true);
      var renderer = new Renderer(renderData);
      using (new LogTimer("Render BasicScene"))
      {
        var pixelArray = renderer.Render(camera, scene);
        pixelArray.SaveAsFile(@"e:\basicscene.png");
      }

      Console.WriteLine("DONE!");
    }
  }
}