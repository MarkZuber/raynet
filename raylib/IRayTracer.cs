using System;
using System.Collections.Generic;
using System.Text;

namespace raylib
{
    public interface IRayTracer
    {
      ColorVector GetPixelColor(int x, int y);
      RenderData RenderData { get; }
    }
}
