using System;
using System.Collections.Generic;
using System.Text;

namespace raylib
{
    public interface ILight
    {
      int Id { get; }
      PosVector Position { get; }
      ColorVector Color { get; }      
    }
}
