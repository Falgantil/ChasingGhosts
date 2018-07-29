using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ChasingGhosts.Windows.Interfaces;
using ChasingGhosts.Windows.ViewModels;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Sharp2D.Engine.Common;
using Sharp2D.Engine.Common.Components.Animations;
using Sharp2D.Engine.Common.Components.Animations.Predefined;
using Sharp2D.Engine.Common.Components.Sprites;
using Sharp2D.Engine.Common.ObjectSystem;
using Sharp2D.Engine.Common.Scene;
using Sharp2D.Engine.Common.World;
using Sharp2D.Engine.Drawing;
using Sharp2D.Engine.Helper;
using Sharp2D.Engine.Infrastructure;
using Sharp2D.Engine.Utility;

namespace ChasingGhosts.Windows.World
{
    public class Player : GameObject, IMovableCharacter
    {
        private readonly PlayerViewModel viewModel;

        private FragmentedSpriteSheet sheetIdle;
        private FragmentedSpriteSheet sheetWalk;

        private EventValueAnimator animIdle;
        private EventValueAnimator animWalk;

        private Vector2 oldMovement;

        public Player(PlayerViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public override void Initialize(IResolver resolver)
        {
            this.Width = 128;
            this.Height = 144;
            var visual = new Sprite(Color.Red * .25f, (int)this.Width, (int)this.Height);
            visual.CenterObject();
            this.Components.Add(visual);

            this.CreateIdleAnimation();
            this.CreateWalkAnimation();
            
            base.Initialize(resolver);

            this.Direction = World.Movement.Down;
        }

        private string GetAnimationKey()
        {
            switch (this.Direction)
            {
                case World.Movement.Right:
                    return "right";
                case World.Movement.DownRight:
                    return "downright";
                case World.Movement.Down:
                    return "down";
                case World.Movement.DownLeft:
                    return "downleft";
                case World.Movement.Left:
                    return "left";
                case World.Movement.TopLeft:
                    return "topleft";
                case World.Movement.Top:
                    return "top";
                case World.Movement.TopRight:
                    return "topright";
                default:
                    return null;
            }
        }

        private void CreateIdleAnimation()
        {
            this.sheetIdle = new FragmentedSpriteSheet("diablo_idle", new SpriteSheetFragment
            {
                Groups = new List<SpriteSheetFragmentGroup>
                {
                    CreateFragmentGroup("down", 0, 10),
                    CreateFragmentGroup("downleft", 1, 10),
                    CreateFragmentGroup("left", 2, 10),
                    CreateFragmentGroup("topleft", 3, 10),
                    CreateFragmentGroup("top", 4, 10),
                    CreateFragmentGroup("topright", 5, 10),
                    CreateFragmentGroup("right", 6, 10),
                    CreateFragmentGroup("downright", 7, 10),
                }
            });
            this.sheetIdle.Scale = new Vector2(2);
            this.sheetIdle.IsVisible = true;
            this.Components.Add(this.sheetIdle);

            this.animIdle = new EventValueAnimator(TimeSpan.FromSeconds(.8f), true);
            this.animIdle.ValueUpdated += (s, val) =>
            {
                var keys = this.sheetIdle.Groups[this.GetAnimationKey()];
                var index = (int)(val * (keys.Count + 1));
                index = Math.Min(keys.Count - 1, index);
                var key = keys[index];
                this.sheetIdle.RegionKey = key;
            };
            this.animIdle.Easing = AnimationEase.Linear;
            this.animIdle.Loop = true;
            this.animIdle.IsPaused = false;
            this.Components.Add(this.animIdle);
        }

        private void CreateWalkAnimation()
        {
            this.sheetWalk = new FragmentedSpriteSheet("diablo_walk", new SpriteSheetFragment
            {
                Groups = new List<SpriteSheetFragmentGroup>
                {
                    CreateFragmentGroup("down", 0, 8),
                    CreateFragmentGroup("downleft", 1, 8),
                    CreateFragmentGroup("left", 2, 8),
                    CreateFragmentGroup("topleft", 3, 8),
                    CreateFragmentGroup("top", 4, 8),
                    CreateFragmentGroup("topright", 5, 8),
                    CreateFragmentGroup("right", 6, 8),
                    CreateFragmentGroup("downright", 7, 8),
                }
            });
            this.sheetWalk.Scale = new Vector2(2);
            this.sheetWalk.IsVisible = false;
            this.Components.Add(this.sheetWalk);

            this.animWalk = new EventValueAnimator(TimeSpan.FromSeconds(.8f), true);
            this.animWalk.ValueUpdated += (s, val) =>
            {
                var keys = this.sheetWalk.Groups[this.GetAnimationKey()];
                var index = (int)(val * keys.Count);
                index = Math.Min(keys.Count - 1, index);
                this.sheetWalk.RegionKey = keys[index];
            };
            this.animWalk.Easing = AnimationEase.Linear;
            this.animWalk.Loop = true;
            this.animWalk.IsPaused = true;
            this.Components.Add(this.animWalk);
        }

        private static SpriteSheetFragmentGroup CreateFragmentGroup(string groupName, int yStart, int frameCount)
        {
            const int Size = 96;
            return new SpriteSheetFragmentGroup
            {
                TransformOrigin = new Vector2(.5f, .5f),
                GroupName = groupName,
                Frames = Enumerable.Range(0, frameCount).Select(i => new Rectangle(i * Size, yStart * Size, Size, Size)).ToList()
            };
        }

        public Movement Direction { get; private set; }

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
            this.oldMovement = this.Movement;
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

        public override void Draw(SharpDrawBatch batch, GameTime time)
        {
            this.UpdateDirection();

            this.UpdateSpritesheets();

            base.Draw(batch, time);
        }

        private void UpdateSpritesheets()
        {
            if (this.Movement == Vector2.Zero && this.oldMovement != Vector2.Zero)
            {
                this.sheetWalk.IsVisible = false;
                this.animWalk.Stop();
                this.animWalk.IsPaused = true;

                this.sheetIdle.IsVisible = true;
                this.animIdle.Restart();
            }
            else if (this.Movement != Vector2.Zero && this.oldMovement == Vector2.Zero)
            {
                this.sheetIdle.IsVisible = false;
                this.animIdle.Stop();
                this.animIdle.IsPaused = true;

                this.sheetWalk.IsVisible = true;
                this.animWalk.Restart();
            }
        }

        private void UpdateDirection()
        {
            var direction = MovementHelper.GetMovement(this.Movement);
            if (direction != World.Movement.None)
            {
                this.Direction = direction;
            }
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