namespace raylib
{
  public class ChessboardMaterial : BaseMaterial
  {
    private readonly ColorVector _colorEven;
    private readonly ColorVector _colorOdd;
    private readonly double _scale;

    public ChessboardMaterial(double kAmbient, double kDiffuse, double kSpecular, double kReflection, double kTransparent, double refraction, double gloss,
      ColorVector colorEven, ColorVector colorOdd, double scale) : base(kAmbient, kDiffuse, kSpecular, kReflection, kTransparent, refraction, gloss)
    {
      _colorEven = colorEven;
      _colorOdd = colorOdd;
      _scale = scale;
    }

    public override bool HasTexture => true;

    public override ColorVector GetColor(double u, double v)
    {
      var t = WrapUpScale(u, _scale) * WrapUpScale(v, _scale);
      return t < 0.0 ? _colorEven : _colorOdd;
    }

    private double WrapUpScale(double t, double scale)
    {
      var x = t % scale;
      if (x < -scale / 2.0)
      {
        x += scale;
      }

      if (x >= scale / 2.0)
      {
        x -= scale;
      }

      return x;
    }
  }
}