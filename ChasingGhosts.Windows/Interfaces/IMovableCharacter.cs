using ChasingGhosts.Windows.World;

using Microsoft.Xna.Framework;

namespace ChasingGhosts.Windows.Interfaces
{
    public interface IMovableCharacter : IPhysicsEntity
    {
        Movement Direction { get; }

        float MaxMovement { get; }

        Vector2 Movement { get; }
    }
}