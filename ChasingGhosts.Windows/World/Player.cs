using System;
using System.Diagnostics;

using ChasingGhosts.Windows.Interfaces;
using ChasingGhosts.Windows.ViewModels;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Sharp2D.Engine.Common;
using Sharp2D.Engine.Common.Components.Sprites;
using Sharp2D.Engine.Common.ObjectSystem;
using Sharp2D.Engine.Common.Scene;
using Sharp2D.Engine.Common.World;
using Sharp2D.Engine.Helper;
using Sharp2D.Engine.Infrastructure;
using Sharp2D.Engine.Utility;

namespace ChasingGhosts.Windows.World
{
    public class Player : GameObject, IMovableCharacter
    {
        private readonly PlayerViewModel viewModel;

        public Player(PlayerViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public override void Initialize(IResolver resolver)
        {
            var sprite = Sprite.Load("player");
            sprite.TransformOrigin = new Vector2(.5f, 2f / 3f);
            this.Add(new GameObject
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

        public float MaxMovement => 250;

        public Vector2 Movement { get; private set; }

        public override void Update(GameTime time)
        {
            base.Update(time);

            this.HandleMovement();
        }

        private void HandleMovement()
        {
            var movement = Vector2.Zero;

            HandleKeyboard(ref movement);
            HandleGamepad(ref movement);
            this.HandleCursor(ref movement);
            Debug.WriteLine(MovementHelper.GetMovement(movement));
            this.Movement = movement;
        }

        private void HandleCursor(ref Vector2 movement)
        {
            if (!InputManager.IsLeftButtonDown)
            {
                return;
            }

            var pos = InputManager.MousePosition;
            var worldPos = Resolution.TransformPointToWorld(pos, Scene.CurrentScene.MainCamera);
            var m = worldPos - this.GlobalPosition;
            m.Normalize();

            if (m == Vector2.Zero)
            {
                return;
            }

            movement = m;
        }

        private static void HandleGamepad(ref Vector2 movement)
        {
            var m = GamePadManager.LeftThumbstickMovement();
            m = new Vector2(m.X, -m.Y);
            if (m == Vector2.Zero)
            {
                return;
            }
            movement = m;
        }

        private static void HandleKeyboard(ref Vector2 movement)
        {
            var m = Vector2.Zero;
            if (InputManager.IsKeyDown(Keys.A))
            {
                m -= new Vector2(1, 0);
            }

            if (InputManager.IsKeyDown(Keys.W))
            {
                m -= new Vector2(0, 1);
            }

            if (InputManager.IsKeyDown(Keys.D))
            {
                m += new Vector2(1, 0);
            }

            if (InputManager.IsKeyDown(Keys.S))
            {
                m += new Vector2(0, 1);
            }
            if (m == Vector2.Zero)
            {
                return;
            }

            m.Normalize();
            movement = m;
        }
    }

    public static class MovementHelper
    {
        public static Movement GetMovement(Vector2 direction)
        {
            if (direction == Vector2.Zero)
            {
                return Movement.None;
            }
            var degrees = MathHelper.ToDegrees((float)Math.Atan2(direction.Y, direction.X));
            return GetMovement(degrees);
        }

        public static Movement GetMovement(float rotation)
        {
            rotation = SharpMathHelper.Loop(0, 360, rotation);

            const float HalfCoverage = 22.5f;

            if (TestDirection(rotation, -HalfCoverage, HalfCoverage)) // Right
            {
                return Movement.Right;
            }
            if (TestDirection(rotation, HalfCoverage, HalfCoverage * 3f)) // Right
            {
                return Movement.DownRight;
            }
            if (TestDirection(rotation, HalfCoverage * 3f, HalfCoverage * 5f)) // Right
            {
                return Movement.Down;
            }
            if (TestDirection(rotation, HalfCoverage * 5f, HalfCoverage * 7f)) // Right
            {
                return Movement.DownLeft;
            }
            if (TestDirection(rotation, HalfCoverage * 7f, HalfCoverage * 9f)) // Right
            {
                return Movement.Left;
            }
            if (TestDirection(rotation, HalfCoverage * 9f, HalfCoverage * 11f)) // Right
            {
                return Movement.TopLeft;
            }
            if (TestDirection(rotation, HalfCoverage * 11f, HalfCoverage * 13f)) // Right
            {
                return Movement.Top;
            }
            if (TestDirection(rotation, HalfCoverage * 13f, HalfCoverage * 15f)) // Right
            {
                return Movement.TopRight;
            }

            return Movement.None;
        }

        private static bool TestDirection(float rotation, float start, float end)
        {
            bool TestDirection() => start < rotation && rotation <= end;
            if (TestDirection())
            {
                return true;
            }
            if (start < 0)
            {
                start = SharpMathHelper.Loop(0, 360, start);
            }
            if (end < 0)
            {
                end = SharpMathHelper.Loop(0, 360, start);
            }

            return TestDirection();
        }
    }

    public enum Movement
    {
        None,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        TopLeft,
        Top,
        TopRight
    }
}