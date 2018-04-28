namespace raylib
{
  public interface IShape
  {
    PosVector Position { get; }

    int Id { get; }
    BoundingBox BoundingBox { get; }

    IntersectionInfo Intersect(Ray ray);
    BaseMaterial GetMaterial();
  }
}