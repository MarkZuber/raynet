using System;

namespace raylib
{
  public class PosVector
  {
    public PosVector(double x, double y, double z)
    {
      X = x;
      Y = y;
      Z = z;
    }

    public double X { get; }
    public double Y { get; }
    public double Z { get; }

    public static PosVector NewDefault()
    {
      return new PosVector(0.0, 0.0, 0.0);
    }

    public static PosVector NewUnitX()
    {
      return new PosVector(1.0, 0.0, 0.0);
    }

    public static PosVector NewUnitY()
    {
      return new PosVector(0.0, 1.0, 0.0);
    }

    public static PosVector NewUnitZ()
    {
      return new PosVector(0.0, 0.0, 1.0);
    }

    public override string ToString()
    {
      return string.Format($"({X},{Y},{Z})");
    }

    public PosVector Normalize()
    {
      return this / Magnitude();
    }

    public double Magnitude()
    {
      return Math.Sqrt(MagnitudeSquared());
    }

    public double MagnitudeSquared()
    {
      return X * X + Y * Y + Z * Z;
    }

    public static PosVector operator +(PosVector a, PosVector b)
    {
      return new PosVector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static PosVector operator -(PosVector a, PosVector b)
    {
      return new PosVector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    public static PosVector operator *(PosVector a, double scalar)
    {
      return new PosVector(a.X * scalar, a.Y * scalar, a.Z * scalar);
    }

    public static PosVector operator /(PosVector a, double scalar)
    {
      return new PosVector(a.X / scalar, a.Y / scalar, a.Z / scalar);
    }

    public PosVector AddScaled(PosVector b, double scale)
    {
      return new PosVector(X + scale * b.X, Y + scale * b.Y, Z + scale * b.Z);
    }

    public PosVector Cross(PosVector b)
    {
      return new PosVector(Y * b.Z - Z * b.Y, Z * b.X - X * b.Z, X * b.Y - Y * b.X);
    }

    public double Dot(PosVector b)
    {
      return X * b.X + Y * b.Y + Z * b.Z;
    }
  }
}