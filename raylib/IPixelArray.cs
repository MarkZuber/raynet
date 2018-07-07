using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace raylib
{
    public interface IPixelArray : IDisposable
    {
      int Height { get; }
      int Width { get; }

      void Lock();
      void Unlock();

      void SetPixelColor(int x, int y, ColorVector color);
      void SetPixelRowColors(int y, List<ColorVector> xPixels);
      void SaveAsFile(string outputFilePath);
      MemoryStream SaveToStreamAsPng();
    }
}
