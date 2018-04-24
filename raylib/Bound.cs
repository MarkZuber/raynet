namespace raylib
{
  public class Bound
  {
    public Bound(double min, double max)
    {
      Min = min;
      Max = max;
    }

    public double Min { get; }
    public double Max { get; }
  }
}