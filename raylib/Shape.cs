using ImageSharp.Processing;

namespace raylib
{
  public abstract class Shape : IShape
    {
    protected Shape(PosVector position)
    {
      Position = position;
      Id = 0;
    }

    public PosVector Position { get; }

    public int Id { get; set; }

    public abstract IntersectionInfo Intersect(Ray ray);
    public abstract BaseMaterial GetMaterial();
    public abstract Bound CalculateBoundingPlanes(PosVector unitVector);
  }
}