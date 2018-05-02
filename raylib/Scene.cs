// -----------------------------------------------------------------------
// <copyright file="Scene.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace raylib
{
  public class Scene
  {
    private readonly Dictionary<long, ILight> _lights;
    private readonly Dictionary<long, IShape> _shapes;

    private Scene(Background background, Dictionary<long, IShape> shapes, Dictionary<long, ILight> lights, KdTree kdTree)
    {
      Background = background;
      _shapes = shapes;
      _lights = lights;
      KdTree = kdTree;
    }

    public KdTree KdTree { get; }
    public IEnumerable<IShape> Shapes => _shapes.Values;
    public Background Background { get; }
    public IEnumerable<ILight> Lights => _lights.Values;
    public bool HasLights => _lights.Count > 0;
    public bool HasFogDensity { get; }

    public static Scene Create(Background background, IEnumerable<Shape> shapes, IEnumerable<Light> lights)
    {
      var currentShapeId = 1;

      var shapesDict = new Dictionary<long, IShape>();

      foreach (var shape in shapes)
      {
        shape.Id = currentShapeId;
        shape.BoundingBox = new BoundingBox(shape.GetMinPoint(), shape.GetMaxPoint());
        shapesDict[currentShapeId] = shape;
        currentShapeId++;
      }

      var lightsDict = new Dictionary<long, ILight>();
      var currentLightId = 1;
      foreach (var light in lights)
      {
        light.Id = currentLightId;
        lightsDict[currentLightId] = light;
        currentLightId++;
      }

      return new Scene(background, shapesDict, lightsDict, raylib.KdTree.Create(shapesDict.Values));
    }

    public bool TryGetShape(long shapeId, out IShape shape)
    {
      return _shapes.TryGetValue(shapeId, out shape);
    }

    private double ExponentialFogDensity(double distance, double k)
    {
      return 1.0 - Math.Exp(-k * distance);
    }

    public double GetFogDensity(double distance)
    {
      if (HasFogDensity)
      {
        // todo:  i have no idea what k should be, and we need to make this scene configurable
        return ExponentialFogDensity(distance, 0.3);
      }
      else
      {
        return 0.0;
      }
    }
  }
}