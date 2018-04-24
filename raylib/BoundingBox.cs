using System;

namespace raylib
{
  public class BoundingBox
  {
    public enum ValSign
    {
      Zero,
      Negative,
      Positive
    }

    public BoundingBox(PosVector boxMin, PosVector boxMax)
    {
      BoxMin = boxMin;
      BoxMax = boxMax;
    }

    public BoundingBox(Bound x, Bound y, Bound z) : this(new PosVector(x.Min, y.Min, z.Min),
      new PosVector(x.Max, y.Max, z.Max))
    {
    }

    public BoundingBox() : this(new PosVector(double.MaxValue, double.MaxValue, double.MaxValue),
      new PosVector(double.MinValue, double.MinValue, double.MinValue))
    {
    }

    public PosVector BoxMin { get; }
    public PosVector BoxMax { get; }

    public static BoundingBox FromShape(Shape shape)
    {
      return new BoundingBox(
        shape.CalculateBoundingPlanes(PosVector.NewUnitX()),
        shape.CalculateBoundingPlanes(PosVector.NewUnitY()),
        shape.CalculateBoundingPlanes(PosVector.NewUnitZ())
      );
    }


    public BoundingBox GetEnlargedToEnclose(BoundingBox other)
    {
      return new BoundingBox(
        new Bound(Math.Min(BoxMin.X, other.BoxMin.X), Math.Max(BoxMax.X, other.BoxMax.X)),
        new Bound(Math.Min(BoxMin.Y, other.BoxMin.Y), Math.Max(BoxMax.Y, other.BoxMax.Y)),
        new Bound(Math.Min(BoxMin.Z, other.BoxMin.Z), Math.Max(BoxMax.Z, other.BoxMax.Z))
      );
    }

    public bool IsPointInside(PosVector pos)
    {
      return pos.X > BoxMin.X && pos.X < BoxMax.X &&
             pos.Y > BoxMin.Y && pos.Y < BoxMax.Y &&
             pos.Z > BoxMin.Z && pos.Z < BoxMax.Z;
    }

    public bool IsWellFormed()
    {
      return BoxMin.X <= BoxMax.X && BoxMin.Y <= BoxMax.Y && BoxMin.Z <= BoxMax.Z;
    }

    public bool IsEmpty()
    {
      return BoxMax.X < BoxMin.X || BoxMax.Y < BoxMin.Y || BoxMax.Z < BoxMin.Z;
    }

    public double GetSurfaceArea()
    {
      var delta = BoxMax - BoxMin;
      return (delta.X * delta.Y + delta.X * delta.Z + delta.Y * delta.Z) * 2.0;
    }

    public ValSign CalcSign(double value)
    {
      if (value < 0.0)
      {
        return ValSign.Negative;
      }

      if (Math.Abs(value) < 0.00001)
      {
        return ValSign.Zero;
      }

      return ValSign.Positive;
    }
  }
}