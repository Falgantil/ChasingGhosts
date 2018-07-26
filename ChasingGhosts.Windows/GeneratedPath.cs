using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Sharp2D.Engine.Common.World;
using Sharp2D.Engine.Drawing;
using Sharp2D.Engine.Helper;
using Sharp2D.Engine.Infrastructure;

namespace ChasingGhosts.Windows
{
    public class GeneratedPath : WorldObject
    {
        private const int Distance = 50;

        private List<Vector2> dots = new List<Vector2>();

        public override void Initialize(IResolver resolver)
        {
            const int Segments = 50;

            var rnd = new Random();
            var rotation = rnd.Next(0, 360);
            var currentPos = new Vector2();
            for (int i = 0; i < Segments; i++)
            {
                currentPos = SharpMathHelper.Rotate(new Vector2(0, Distance), Vector2.Zero, rotation);
                this.dots.Add(currentPos);
                rotation += rnd.Next(-90, 90);
            }

            base.Initialize(resolver);
        }

        public override void Draw(SharpDrawBatch batch, GameTime time)
        {
            base.Draw(batch, time);

            var global = this.GlobalPosition;
            foreach (var dot in this.dots)
            {
                
            }
        }
    }
}