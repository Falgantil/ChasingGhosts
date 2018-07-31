using System.Collections.Generic;
using System.Linq;

using ChasingGhosts.Windows.Interfaces;

using Microsoft.Xna.Framework;

using Sharp2D.Engine.Common.Components;
using Sharp2D.Engine.Common.ObjectSystem;
using Sharp2D.Engine.Infrastructure;

namespace ChasingGhosts.Windows.World
{
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
            obj.ChildObjectMoved -= this.Item_ChildObjectMoved;
            obj.ChildObjectMoved += this.Item_ChildObjectMoved;

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
                    player.RefreshMovement();
                    print.Dismiss();
                }
            }
        }

        private void HandleMovement(GameTime time)
        {
            var elapsedSeconds = (float)time.ElapsedGameTime.TotalSeconds;
            foreach (var character in this.characters)
            {
                var rawMov = character.Movement;
                if (rawMov == Vector2.Zero)
                {
                    continue;
                }
                rawMov.Normalize();
                var movement = (rawMov * character.MaxMovement) * elapsedSeconds;
                if (movement == Vector2.Zero)
                {
                    continue;
                }

                var startRegion = character.GlobalRegion;

                CheckMovement(this.walls, startRegion, ref movement);
                CheckMovement(this.characters.Where(c => c != character), startRegion, ref movement);

                character.LocalPosition += movement;
            }
        }

        private static void CheckMovement(IEnumerable<IPhysicsEntity> list, Rectanglef startRegion, ref Vector2 movement)
        {
            foreach (var wall in list)
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
}