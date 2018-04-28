using System;
using System.Linq;

namespace raylib
{
  public class RayTracer
  {
    public RayTracer(Camera camera, RenderData renderData, Scene scene, bool useKdTree)
    {
      Camera = camera;
      RenderData = renderData;
      Scene = scene;
      UseKdTree = useKdTree;
    }

    public Camera Camera { get; }
    public RenderData RenderData { get; }
    public Scene Scene { get; }
    public bool UseKdTree { get; }

    public ColorVector GetPixelColor(int x, int y)
    {
      var xp = Convert.ToDouble(x) / Convert.ToDouble(RenderData.Width) * 2.0 - 1.0;
      var yp = -(Convert.ToDouble(y) / Convert.ToDouble(RenderData.Height) * 2.0 -
                 1.0); // yp is UP but our pixels are increasing in value DOWN, so need inverse

      var ray = Camera.GetRay(xp, yp);
      return CalculateColor(ray);
    }

    private Ray GetReflectionRay(PosVector p, PosVector n, PosVector v)
    {
      var rl = v + n * 2.0 * -n.Dot(v);
      return new Ray(p, rl);
    }

    private Ray GetRefractionRay(PosVector p, PosVector n, PosVector v, double refraction)
    {
      var c1 = n.Dot(v);
      var c2 = 1.0 - refraction * refraction * Math.Sqrt(1.0 - c1 * c1);
      var t = (n * (refraction * c1 - c2) - v * refraction * -1.0).Normalize();
      return new Ray(p, t);
    }

    private IntersectionInfo TestIntersectionBasic(Ray ray, int excludeId)
    {
      var bestInfo = new IntersectionInfo();

      foreach (var shape in Scene.Shapes.Where(s => s.Id != excludeId))
      {
        var info = shape.Intersect(ray);
        if (info.IsHit && info.Distance < bestInfo.Distance && info.Distance >= 0.0)
        {
          bestInfo = info;
        }
      }

      return bestInfo;
    }

    private IntersectionInfo TestIntersectionKd(Ray ray, int excludeId)
    {
      return Scene.KdTree.FindIntersectionTree(ray);
    }

    private IntersectionInfo TestIntersection(Ray ray, int excludeId)
    {
      return UseKdTree ? TestIntersectionKd(ray, excludeId) : TestIntersectionBasic(ray, excludeId);
    }

    private ColorVector RenderDiffuse(ColorVector currentColor, IntersectionInfo intersectionInfo, ILight light)
    {
      if (RenderData.RenderDiffuse)
      {
        var v = (light.Position - intersectionInfo.Position).Normalize();
        var l = v.Dot(intersectionInfo.Normal);
        if (l > 0.0)
        {
          return currentColor + intersectionInfo.Color * light.Color * l;
        }
      }

      return currentColor;
    }

    private ColorVector RenderReflection(ColorVector currentColor, IntersectionInfo intersectionInfo, Ray ray,
      int depth)
    {
      if (RenderData.RenderReflection)
      {
        if (Scene.TryGetShape(intersectionInfo.ElementId, out var shape))
        {
          if (shape.GetMaterial().Reflection > 0.0)
          {
            var reflectionRay = GetReflectionRay(intersectionInfo.Position, intersectionInfo.Normal, ray.Direction);
            var refl = TestIntersection(reflectionRay, shape.Id);
            var reflColor = refl.IsHit && refl.Distance > 0.0
              ? RayTrace(refl, reflectionRay, depth + 1)
              : Scene.Background.Color;

            return currentColor.Blend(reflColor, shape.GetMaterial().Reflection);
          }
        }
      }

      return currentColor;
    }

    private ColorVector RenderRefraction(ColorVector currentColor, IntersectionInfo intersectionInfo, Ray ray,
      int depth)
    {
      if (RenderData.RenderRefraction)
      {
        if (Scene.TryGetShape(intersectionInfo.ElementId, out var shape))
        {
          if (shape.GetMaterial().Transparency > 0.0)
          {
            var refractionRay = GetRefractionRay(intersectionInfo.Position, intersectionInfo.Normal, ray.Direction,
              shape.GetMaterial().Refraction);
            var refr = shape.Intersect(refractionRay);
            var refractedColor = Scene.Background.Color;
            if (refr.IsHit)
            {
              if (Scene.TryGetShape(refr.ElementId, out var refrShape))
              {
                var elemRefractionRay = GetRefractionRay(refr.Position, refr.Normal, refractionRay.Direction,
                  refrShape.GetMaterial().Refraction);
                var secondRefr = TestIntersection(elemRefractionRay, shape.Id);
                if (secondRefr.IsHit && secondRefr.Distance > 0.0)
                {
                  refractedColor = RayTrace(secondRefr, elemRefractionRay, depth + 1);
                }
              }
            }
            else
            {
              refractedColor = Scene.Background.Color;
            }

            return currentColor.Blend(refractedColor, shape.GetMaterial().Transparency);
          }
        }
      }

      return currentColor;
    }

    private ColorVector RenderHighlights(ColorVector currentColor, IShape shape, IntersectionInfo shadowIntersection,
      ILight light)
    {
      if (RenderData.RenderHighlights && !shadowIntersection.IsHit && shape.GetMaterial().Gloss > 0.0)
      {
        var lv = (shape.Position - light.Position).Normalize();
        var e = (Camera.Position - shape.Position).Normalize();
        var h = (e - lv).Normalize();
        var glossWeight = 0.0; // todo: pow(max(dot(info.Normal, h), 0.0), shininess)
        return currentColor + light.Color * glossWeight;
      }

      return currentColor;
    }

    private ColorVector RenderShadowAndHighlights(ColorVector currentColor, IntersectionInfo intersectionInfo,
      ILight light)
    {
      var v = (light.Position - intersectionInfo.Position).Normalize();
      var shadowRay = new Ray(intersectionInfo.Position, v);

      var color = currentColor;

      if (Scene.TryGetShape(intersectionInfo.ElementId, out var shape))
      {
        if (RenderData.RenderShadow)
        {
          var shadowIntersection = TestIntersection(shadowRay, shape.Id);
          if (shadowIntersection.IsHit)
          {
            if (Scene.TryGetShape(shadowIntersection.ElementId, out var shadowShape))
            {
              if (shadowShape.Id != shape.Id)
              {
                var trans = shadowShape.GetMaterial().Transparency;
                var transPower = Math.Pow(trans, 0.5);
                color = color * (0.5 + 0.5 * transPower);
              }
            }
          }

          color = RenderHighlights(color, shape, shadowIntersection, light);
        }
      }

      return color;
    }


    private ColorVector RayTrace(IntersectionInfo intersectionInfo, Ray ray, int depth)
    {
      var color = intersectionInfo.Color * Scene.Background.Ambience;

      foreach (var light in Scene.Lights)
      {
        color = RenderDiffuse(color, intersectionInfo, light);

        if (depth < RenderData.RayTraceDepth)
        {
          color = RenderReflection(color, intersectionInfo, ray, depth);
          color = RenderRefraction(color, intersectionInfo, ray, depth);
          color = RenderShadowAndHighlights(color, intersectionInfo, light);
        }
      }

      return color;
    }

    private ColorVector CalculateColor(Ray ray)
    {
      var intersectionInfo = TestIntersection(ray, 0);
      return intersectionInfo.IsHit ? RayTrace(intersectionInfo, ray, 0) : Scene.Background.Color;
    }
  }
}