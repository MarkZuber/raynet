using System;
using System.Collections.Generic;
using System.Text;

namespace raylib
{
    public class PointLight : Light
    {
      /// <inheritdoc />
      public PointLight(PosVector position, ColorVector color) : base(position, color)
      {
      }
    }
}
