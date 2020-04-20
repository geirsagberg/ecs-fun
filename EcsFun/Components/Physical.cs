using Microsoft.Xna.Framework;

namespace EcsFun.Components
{
    public class Physical
    {
        public Vector2 Velocity { get; set; }
        public bool AffectedByGravity { get; set; } = true;
    }
}