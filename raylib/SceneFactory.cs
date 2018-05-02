using System;
using System.Collections.Generic;

namespace raylib
{
  public static class SceneFactory
  {
    public static Scene CreateBasicScene()
    {
      var background = new Background(new ColorVector(0.2, 0.2, 0.2), 0.2);

      var shapes = new List<Shape>();

      // right most sphere: purple
      shapes.Add(new SphereShape(
        new PosVector(2.5, 5.0, 1.0),
        0.75,
        new SolidMaterial(
          0.6,
          0.2,
          0.0,
          0.0,
          0.0,
          0.0,
          0.0,
          new ColorVector(1.0, 0.0, 1.0)
        )));

      // left most sphere: red
      shapes.Add(new SphereShape(
        new PosVector(3.5, 1.25, 1.5),
        1.0,
        new SolidMaterial(
          0.6,
          0.2,
          0.0,
          0.0,
          0.0,
          0.0,
          0.0,
          new ColorVector(1.0, 1.0, 0.0)
        )));

      // middle sphere: cyan
      shapes.Add(new SphereShape(
        new PosVector(2.0, 3.0, 1.0),
        1.0,
        new SolidMaterial(
          0.6,
          0.2,
          0.0,
          0.0,
          0.0,
          0.0,
          0.0,
          new ColorVector(0.0, 1.0, 1.0)
        )));

      // bottom plane:  green
      shapes.Add(new PlaneShape(
        new PosVector(0.0, 0.0, 1.0),
        0.0,
        new SolidMaterial(
          0.6,
          0.2,
          0.0,
          0.0,
          0.0,
          0.0,
          0.0,
          new ColorVector(0.0, 1.0, 0.0)
        )));

      // right plane:  blue
      shapes.Add(new PlaneShape(
        new PosVector(1.0, 0.0, 0.0),
        0.0,
        new SolidMaterial(
          0.6,
          0.2,
          0.0,
          0.0,
          0.0,
          0.0,
          0.0,
          new ColorVector(0.0, 0.0, 1.0)
        )));

      // left plane: red
      shapes.Add(new PlaneShape(
        new PosVector(0.0, 1.0, 0.0),
        0.0,
        new SolidMaterial(
          0.6,
          0.2,
          0.2,
          0.0,
          0.0,
          0.0,
          0.0,
          new ColorVector(1.0, 0.0, 0.0)
        )));

      var lights = new List<Light>
      {
        new PointLight(
          new PosVector(100.0,60.0, 40.0),
          new ColorVector(1.0, 1.0, 1.0)
        )
      };

      return Scene.Create(background, shapes, lights);
    }

    public static Scene CreateMarblesAxisScene()
    {
      var background = new Background(new ColorVector(), 0.0);

      var redMaterial = new SolidMaterial(0.5, 0.2, 0.0, 0.6, 0.0, 0.0, 1.0, new ColorVector(0.9, 0.0, 0.0));
      var greenMaterial = new SolidMaterial(0.5, 0.2, 0.0, 0.6, 0.0, 0.0, 1.0, new ColorVector(0.0, 0.9, 0.0));
      var blueMaterial = new SolidMaterial(0.5, 0.2, 0.0, 0.6, 0.0, 0.0, 1.0, new ColorVector(0.0, 0.0, 0.9));

      double sphereDistanceIncrement = 5.0;
      int numSpheresPerAxis = 10;
      double sphereRadius = 2.0;

      double maxAxis = sphereDistanceIncrement * Convert.ToDouble(numSpheresPerAxis);

      var shapes = new List<Shape>();

      double x = 0.0;
      while (x <= maxAxis)
      {
        shapes.Add(new SphereShape(new PosVector(x, 0.0, 0.0), sphereRadius, redMaterial));
        x += sphereDistanceIncrement;
      }

      double y = 0.0;
      while (y <= maxAxis)
      {
        shapes.Add(new SphereShape(new PosVector(0.0, y, 0.0), sphereRadius, greenMaterial));
        y += sphereDistanceIncrement;
      }

      double z = 0.0;
      while (z <= maxAxis)
      {
        shapes.Add(new SphereShape(new PosVector(0.0, 0.0, z), sphereRadius, blueMaterial));
        z += sphereDistanceIncrement;
      }

      var chessMaterial = new ChessboardMaterial(0.5, 0.2, 0.0, 0.2, 0.0, 0.0, 2.0, new ColorVector(0.8, 0.8, 0.8), new ColorVector(), 15.0);

      shapes.Add(new PlaneShape(new PosVector(0.0, 0.0, 1.0), 0.0, chessMaterial));

      var lights = new List<Light>();
      lights.Add(new PointLight(new PosVector(5.0, 10.0, 10.0), new ColorVector(0.9, 0.9, 0.9)));

      return Scene.Create(background, shapes, lights);
    }
  }
}