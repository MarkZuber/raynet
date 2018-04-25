namespace raylib
{
  public class Ray
  {
    public Ray(PosVector pos, PosVector dir)
    {
      Position = pos;
      Direction = dir;
    }

    public PosVector Position { get; }
    public PosVector Direction { get; }

    public override string ToString()
    {
      return string.Format($"Ray(Pos:{Position} Dir:{Direction})");
    }
  }
}