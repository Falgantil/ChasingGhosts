using Microsoft.Xna.Framework;

namespace ChasingGhosts.Windows.Interfaces
{
    public interface IMovableCharacter : IPhysicsEntity
    {
        float MaxMovement { get; }

        Vector2 Movement { get; }
    }
}