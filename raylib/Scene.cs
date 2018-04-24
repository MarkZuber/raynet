using System.Collections.Generic;

namespace raylib
{
  public class Scene
  {
    private readonly Dictionary<long, ILight> _lights = new Dictionary<long, ILight>();
    private readonly Dictionary<long, IShape> _shapes = new Dictionary<long, IShape>();

    public Scene(Background background, IEnumerable<Shape> shapes, IEnumerable<Light> lights)
    {
      Background = background;

      var currentShapeId = 1;

      foreach (var shape in shapes)
      {
        shape.Id = currentShapeId;
        // todo: build bounding boxes
        _shapes[currentShapeId] = shape;
        currentShapeId++;
      }

      var currentLightId = 1;
      foreach (var light in lights)
      {
        light.Id = currentLightId;
        _lights[currentLightId] = light;
        currentLightId++;
      }

      // todo: build kdtree
    }

    public IEnumerable<IShape> Shapes => _shapes.Values;
    public Background Background { get; }
    public IEnumerable<ILight> Lights => _lights.Values;

    public bool TryGetShape(long shapeId, out IShape shape)
    {
      return _shapes.TryGetValue(shapeId, out shape);
    }
  }
}