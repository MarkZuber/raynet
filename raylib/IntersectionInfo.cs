using System;
using System.Collections.Generic;
using System.Text;

namespace raylib
{
    public class IntersectionInfo
    {
      public IntersectionInfo() : this(new ColorVector(0.0, 0.0, 0.0), double.MaxValue, PosVector.NewDefault(),
        PosVector.NewDefault(), 0)
      {
        
      }

      public IntersectionInfo(ColorVector color, double distance, PosVector normal, PosVector position, int elementId)
      {
        Color = color;
        Distance = distance;
        ElementId = elementId;
        Normal = normal;
        Position = position;
      }

      public ColorVector Color { get; }
      public double Distance { get; }
      public long ElementId { get; }
      public bool IsHit => ElementId != 0;
      public PosVector Normal { get; }
      public PosVector Position { get; }
    }
}
