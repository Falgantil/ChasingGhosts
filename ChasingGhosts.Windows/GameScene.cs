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

            var generatedPath = new GeneratedPath();
            this.WorldRoot.Add(generatedPath);
            generatedPath.CreatePath(this.player.GlobalPosition);

            this.physics = new PhysicsEngine();
            this.WorldRoot.Components.Add(this.physics);

            this.WorldRoot.Add(this.player);

            base.Initialize(resolver);
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
        private readonly List<IShoePrint> shoeprints = new List<IShoePrint>();

        public override void Initialize(IResolver resolver)
        {
            base.Initialize(resolver);

            this.Hook(this.Parent);
        }

        private void Hook(GameObject obj)
        {
            obj.ChildObjectMoved -= Item_ChildObjectMoved;
            obj.ChildObjectMoved += Item_ChildObjectMoved;

            if (obj is IWall w)
            {
                if (!this.walls.Contains(w))
                    this.walls.Add(w);
            }

            if (obj is IMovableCharacter mc)
            {
                if (!this.characters.Contains(mc))
                    this.characters.Add(mc);
            }

            if (obj is IShoePrint sp)
            {
                if (!this.shoeprints.Contains(sp))
                    this.shoeprints.Add(sp);
            }

            foreach (var child in obj.Children)
            {
                this.Hook(child);
            }
        }

        private void Item_ChildObjectMoved(GameObject sender, ChildObjectMovedArgs args)
        {
            switch (args.Action)
            {
                case ChildObjectMoveAction.Added:
                case ChildObjectMoveAction.Inserted:
                    this.Hook(args.Child);
                    break;
                case ChildObjectMoveAction.Removed:
                    args.Child.ChildObjectMoved -= this.Item_ChildObjectMoved;
                    break;
            }
        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            this.HandleMovement(time);

            this.HandlePlayerPrints();
        }

        private void HandlePlayerPrints()
        {
            var player = this.characters.OfType<Player>().First();
            var plrReg = player.GlobalRegion;
            foreach (var print in this.shoeprints.ToArray())
            {
                if (!print.IsActive)
                {
                    this.shoeprints.Remove(print);
                    continue;
                }
                var center = print.GlobalRegion.Center;
                if (plrReg.Contains((int)center.X, (int)center.Y))
                {
                    print.Dismiss();
                }
            }
        }

        private void HandleMovement(GameTime time)
        {
            var elapsedSeconds = (float)time.ElapsedGameTime.TotalSeconds;
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
                    var endRegion = new Rectanglef(startRegion.X + movement.X, startRegion.Y + movement.Y, startRegion.Width, startRegion.Height);

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
