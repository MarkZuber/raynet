using System;
using System.IO;
using ImageSharp;
using ImageSharp.Formats;

namespace raylib
{
  public class PixelArray : IDisposable
  {
    private readonly Image<Rgba32> _image;
    private readonly object _lock = new object();

    public PixelArray(int width, int height)
    {
      Width = width;
      Height = height;
      _image = new Image<Rgba32>(width, height);
    }

    public int Width { get; }
    public int Height { get; }

    /// <inheritdoc />
    public void Dispose()
    {
      _image?.Dispose();
    }

    public void SetPixelColor(int x, int y, ColorVector color)
    {
      // Console.WriteLine($"Setting ({x}, {y}) to {color}");
      lock (_lock)
      {
        _image[x, y] = ClampToPixel(color);
      }
    }

    private byte ColorToByte(double c)
    {
      return Convert.ToByte(c * 255.0);
    }

    private Rgba32 ClampToPixel(ColorVector color)
    {
      var clamped = color.Clamp();
      return new Rgba32(ColorToByte(clamped.R), ColorToByte(clamped.G), ColorToByte(clamped.B));
    }

    public void SaveAsFile(string outputFilePath)
    {
      lock (_lock)
      {
        _image.Save(outputFilePath);
      }
    }

    public MemoryStream SaveToStreamAsPng()
    {
      lock (_lock)
      {
        var ms = new MemoryStream();
        // _image.SaveAsPng(ms);
        return ms;
      }
    }
  }
}