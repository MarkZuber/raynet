namespace raylib
{
  public class RenderData
  {
    public RenderData(int width, int height, int rayTraceDepth, int numThreads, bool threadPerLine)
    {
      Width = width;
      Height = height;
      RayTraceDepth = rayTraceDepth;
      NumThreads = numThreads;
      ThreadPerLine = threadPerLine;
      RenderDiffuse = true;
      RenderReflection = true;
      RenderRefraction = true;
      RenderShadow = true;
      RenderHighlights = true;
    }

    public int Width { get; }
    public int Height { get; }
    public int RayTraceDepth { get; }
    public int NumThreads { get; }
    public bool ThreadPerLine { get; }
    public bool RenderDiffuse { get; }
    public bool RenderReflection { get; }
    public bool RenderRefraction { get; }
    public bool RenderShadow { get; }
    public bool RenderHighlights { get; }
  }
}