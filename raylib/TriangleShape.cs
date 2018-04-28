using System;

namespace raylib
{
  public class TriangleShape : Shape
  {
    private readonly BaseMaterial _backMaterial;
    private readonly BaseMaterial _frontMaterial;

    public TriangleShape(PosVector position, PosVector vb, PosVector vc, BaseMaterial frontMaterial,
      BaseMaterial backMaterial) : base(position)
    {
      VB = vb;
      VC = vc;
      _frontMaterial = frontMaterial;
      _backMaterial = backMaterial;

      EdgeAb = VB - VA;
      EdgeBc = VC - VB;
      EdgeCa = VA - VC;

      Normal = EdgeAb.Dot(EdgeBc) < EdgeBc.Dot(EdgeCa) ? EdgeAb.Cross(EdgeBc) : EdgeBc.Cross(EdgeCa);
      Magnitude = Normal.Magnitude();
      if (Magnitude > 0.0)
      {
        Normal = Normal / Magnitude; // unit vector to triangle plane
      }

      PlaneCoefficient = Normal.Dot(VA); // same coeff for all three vertices

      var a = EdgeAb.MagnitudeSquared();
      var b = EdgeAb.Dot(EdgeCa);
      var c = EdgeCa.MagnitudeSquared();
      var dinv = 1.0 / (a * c - b * b);

      a = a * dinv;
      b = b * dinv;
      c = c * dinv;

      Ubeta = (EdgeAb * c).AddScaled(EdgeCa, -b);
      Ugamma = (EdgeCa * -a).AddScaled(EdgeAb, b);
    }

    public PosVector VA => Position;
    public PosVector VB { get; }
    public PosVector VC { get; }

    public PosVector EdgeAb { get; }
    public PosVector EdgeBc { get; }
    public PosVector EdgeCa { get; }
    public PosVector Normal { get; }
    public double Magnitude { get; }
    public double PlaneCoefficient { get; }
    public PosVector Ubeta { get; }
    public PosVector Ugamma { get; }

    public bool IsWellFormed()
    {
      return Normal.MagnitudeSquared() > 0.0;
    }

    public bool IsBackfaceCulled()
    {
      // todo: this is if back material is null.  do we ever have that case?
      return false;
    }

    public override IntersectionInfo Intersect(Ray ray)
    {
      var maxDistance = double.MaxValue;

      var mdotn = ray.Direction.Dot(Normal);
      var planarDist = ray.Position.Dot(Normal) - PlaneCoefficient;
      var noIntersection = false;

      var frontFace = mdotn <= 0.0;
      if (frontFace)
      {
        if (planarDist <= 0.0 || planarDist >= -maxDistance * mdotn)
        {
          noIntersection = true;
        }
      }
      else
      {
        if (IsBackfaceCulled() || planarDist >= 0.0 || -planarDist >= maxDistance * mdotn)
        {
          noIntersection = true;
        }
      }

      var intersectDistance = -planarDist / mdotn;

      // point of view line intersecting plane
      var q = ray.Direction * intersectDistance + ray.Position;

      // compute barycentric coordinates
      var v = q - VA;
      var vCoord = v.Dot(Ubeta);
      if (vCoord < 0.0)
      {
        noIntersection = true;
      }

      var wCoord = v.Dot(Ugamma);
      if (wCoord < 0.0 || vCoord + wCoord > 1.0)
      {
        noIntersection = true;
      }

      if (noIntersection)
      {
        return new IntersectionInfo();
      }

      var color = frontFace ? _frontMaterial.GetColor(vCoord, wCoord) : _backMaterial.GetColor(vCoord, wCoord);
      return new IntersectionInfo(color, intersectDistance, Normal, q, Id);
    }

    public override BaseMaterial GetMaterial()
    {
      return _frontMaterial;
    }

    public override PosVector GetMinPoint()
    {
      double xmin = VA.X;
      double ymin = VA.Y;
      double zmin = VA.Z;

      xmin = Math.Min(xmin, VB.X);
      ymin = Math.Min(ymin, VB.Y);
      zmin = Math.Min(zmin, VB.Z);

      xmin = Math.Min(xmin, VC.X);
      ymin = Math.Min(ymin, VC.Y);
      zmin = Math.Min(zmin, VC.Z);

      return new PosVector(xmin - double.Epsilon, ymin - double.Epsilon, zmin - double.Epsilon);
    }

    public override PosVector GetMaxPoint()
    {
      double xmax = VA.X;
      double ymax = VA.Y;
      double zmax = VA.Z;

      xmax = Math.Max(xmax, VB.X);
      ymax = Math.Max(ymax, VB.Y);
      zmax = Math.Max(zmax, VB.Z);

      xmax = Math.Max(xmax, VC.X);
      ymax = Math.Max(ymax, VC.Y);
      zmax = Math.Max(zmax, VC.Z);

      return new PosVector(xmax + double.Epsilon, ymax + double.Epsilon, zmax + double.Epsilon);
    }
  }
}