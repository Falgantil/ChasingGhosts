using System;
using System.Collections.Generic;
using System.Linq;

using ChasingGhosts.Windows.World;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Sharp2D.Engine.Common;
using Sharp2D.Engine.Common.Components.Animations;
using Sharp2D.Engine.Common.Components.Animations.Predefined;
using Sharp2D.Engine.Common.Components.Sprites;
using Sharp2D.Engine.Common.ObjectSystem;
using Sharp2D.Engine.Common.World;
using Sharp2D.Engine.Drawing;
using Sharp2D.Engine.Helper;
using Sharp2D.Engine.Infrastructure;

namespace ChasingGhosts.Windows
{
    public class GeneratedPath : GameObject
    {
        private const int Distance = 250;

        public override void Initialize(IResolver resolver)
        {
            GenerateDots(15);

            base.Initialize(resolver);
        }

        public GamePath CreatePath(Vector2 playerPosition)
        {
            var start = playerPosition - this.GlobalPosition;

            foreach (var gamePath in this.Children.OfType<GamePath>().ToArray())
            {
                this.Remove(gamePath);
            }

            var path = new GamePath(GenerateDots(15).Select(v => start + v).ToArray());
            this.Add(path);
            return path;
        }

        private static Vector2[] GenerateDots(int segments)
        {
            var dots = new List<Vector2>();

            var rnd = new Random();
            var rotation = rnd.Next(0, 360);
            var currentPos = Vector2.Zero;
            dots.Add(currentPos);
            for (int i = 0; i < segments; i++)
            {
                currentPos += SharpMathHelper.Rotate(new Vector2(0, Distance), Vector2.Zero, rotation);
                dots.Add(currentPos);
                const int MaxRotation = 75;
                rotation += rnd.Next(-MaxRotation, MaxRotation);
            }

            return dots.ToArray();
        }
    }

    public class GamePath : GameObject
    {
        private readonly Vector2[] dots;

        public GamePath(Vector2[] dots)
        {
            var root = dots.First();
            if (root != Vector2.Zero)
            {
                dots = dots.Select(v => v - root).ToArray();
            }

            this.dots = dots;
        }

        public override void Initialize(IResolver resolver)
        {
            var foot = ShoeFoot.Right;

            for (var i = 0; i < this.dots.Length - 1; i++)
            {
                var here = this.dots[i];
                var next = this.dots[i + 1];
                var degrees = MathHelper.ToDegrees((float)Math.Atan2(next.Y - here.Y, next.X - here.X));

                const float Threshold = 40f;
                while (Vector2.Distance(here, next) > Threshold)
                {

                    var distance = next - here;
                    distance.Normalize();
                    distance *= Threshold;
                    here += distance;

                    this.Add(new ShoePrint(here, degrees, foot));
                    foot = foot == ShoeFoot.Left ? ShoeFoot.Right : ShoeFoot.Left;
                }
            }


            base.Initialize(resolver);
        }
    }

    public class ShoePrint : GameObject, IShoePrint
    {
        private Sprite shoeSprite;

        public ShoePrint(Vector2 localPosition, float turnDegrees, ShoeFoot foot)
        {
            this.LocalPosition = localPosition;
            this.LocalRotation = turnDegrees;

            const float Offset = 10;
            this.shoeSprite = Sprite.Load("shoe");
            this.shoeSprite.Scale = new Vector2(.8f);
            this.shoeSprite.CenterObject();
            this.shoeSprite.SpriteEffect = foot == ShoeFoot.Right ? SpriteEffects.None : SpriteEffects.FlipVertically;
            this.Add(
                new GameObject
                {
                    LocalPosition = new Vector2(0, foot == ShoeFoot.Right ? Offset : -Offset),
                    Components =
                    {
                        this.shoeSprite
                    }
                });
        }

        public bool IsActive { get; private set; } = true;

        public Color Tint
        {
            get => this.shoeSprite.Tint;
            set => this.shoeSprite.Tint = value;
        }

        public void Dismiss()
        {
            this.IsActive = false;
            var animation = ValueAnimator.PlayAnimation(this, val => this.shoeSprite.Tint = Color.White * val, TimeSpan.FromSeconds(.5f));
            animation.Easing = AnimationEase.CubicEaseOut;
            animation.Loop = false;
            animation.Inverse = true;
        }
    }

    public interface IShoePrint
    {
        bool IsActive { get; }

        void Dismiss();

        Rectanglef GlobalRegion { get; }
    }
}