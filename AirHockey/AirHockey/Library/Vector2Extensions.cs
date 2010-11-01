using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AirHockey.Library
{
    public static class Vector2Extensions 
    {
        public static Vector2 OriginVector(this Vector2 vec)
        {
            return new Vector2(vec.X / 2, vec.Y / 2);
        }
    }
}
