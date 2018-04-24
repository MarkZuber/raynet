using System;

namespace raylib
{
  public class Camera
  {
    private readonly PosVector _a1;
    private readonly PosVector _a2;
    private readonly PosVector _a3;
    private readonly double _dval;
    private readonly PosVector _center;

    public Camera(PosVector pos, PosVector lookAt, PosVector up, double fov)
    {
      Position = pos;
      LookAt = lookAt;
      Up = up;
      Fov = fov;

      _a3 = (LookAt - Position).Normalize();
      _a1 = _a3.Cross(up).Normalize();
      _a2 = _a1.Cross(_a3).Normalize();
      double viewAngleRadians = Fov * 0.017453239;
      _dval = Math.Cos(viewAngleRadians / 2.0) / Math.Sin(viewAngleRadians / 2.0);

      _center = _a3 * _dval;
        }

    public PosVector Position { get; }
    public PosVector LookAt { get; }
    public PosVector Up { get; }
    public double Fov { get; }

    public Ray GetRay(double vx, double vy)
    {
      var dir = _center + (_a1 * vx) + (_a2 * vy);

      return new Ray(Position, dir.Normalize());
    }
  }
}