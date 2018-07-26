using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Sharp2D.Engine.Common;
using Sharp2D.Engine.Common.Components;
using Sharp2D.Engine.Common.Components.Sprites;
using Sharp2D.Engine.Common.ObjectSystem;
using Sharp2D.Engine.Common.Scene;
using Sharp2D.Engine.Common.World;
using Sharp2D.Engine.Common.World.Camera;
using Sharp2D.Engine.Helper;
using Sharp2D.Engine.Infrastructure;
using Sharp2D.Engine.Utility;
using Sharp2D.Windows.Keyboard;

namespace ChasingGhosts.Windows
{
    public class GameScene : Scene
    {
        private Player player;

        private PhysicsEngine physics;

        public GameScene(IResolver resolver)
            : base(resolver)
        {
        }

        public override void Initialize(IResolver resolver)
        {
            this.player = new Player();
            var camera = new Camera
            {
                MainCamera = true,

                EnableMovementTracking = true,
                Tracker = new CameraTracker
                {
                    Target = this.player,
                    EnablePositionTracking = true
                }
            };
            this.WorldRoot.Add(camera);

            this.WorldRoot.Add(new Wall
            {
                LocalPosition = new Vector2(-400, 0),
                Components =
                {
                    new Sprite(Color.Green, 128, 128)
                }
            });
            this.WorldRoot.Add(new Wall
            {
                LocalPosition = new Vector2(-200, 0),
                Components =
                {
                    new Sprite(Color.Green, 128, 128)
                }
            });

            //this.GeneratePath(camera);

            var generatedPath = new GeneratedPath();
            this.WorldRoot.Add(generatedPath);
            generatedPath.CreatePath(this.player.GlobalPosition);

            this.physics = new PhysicsEngine();
            this.WorldRoot.Components.Add(this.physics);

            this.WorldRoot.Add(this.player);

            base.Initialize(resolver);
        }

        private void GeneratePath(Camera cam)
        {
            var points = new[]
            {
                new Vector2(19, 27),
                new Vector2(79, 52),
                //new Vector2(122, 79),
                new Vector2(121, 118),
                new Vector2(114, 196),
                new Vector2(84, 284),
                new Vector2(79, 427),
                new Vector2(111, 564),
                new Vector2(265, 683),
                new Vector2(649, 702),
                new Vector2(963, 718),
                new Vector2(1109, 637),
                new Vector2(1165, 507),
                new Vector2(1158, 330),
                new Vector2(1085, 207),
                new Vector2(822, 144),
                new Vector2(573, 132),
                new Vector2(419, 172),
                new Vector2(264, 97),
                new Vector2(290, 32),
                new Vector2(373, 13),
            };

            foreach (var position in points)
            {
                var pos = Resolution.TransformPointToWorld(position, cam);
                this.WorldRoot.Add(new WorldObject
                {
                    LocalPosition = pos,
                    Components =
                    {
                        new Sprite(Color.Yellow, 5, 5)
                    }
                });
            }

            var foot = ShoeFoot.Right;

            for (var i = 0; i < points.Length - 1; i++)
            {
                var here = Resolution.TransformPointToWorld(points[i], cam);
                var next = Resolution.TransformPointToWorld(points[i + 1], cam);
                var degrees = MathHelper.ToDegrees((float)Math.Atan2(next.Y - here.Y, next.X - here.X));
                
                const float Threshold = 40f;
                while (Vector2.Distance(here, next) > Threshold)
                {
                    const float Offset = 10;

                    var distance = next - here;
                    distance.Normalize();
                    distance *= Threshold;
                    here += distance;
                    var shoeSprite = Sprite.Load("shoe");
                    shoeSprite.CenterObject();
                    shoeSprite.SpriteEffect = foot == ShoeFoot.Right ? SpriteEffects.None : SpriteEffects.FlipVertically;
                    this.WorldRoot.Add(new WorldObject
                    {
                        LocalPosition = here,
                        LocalRotation = degrees,
                        Children =
                        {
                            new WorldObject
                            {
                                LocalPosition = new Vector2(0, foot == ShoeFoot.Right ? Offset : -Offset),
                                Components =
                                {
                                    shoeSprite
                                }
                            }
                        }
                    });
                    foot = foot == ShoeFoot.Left ? ShoeFoot.Right : ShoeFoot.Left;
                }
            }
        }
    }

    public enum ShoeFoot
    {
        Right,
        Left
    }

    public class PhysicsEngine : Component
    {
        private readonly List<IWall> walls = new List<IWall>();
        private readonly List<IMovableCharacter> characters = new List<IMovableCharacter>();

        public override void Initialize(IResolver resolver)
        {
            base.Initialize(resolver);
            foreach (var obj in this.Parent.GetAllChildren())
            {
                if (obj is IWall w)
                {
                    this.walls.Add(w);
                }

                if (obj is IMovableCharacter mc)
                {
                    this.characters.Add(mc);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (var character in this.characters)
            {
                var movement = character.Movement * elapsedSeconds;
                if (movement == Vector2.Zero)
                {
                    continue;
                }

                var startRegion = character.GlobalRegion;

                foreach (var wall in this.walls)
                {
                    var endRegion = new Rectanglef(
                        startRegion.X + movement.X,
                        startRegion.Y + movement.Y,
                        startRegion.Width,
                        startRegion.Height);

                    var wallReg = wall.GlobalRegion;
                    if (!wallReg.Intersects(endRegion))
                    {
                        continue;
                    }

                    movement = new Vector2(CheckRight(startRegion, endRegion, wallReg) ?? movement.X, movement.Y);
                    movement = new Vector2(CheckLeft(startRegion, endRegion, wallReg) ?? movement.X, movement.Y);
                    movement = new Vector2(movement.X, CheckBottom(startRegion, endRegion, wallReg) ?? movement.Y);
                    movement = new Vector2(movement.X, CheckTop(startRegion, endRegion, wallReg) ?? movement.Y);
                }

                character.LocalPosition += movement;
            }
        }

        private static int? CheckTop(Rectanglef startRegion, Rectanglef endRegion, Rectanglef wallReg)
        {
            if (wallReg.Bottom >= endRegion.Top && wallReg.Bottom <= startRegion.Top)
            {
                return (int)(wallReg.Bottom - startRegion.Top);
            }

            return null;
        }

        private static int? CheckBottom(Rectanglef startRegion, Rectanglef endRegion, Rectanglef wallReg)
        {
            if (wallReg.Top <= endRegion.Bottom && wallReg.Top >= startRegion.Bottom)
            {
                return (int)(wallReg.Top - startRegion.Bottom);
            }

            return null;
        }

        private static int? CheckRight(Rectanglef startRegion, Rectanglef endRegion, Rectanglef wallReg)
        {
            if (wallReg.Left <= endRegion.Right && wallReg.Left >= startRegion.Right)
            {
                return (int)(wallReg.Left - startRegion.Right);
            }

            return null;
        }

        private static int? CheckLeft(Rectanglef startRegion, Rectanglef endRegion, Rectanglef wallReg)
        {
            if (wallReg.Right >= endRegion.Left && wallReg.Right <= startRegion.Left)
            {
                return (int)(wallReg.Right - startRegion.Left);
            }

            return null;
        }
    }

    public class Wall : WorldObject, IWall
    {

    }

    public class Player : WorldObject, IMovableCharacter
    {
        public override void Initialize(IResolver resolver)
        {
            var sprite = Sprite.Load("player");
            sprite.TransformOrigin = new Vector2(.5f, 2f / 3f);
            this.Add(new WorldObject
            {
                Components =
                {
                    sprite
                }
            });

            this.Width = 112;
            this.Height = 128;
            var visual = new Sprite(Color.Red * .25f, (int)this.Width, (int)this.Height);
            visual.CenterObject();
            this.Components.Add(visual);
            base.Initialize(resolver);
        }

        private const int Mps = 250;

        public Vector2 Movement { get; private set; }

        public override void Update(GameTime time)
        {
            base.Update(time);

            var movement = Vector2.Zero;

            if (InputManager.IsKeyDown(Keys.A))
            {
                movement -= new Vector2(1, 0);
            }

            if (InputManager.IsKeyDown(Keys.W))
            {
                movement -= new Vector2(0, 1);
            }

            if (InputManager.IsKeyDown(Keys.D))
            {
                movement += new Vector2(1, 0);
            }

            if (InputManager.IsKeyDown(Keys.S))
            {
                movement += new Vector2(0, 1);
            }

            this.Movement = movement * Mps;
        }
    }

    public interface IMovableCharacter
    {
        Vector2 Movement { get; }

        Vector2 LocalPosition { get; set; }

        Rectanglef GlobalRegion { get; }
    }

    public interface IWall
    {
        Rectanglef GlobalRegion { get; }
    }
}
