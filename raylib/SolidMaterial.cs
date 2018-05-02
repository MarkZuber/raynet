namespace raylib
{
  public class SolidMaterial : BaseMaterial
  {
    public SolidMaterial(double kAmbient, double kDiffuse, double kSpecular, double kReflection, double kTransparent, double refraction, double gloss, ColorVector color)
      : base(kAmbient, kDiffuse, kSpecular, kReflection, kTransparent, refraction, gloss)
    {
      Color = color;
    }

    public ColorVector Color { get; }

    public override bool HasTexture => false;

    public override ColorVector GetColor(double u, double v)
    {
      return Color;
    }
  }
}