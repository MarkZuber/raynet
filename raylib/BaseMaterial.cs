using System;

namespace raylib
{
  public abstract class BaseMaterial
  {
    protected BaseMaterial(double kAmbient, double kDiffuse, double kSpecular, double kReflection, double kTransparent, double refraction, double gloss)
    {
      if (!IsEqualWithinTolerance(1.0, 0.005, kAmbient + kDiffuse + kSpecular + kReflection + kTransparent))
      {
        // throw new ArgumentException();
      }
      KAmbient = kAmbient;
      KDiffuse = kDiffuse;
      KSpecular = kSpecular;
      KReflection = kReflection;
      KTransparent = kTransparent;
      Refraction = refraction;
      Gloss = gloss;
    }

    private static bool IsEqualWithinTolerance(double expected, double tolerance, double actual)
    {
      double total = Math.Abs(Math.Abs(actual) - Math.Abs(expected));
      return total < tolerance;
    }

    // required:  Ka + Kd + Ks + Kr + Kt = 1.0
    public double KAmbient { get; }
    public double KDiffuse { get; }
    public double KSpecular { get; }
    public double KReflection { get; }
    public double KTransparent { get; }
    public double Gloss { get; }
    public double Refraction { get; }

    public abstract bool HasTexture { get; }

    public abstract ColorVector GetColor(double u, double v);
  }
}