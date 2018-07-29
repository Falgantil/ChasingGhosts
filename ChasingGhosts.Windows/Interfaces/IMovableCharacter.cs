using Microsoft.Xna.Framework;

using Sharp2D.Engine.Common.ObjectSystem;

namespace ChasingGhosts.Windows.Interfaces
{
    public interface IMovableCharacter
    {
        Vector2 Movement { get; }

        Vector2 LocalPosition { get; set; }

        Rectanglef GlobalRegion { get; }
    }
}