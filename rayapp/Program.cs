using System;
using raylib;

namespace rayapp
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      Console.WriteLine("RayApp...");

      bool useKdTree = true;
      bool renderBasic = false;
      bool renderBalls = false;
      bool renderJacks = true;

      if (renderBasic)
      {
        var scene = SceneFactory.CreateBasicScene();
        var camera = new Camera(
          new PosVector(7.5, 7.5, 2.3),
          new PosVector(0.0, 0.0, 0.0),
          new PosVector(0.0, 0.0, 1.0),
          50.0);
        var renderData = new RenderData(1500, 1500, 5, 8, true);

        var renderer = new Renderer(renderData);
        using (new LogTimer("Render BasicScene"))
        {
          var pixelArray = renderer.Render(camera, scene, useKdTree);
          pixelArray.SaveAsFile(@"e:\basicscene.png");
        }
      }

      if (renderBalls)
      {
        var nffResult = NffParser.ParseFile(@"E:\repos\rustray\nff\balls1.nff", 8, 5, 500, 500);

        var renderer = new Renderer(nffResult.RenderData);
        using (new LogTimer("Render Balls"))
        {
          var pixelArray = renderer.Render(nffResult.Camera, nffResult.Scene, useKdTree);
          pixelArray.SaveAsFile(@"e:\balls1.png");
        }
      }

      if (renderJacks)
      {
        var nffResult = NffParser.ParseFile(@"E:\repos\rustray\nff\jacks4.nff", 8, 5, 500, 500);

        var renderer = new Renderer(nffResult.RenderData);
        using (new LogTimer("Render Jacks"))
        {
          var pixelArray = renderer.Render(nffResult.Camera, nffResult.Scene, useKdTree);
          pixelArray.SaveAsFile(@"e:\jacks4.png");
        }
      }

      Console.WriteLine("DONE!");
    }
  }
}