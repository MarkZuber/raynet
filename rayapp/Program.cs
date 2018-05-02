﻿using System;
using System.IO;
using raylib;

namespace rayapp
{
  public class Harness
  {
    public Harness()
    {
      UseKdTree = true;
      UseExTracer = false;
      RenderData = new RenderData(500, 500, 5, 8, true);
      NffDirectory = @"E:\repos\rustray\nff";
      OutputDirectory = @"e:\render_out";
    }

    public bool UseKdTree { get; set; }
    public bool UseExTracer { get; set; }
    public RenderData RenderData { get; set; }
    public string OutputDirectory { get; set; }
    public string NffDirectory { get; set; }

    public void RenderBasics()
    {
      var scene = SceneFactory.CreateBasicScene();
      var camera = new Camera(
        new PosVector(7.5, 7.5, 2.3),
        new PosVector(0.0, 0.0, 0.0),
        new PosVector(0.0, 0.0, 1.0),
        50.0);
      Render(scene, camera, "basic");
    }

    public void RenderMarblesAxis()
    {
      var scene = SceneFactory.CreateMarblesAxisScene();
      var camera = new Camera(
        new PosVector(50.0, 30.0, 50.0),
        new PosVector(-0.1, 0.1, 0.0),
        new PosVector(0.0, 0.0, 1.0),
        50.0);
      Render(scene, camera, "marblesaxis");
    }

    public void RenderNff(string filename)
    {
      string filePath = Path.Combine(NffDirectory, filename);
      var nffResult = NffParser.ParseFile(filePath, 8, 5, 500, 500);
      string fileNameNoExt = Path.GetFileNameWithoutExtension(filePath);
      Render(nffResult.Scene, nffResult.Camera, fileNameNoExt);
    }

    private void Render(Scene scene, Camera camera, string name)
    {
      var renderer = new Renderer(RenderData, UseExTracer);
      using (new LogTimer($"Render {name}"))
      {
        var pixelArray = renderer.Render(camera, scene, UseKdTree);
        string filename = UseExTracer ? $"{name}_scene_ex.png" : $"{name}_scene.png";
        Directory.CreateDirectory(OutputDirectory);
        pixelArray.SaveAsFile(Path.Combine(OutputDirectory, filename));
      }
    }
  }

  internal class Program
  {
    private static void Main(string[] args)
    {
      Console.WriteLine("RayApp...");

      var harness = new Harness();
      harness.RenderBasics();
      harness.RenderMarblesAxis();
      //harness.RenderNff("balls1.nff");
      //harness.RenderNff("jacks1.nff");

      harness.UseExTracer = true;
      harness.RenderBasics();
      harness.RenderMarblesAxis();
      //harness.RenderNff("balls1.nff");
      //harness.RenderNff("jacks1.nff");

      Console.WriteLine("DONE!");
    }
  }
}