using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Sharp2D.Engine.Common.ObjectSystem;
using Sharp2D.Engine.Infrastructure;

namespace ChasingGhosts.Windows.UI
{
    public class DeathScreen : GameObject
    {
        public override void Initialize(IResolver resolver)
        {
            base.Initialize(resolver);

            this.Add(new AnimatingBackground(new Color(48, 48, 48), .85f));
        }
    }
}