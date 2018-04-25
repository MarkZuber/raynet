namespace raylib
{
  public abstract class BaseMaterial
  {
    protected BaseMaterial(double gloss, double reflection, double refraction, double transparency)
    {
      Gloss = gloss;
      Reflection = reflection;
      Refraction = refraction;
      Transparency = transparency;
    }

    public double Gloss { get; }
    public double Reflection { get; }
    public double Refraction { get; }
    public double Transparency { get; }
    public abstract bool HasTexture { get; }

    public abstract ColorVector GetColor(double u, double v);
  }
}