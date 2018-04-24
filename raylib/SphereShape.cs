using System;
using System.Collections.Generic;
using System.Text;

namespace raylib
{
    public class SphereShape : Shape
    {
      private readonly BaseMaterial _material;

        public SphereShape(PosVector position, double radius, BaseMaterial material) : base(position)
      {
        Radius = radius;
        _material= material;
      }

      public double Radius { get; }

      public override IntersectionInfo Intersect(Ray ray)
      {
        var dst = ray.Position - Position;
        double b = dst.Dot(ray.Direction);
        double c = dst.Dot(dst) - (Radius * Radius);
        double d = b * b - c;

        if (d > 0.0)
        {
          double distance = -b - Math.Sqrt(d);
          var position = ray.Position + (ray.Direction * distance);
          var normal = (position - Position).Normalize();

          // todo: u/v coord texture mapping
          ColorVector color = GetMaterial().GetColor(0.0, 0.0);

          return new IntersectionInfo(color, distance, normal, position, Id);
        }
        else
        {
          return new IntersectionInfo();
        }
      }

      public override BaseMaterial GetMaterial()
      {
        return _material;
      }

      public override Bound CalculateBoundingPlanes(PosVector unitVector)
      {
        double cd = unitVector.Dot(Position);
        return new Bound(cd + Radius, cd - Radius);
      }
    }
}
