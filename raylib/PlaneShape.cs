namespace raylib
{
  public class PlaneShape : Shape
  {
    private readonly double _dval;
    private readonly BaseMaterial _material;

    public PlaneShape(PosVector position, double dval, BaseMaterial material) : base(position)
    {
      _dval = dval;
      _material = material;
    }

    public override IntersectionInfo Intersect(Ray ray)
    {
      var vd = Position.Dot(ray.Direction);

      if (vd >= 0.0)
      {
        return new IntersectionInfo();
      }

      var t = -(Position.Dot(ray.Position) + _dval) / vd;

      if (t <= 0.0)
      {
        return new IntersectionInfo();
      }

      var intersectPosition = ray.Position + ray.Direction * t;

      var u = 0.0;
      var v = 0.0;

      if (GetMaterial().HasTexture)
      {
        var vecU = new PosVector(Position.Y, Position.Z, -Position.X);
        var vecV = vecU.Cross(Position);
        u = intersectPosition.Dot(vecU);
        v = intersectPosition.Dot(vecV);
      }

      var color = GetMaterial().GetColor(u, v);

      return new IntersectionInfo(color, t, Position, intersectPosition, Id);
    }

    public override BaseMaterial GetMaterial()
    {
      return _material;
    }

    public override Bound CalculateBoundingPlanes(PosVector unitVector)
    {
      // todo: fix this.  it's an infinite plane so it doesn't really have a bounding box.
      return new Bound(1.0, 1.0);
    }
  }
}