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
          0.0,
          0.0,
          0.0,
          0.0,
          new ColorVector(1.0, 0.0, 0.0)
        )));

      var lights = new List<Light>
      {
        new PointLight(
          new PosVector(100.0, 60.0, 40.0),
          new ColorVector(1.0, 1.0, 1.0)
        )
      };

      return Scene.Create(background, shapes, lights);
    }
  }
}