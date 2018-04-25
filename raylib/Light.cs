namespace raylib
{
  public class Light : ILight
  {
    public Light(PosVector position, ColorVector color)
    {
      Position = position;
      Color = color;
      Id = 0;
    }

    public int Id { get; set; }
    public PosVector Position { get; }
    public ColorVector Color { get; }
  }
}