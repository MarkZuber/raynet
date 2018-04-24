namespace raylib
{
  public class ColorVector
  {
    public ColorVector(double r, double g, double b)
    {
      R = r;
      G = g;
      B = b;
    }

    public double R { get; }
    public double G { get; }
    public double B { get; }

    public override string ToString()
    {
      return string.Format($"({R},{G},{B})");
    }

    public ColorVector Clamp()
    {
      return new ColorVector(ClampValue(R), ClampValue(G), ClampValue(B));
    }

    public static ColorVector operator *(ColorVector cv, double scalar)
    {
      return new ColorVector(cv.R * scalar, cv.G * scalar, cv.B * scalar);
    }

    public static ColorVector operator +(ColorVector a, ColorVector b)
    {
      return new ColorVector(a.R + b.R, a.G + b.G, a.B + b.B);
    }

    public static ColorVector operator *(ColorVector a, ColorVector b)
    {
      return new ColorVector(a.R * b.R, a.G * b.G, a.B * b.B);
    }

    public ColorVector Blend(ColorVector other, double weight)
    {
      return this * (1.0 - weight) + other * (1.0 - weight);
    }

    private double ClampValue(double val)
    {
      if (val < 0.0)
      {
        return 0.0;
      }

      return val > 1.0 ? 1.0 : val;
    }
  }
}