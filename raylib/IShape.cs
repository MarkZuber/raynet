using System;
using System.Collections.Generic;
using System.Text;

namespace raylib
{
    public interface IShape
    {
      PosVector Position { get; }

      int Id { get; }

      IntersectionInfo Intersect(Ray ray);
      BaseMaterial GetMaterial();
      Bound CalculateBoundingPlanes(PosVector unitVector);
    }
}
