using System.Collections.Generic;
using System.IO;

namespace raylib
{
  public class NffParserResult
  {
    public NffParserResult(Scene scene, RenderData renderData, Camera camera)
    {
      Scene = scene;
      RenderData = renderData;
      Camera = camera;
    }

    public Scene Scene { get; }
    public Camera Camera { get; }
    public RenderData RenderData { get; }
  }

  public static class NffParser
  {
    public enum LookingFor
    {
      Instruction,
      ViewpointFrom,
      ViewpointAt,
      ViewpointUp,
      ViewpointAngle,
      ViewpointHither,
      ViewpointResolution,
      Polygon
    }

    public static NffParserResult ParseFile(string path, int numThreads, int rayTraceDepth, int resolutionX,
      int resolutionY)
    {
      var background = new Background(new ColorVector(0.0, 0.0, 0.0), 0.0);
      var lights = new List<Light>();
      var shapes = new List<Shape>();
      var cameraAt = PosVector.NewDefault();
      var cameraFrom = PosVector.NewDefault();
      var cameraUp = PosVector.NewDefault();

      var lookingFor = LookingFor.Instruction;

      var currentMaterial = new SolidMaterial(0.0, 0.0, 0.0, 0.0, new ColorVector(0.0, 0.0, 0.0));

      var polyVectors = new List<PosVector>();
      var currentItemCounter = 0;

      var lines = File.ReadAllLines(path);

      foreach (var line in lines)
      {
        var split = line.Split(' ', '\t');

        switch (lookingFor)
        {
          case LookingFor.Instruction:
          {
            var instruction = split[0];

            if (instruction == "b")
            {
              // background color
              background =
                new Background(new ColorVector(double.Parse(split[1]), double.Parse(split[2]), double.Parse(split[3])),
                  0.0);
            }
            else if (instruction == "v")
            {
              // viewpoint location
              lookingFor = LookingFor.ViewpointFrom;
            }
            else if (instruction == "l")
            {
              // positional light
              var colorVector = split.Length == 7
                ? new ColorVector(double.Parse(split[4]), double.Parse(split[5]), double.Parse(split[6]))
                : new ColorVector(1.0, 1.0, 1.0);
              lights.Add(
                new PointLight(new PosVector(double.Parse(split[1]), double.Parse(split[2]), double.Parse(split[3])),
                  colorVector));
            }
            else if (instruction == "f")
            {
              // println!("reading f: {}", num);
              // object material properties
              // "f" red green blue Kd Ks Shine T index_of_refraction
              // Kd Diffuse component
              // Ks Specular
              // Shine Phong cosine power for highlights
              // T Transmittance (fraction of contribution of the transmitting ray).
              // Usually, 0 <= Kd <= 1 and 0 <= Ks <= 1, though it is not required that Kd + Ks = 1. Note that transmitting objects (T > 0) are considered to have two sides for algorithms that need these (normally, objects have one side).
              // todo: i don't think i'm assigning the correct values into my solidmaterial yet
              currentMaterial = new SolidMaterial(
                double.Parse(split[6]),
                double.Parse(split[5]),
                double.Parse(split[8]),
                double.Parse(split[7]),
                new ColorVector(double.Parse(split[1]), double.Parse(split[2]), double.Parse(split[3]))
              );
            }
            else if (instruction == "c")
            {
              // cone or cylinder
            }
            else if (instruction == "s")
            {
              // sphere
              shapes.Add(new SphereShape(
                new PosVector(double.Parse(split[1]), double.Parse(split[2]), double.Parse(split[3])),
                double.Parse(split[4]),
                currentMaterial
              ));
            }
            else if (instruction == "p")
            {
              // polygon
              currentItemCounter = int.Parse(split[1]);
              polyVectors = new List<PosVector>();
              lookingFor = LookingFor.Polygon;
            }
            else if (instruction == "pp")
            {
              // polygon patch
            }
            else if (instruction == "#")
            {
              // comment
            }
          }
            break;
          case LookingFor.Polygon:
          {
            if (currentItemCounter > 0)
            {
              currentItemCounter--;
              polyVectors.Add(new PosVector(double.Parse(split[0]), double.Parse(split[1]), double.Parse(split[2])));
            }

            if (currentItemCounter == 0)
            {
              if (polyVectors.Count >= 3)
              {
                var firstVert = polyVectors[0];
                var prevVert = polyVectors[1];
                var thisVert = polyVectors[2];
                shapes.Add(new TriangleShape(firstVert, prevVert, thisVert, currentMaterial, currentMaterial));

                for (var i = 3; i < polyVectors.Count; i++)
                {
                  prevVert = thisVert;
                  thisVert = polyVectors[i];
                  shapes.Add(new TriangleShape(firstVert, prevVert, thisVert, currentMaterial, currentMaterial));
                }
              }

              lookingFor = LookingFor.Instruction;
            }
          }
            break;
          case LookingFor.ViewpointFrom:
          {
            cameraFrom = new PosVector(double.Parse(split[1]), double.Parse(split[2]), double.Parse(split[3]));
            lookingFor = LookingFor.ViewpointAt;
          }
            break;
          case LookingFor.ViewpointAt:
          {
            cameraAt = new PosVector(double.Parse(split[1]), double.Parse(split[2]), double.Parse(split[3]));
            lookingFor = LookingFor.ViewpointUp;
          }
            break;
          case LookingFor.ViewpointUp:
          {
            cameraUp = new PosVector(double.Parse(split[1]), double.Parse(split[2]), double.Parse(split[3]));
            lookingFor = LookingFor.ViewpointAngle;
          }
            break;
          case LookingFor.ViewpointAngle:
          {
            // todo: implement
            lookingFor = LookingFor.ViewpointHither;
          }
            break;
          case LookingFor.ViewpointHither:
          {
            // todo: implement
            lookingFor = LookingFor.ViewpointResolution;
          }
            break;
          case LookingFor.ViewpointResolution:
          {
            //resolutionX = int.Parse(split[1]);
            //resolutionY = int.Parse(split[2]);

            lookingFor = LookingFor.Instruction;
          }
            break;
        }
      }

      return new NffParserResult(Scene.Create(background, shapes, lights),
        new RenderData(resolutionX, resolutionY, rayTraceDepth, numThreads, true),
        new Camera(cameraFrom, cameraAt, cameraUp, 50.0));
    }
  }
}