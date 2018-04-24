namespace raylib
{
  public class SolidMaterial : BaseMaterial
  {
    public SolidMaterial(double gloss, double reflection, double refraction, double transparency, ColorVector color) 
      : base(gloss, reflection, refraction, transparency)
    {
      Color = color;
    }

    public ColorVector Color { get; }

    public override ColorVector GetColor(double u, double v)
    {
      return Color;
    }

    public override bool HasTexture => false;
  }
}