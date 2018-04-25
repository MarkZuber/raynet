namespace raylib
{
  public class Background
  {
    public Background(ColorVector color, double ambience)
    {
      Color = color;
      Ambience = ambience;
    }

    public ColorVector Color { get; }
    public double Ambience { get; }
  }
}