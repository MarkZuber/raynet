using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using raylib;

namespace raywpf
{
  public class WpfPixelArray : IPixelArray
  {
    public WpfPixelArray(Dispatcher dispatcher, int width, int height)
    {
      Width = width;
      Height = height;
      WriteableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
      _dispatcher = dispatcher;
      // _image = new Image<Rgba32>(width, height);
    }

    private readonly Dispatcher _dispatcher;
    public WriteableBitmap WriteableBitmap { get; }

    /// <inheritdoc />
    public void Dispose()
    {
    }

    /// <inheritdoc />
    public int Height { get; }

    /// <inheritdoc />
    public int Width { get; }

    /// <inheritdoc />
    public void Lock()
    {
      _dispatcher.Invoke(() => { WriteableBitmap.Lock(); });
    }

    /// <inheritdoc />
    public void Unlock()
    {
      _dispatcher.Invoke(() => { WriteableBitmap.Unlock(); });
    }

    private int ColorToInt(double c)
    {
      return Convert.ToInt32(c * 255.0);
    }

    private void UnsafeSetPixelColor(int x, int y, ColorVector color)
    {
      unsafe
      {
            // Get a pointer to the back buffer.
            int pBackBuffer = (int)WriteableBitmap.BackBuffer;

            // Find the address of the pixel to draw.
            pBackBuffer += y * WriteableBitmap.BackBufferStride;
            pBackBuffer += x * 4;

            // Compute the pixel's color.
            var clamped = color.Clamp();
            int colorData = ColorToInt(clamped.R) << 16; // R
            colorData |= ColorToInt(clamped.G) << 8; // G
            colorData |= ColorToInt(clamped.B) << 0; // B

            // Assign the color data to the pixel.
            *((int*)pBackBuffer) = colorData;
      }
    }

    /// <inheritdoc />
    public void SetPixelColor(int x, int y, ColorVector color)
    {
        _dispatcher.Invoke(
          () =>
          {
            UnsafeSetPixelColor(x, y, color);
            WriteableBitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
          });
    }

    /// <inheritdoc />
    public void SetPixelRowColors(int y, List<ColorVector> xPixels)
    {
      _dispatcher.Invoke(
        () =>
        {
          for (int x = 0; x < Width; x++)
          {
            UnsafeSetPixelColor(x, y, xPixels[x]);
          }

          WriteableBitmap.AddDirtyRect(new Int32Rect(0, y, Width, 1));
        });
    }

    /// <inheritdoc />
    public void SaveAsFile(string outputFilePath)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc />
    public MemoryStream SaveToStreamAsPng()
    {
      throw new NotImplementedException();
    }
  }
}
