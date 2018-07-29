using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Sharp2D.Engine.Common;
using Sharp2D.Engine.Common.Components.Animations;
using Sharp2D.Engine.Common.ObjectSystem;
using Sharp2D.Engine.Drawing;
using Sharp2D.Engine.Infrastructure;

namespace ChasingGhosts.Windows.UI
{
    public class AnimatingBackground : GameObject
    {
        private readonly Color color;

        private float transparency;

        private Texture2D text;

        public AnimatingBackground(Color color, float transparency)
        {
            this.color = color;
            this.transparency = transparency;
        }

        public override void Initialize(IResolver resolver)
        {
            base.Initialize(resolver);

            this.text = new Texture2D(resolver.Resolve<GraphicsDevice>(), 1, 1);
            this.text.SetData(new[]
            {
                Color.White
            });

            const float FullTransparent = .85f;
            var anim = ValueAnimator.PlayAnimation(this, val => this.transparency = val * FullTransparent, TimeSpan.FromSeconds(1f));
            anim.Easing = AnimationEase.CubicEaseOut;
        }

        public override void Draw(SharpDrawBatch batch, GameTime time)
        {
            var screenSize = Resolution.VirtualScreen;
            batch.Draw(this.text, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), this.color * this.transparency);
            base.Draw(batch, time);
        }
    }
}