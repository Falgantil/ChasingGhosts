using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Sharp2D.Engine.Common.Components.Sprites;
using Sharp2D.Engine.Common.ObjectSystem;
using Sharp2D.Engine.Common.Scene;
using Sharp2D.Engine.Common.World;
using Sharp2D.Engine.Common.World.Camera;
using Sharp2D.Engine.Infrastructure;
using Sharp2D.Engine.Utility;

namespace ChasingGhosts.Windows
{
    //public class TopDownScene : Scene
    //{
    //    private Player player;

    //    public TopDownScene(IResolver resolver)
    //        : base(resolver)
    //    {
    //    }

    //    public override void Initialize(IResolver resolver)
    //    {
    //        this.player = new Player();
    //        this.WorldRoot.Add(this.player);
    //        this.WorldRoot.Add(new Camera
    //        {
    //            MainCamera = true,

    //            // EnableMovementTracking = true,
    //            // Tracker = new CameraTracker
    //            // {
    //            // Target = this.player,
    //            // EnablePositionTracking = true
    //            // }
    //        });
    //        this.WorldRoot.Add(new Shoe(ShoeFoot.Right)
    //        {
    //            LocalPosition = new Vector2(-400, -0),
    //            LocalRotation = -90f
    //        });
    //        this.WorldRoot.Add(new Shoe(ShoeFoot.Left)
    //        {
    //            LocalPosition = new Vector2(-400, 0),
    //            LocalRotation = -90f
    //        });

    //        base.Initialize(resolver);
    //    }
    //}

    //public class Shoe : WorldObject
    //{
    //    public Shoe(ShoeFoot foot)
    //    {
    //        var sprite = Sprite.Load("shoe");
    //        sprite.SpriteEffect = foot == ShoeFoot.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
    //        sprite.CenterObject();

    //        float offset = 10;
    //        this.Children.Add(new WorldObject
    //        {
    //            Components =
    //            {
    //                sprite
    //            },
    //            LocalPosition = new Vector2(foot == ShoeFoot.Right ? offset : -offset, 0)
    //        });
    //    }

    //    public override void Update(GameTime time)
    //    {
    //        base.Update(time);
    //        this.LocalRotation += 90 * (float)time.ElapsedGameTime.TotalSeconds;
    //    }
    //}

    //public enum ShoeFoot
    //{
    //    Right,
    //    Left
    //}

    //public class Player : WorldObject
    //{
    //    private WorldObject sprite;

    //    public override void Initialize(IResolver resolver)
    //    {
    //        var sprite = Sprite.Load("player");
    //        sprite.CenterObject();

    //        this.Children.Add(this.sprite = new WorldObject
    //        {
    //            Components =
    //            {
    //                sprite
    //            }
    //        });

    //        base.Initialize(resolver);
    //    }

    //    private const int Mps = 250;

    //    public override void Update(GameTime time)
    //    {
    //        float elapsedSeconds = (float)time.ElapsedGameTime.TotalSeconds;
    //        base.Update(time);

    //        var movement = Vector2.Zero;

    //        if (InputManager.IsKeyDown(Keys.A))
    //        {
    //            movement -= new Vector2(1, 0);
    //        }

    //        if (InputManager.IsKeyDown(Keys.W))
    //        {
    //            movement -= new Vector2(0, 1);
    //        }

    //        if (InputManager.IsKeyDown(Keys.D))
    //        {
    //            movement += new Vector2(1, 0);
    //        }

    //        if (InputManager.IsKeyDown(Keys.S))
    //        {
    //            movement += new Vector2(0, 1);
    //        }

    //        if (movement == Vector2.Zero)
    //        {
    //            return;
    //        }

    //        movement.Normalize();

    //        var direction = Direction.None;
    //        if (movement.X > 0)
    //        {
    //            direction |= Direction.Right;
    //        }
    //        else if (movement.X < 0)
    //        {
    //            direction |= Direction.Left;
    //        }
    //        if (movement.Y > 0)
    //        {
    //            direction |= Direction.Top;
    //        }
    //        else if (movement.Y < 0)
    //        {
    //            direction |= Direction.Bottom;
    //        }

    //        if (direction == Direction.TopLeft)
    //        {
    //            this.sprite.LocalRotation = -45f;
    //        }
    //        else if (direction == Direction.TopRight)
    //        {
    //            this.sprite.LocalRotation = 45f;
    //        }
    //        else if (direction == Direction.BottomRight)
    //        {
    //            this.sprite.LocalRotation = 45f * 3f;
    //        }
    //        else if (direction == Direction.BottomLeft)
    //        {
    //            this.sprite.LocalRotation = -45f * 3f;
    //        }
    //        else
    //        {
    //            switch (direction)
    //            {
    //                case Direction.None:
    //                    break;
    //                case Direction.Left:
    //                    this.sprite.LocalRotation = -90f;
    //                    break;
    //                case Direction.Top:
    //                    this.sprite.LocalRotation = 0f;
    //                    break;
    //                case Direction.Right:
    //                    this.sprite.LocalRotation = 90f;
    //                    break;
    //                case Direction.Bottom:
    //                    this.sprite.LocalRotation = 180f;
    //                    break;
    //            }
    //        }

    //        this.LocalPosition += movement * Mps * elapsedSeconds;
    //    }
    //}

    //[Flags]
    //public enum Direction
    //{
    //    None = 0,
    //    Left = 1 << 0,
    //    Top = 1 << 1,
    //    Right = 1 << 2,
    //    Bottom = 1 << 3,
    //    TopLeft = Top | Left,
    //    TopRight = Top | Right,
    //    BottomRight = Bottom | Right,
    //    BottomLeft = Bottom | Left
    //}
}
