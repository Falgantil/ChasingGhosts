using System;
using System.Collections.Generic;
using System.Linq;

using ChasingGhosts.Windows.Interfaces;
using ChasingGhosts.Windows.ViewModels;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Sharp2D.Engine.Common;
using Sharp2D.Engine.Common.Components;
using Sharp2D.Engine.Common.Components.Animations;
using Sharp2D.Engine.Common.Components.Animations.Predefined;
using Sharp2D.Engine.Common.Components.Sprites;
using Sharp2D.Engine.Common.ObjectSystem;
using Sharp2D.Engine.Common.Scene;
using Sharp2D.Engine.Drawing;
using Sharp2D.Engine.Helper;
using Sharp2D.Engine.Infrastructure;
using Sharp2D.Engine.Utility;

namespace ChasingGhosts.Windows.World
{
    public class Player : GameObject, IMovableCharacter
    {
        private readonly PlayerViewModel viewModel;

        private Vector2 oldMovement;

        private MovementSprite spriteWalk;

        private MovementSprite spriteIdle;

        private EventValueAnimator speedReducer;

        private float topSpeed = 250;

        public Player(PlayerViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.MaxMovement = this.topSpeed;
        }

        public override void Initialize(IResolver resolver)
        {
            this.Width = 56;
            this.Height = 56;
            var visual = new Sprite(Color.Red * .25f, (int)this.Width, (int)this.Height);
            visual.CenterObject();
            visual.IsVisible = false;
            this.Components.Add(visual);

            this.spriteIdle = new MovementSprite("player_walk", TimeSpan.FromSeconds(1.5f))
            {
                Scale = 3f
            };
            this.spriteWalk = new MovementSprite("player_walk", TimeSpan.FromSeconds(.5f))
            {
                Scale = 3f
            };
            this.Components.Add(this.spriteIdle);
            this.Components.Add(this.spriteWalk);
            this.spriteIdle.Start();

            this.speedReducer = new EventValueAnimator(TimeSpan.FromSeconds(5));
            this.speedReducer.ValueUpdated += (s, perc) =>
            {
                var halfSpeed = this.topSpeed / 2f;
                this.MaxMovement = halfSpeed + halfSpeed * (1 - perc);
            };
            this.Components.Add(this.speedReducer);

            this.speedReducer.Start();

            base.Initialize(resolver);

            this.Direction = World.Movement.Down;
            this.EquipShoe(ShoeType.FlipFlops);
        }

        public Movement Direction { get; private set; }

        public float MaxMovement { get; private set; }

        public Vector2 Movement { get; private set; }

        public override void Update(GameTime time)
        {
            base.Update(time);

            this.HandleMovement(time);

            this.HandleHealth(time);
        }

        private void HandleHealth(GameTime time)
        {
            if (this.ShoeType != ShoeType.FlipFlops)
            {
                return;
            }

            if (this.viewModel.Health >= 100)
            {
                return;
            }

            this.viewModel.HealPlayer((float)time.ElapsedGameTime.TotalSeconds * 3);
        }

        private void HandleMovement(GameTime time)
        {
            var movement = this.ShoeType == ShoeType.Rollerblades ? this.Movement : Vector2.Zero;

            this.HandleKeyboard(ref movement, time);
            this.HandleGamepad(ref movement, time);
            this.HandleCursor(ref movement, time);
            this.oldMovement = this.Movement;
            this.Movement = movement;
            if (this.ShoeType == ShoeType.Rollerblades && movement != Vector2.Zero && Vector2.Distance(movement, Vector2.Zero) > 2)
            {
                movement.Normalize();
                movement *= 2;
                this.Movement = movement;
            }
        }

        private const float RollerbladeDelay = 2;

        private void HandleCursor(ref Vector2 movement, GameTime time)
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

            if (this.ShoeType == ShoeType.Rollerblades)
                m *= RollerbladeDelay * (float)time.ElapsedGameTime.TotalSeconds;

            movement += m;
        }

        private void HandleGamepad(ref Vector2 movement, GameTime time)
        {
            var m = GamePadManager.LeftThumbstickMovement();
            m = new Vector2(m.X, -m.Y);
            if (m == Vector2.Zero)
            {
                return;
            }

            if (this.ShoeType == ShoeType.Rollerblades)
                m *= RollerbladeDelay * (float)time.ElapsedGameTime.TotalSeconds;

            movement += m;
        }

        private void HandleKeyboard(ref Vector2 movement, GameTime time)
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

            if (this.ShoeType == ShoeType.Rollerblades)
                m *= RollerbladeDelay * (float)time.ElapsedGameTime.TotalSeconds;

            movement += m;
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
                this.spriteWalk.Stop();
                this.spriteIdle.Start();
            }
            else if (this.Movement != Vector2.Zero && this.oldMovement == Vector2.Zero)
            {
                this.spriteWalk.Start();
                this.spriteIdle.Stop();
            }
        }

        private void UpdateDirection()
        {
            var direction = MovementHelper.GetMovement(this.Movement);
            if (direction != World.Movement.None)
            {
                this.spriteIdle.Direction = direction;
                this.spriteWalk.Direction = direction;
            }
        }

        public void RefreshMovement()
        {
            this.MaxMovement = this.topSpeed;
            this.speedReducer.Restart();
        }

        public void EquipShoe(ShoeType type)
        {
            this.ShoeType = type;
            switch (type)
            {
                case ShoeType.None:
                    this.topSpeed = 225;
                    break;
                case ShoeType.Sneakers:
                    this.topSpeed = 250;
                    break;
                case ShoeType.Rollerblades:
                    this.topSpeed = 350;
                    break;
                case ShoeType.FlipFlops:
                    this.topSpeed = 200;
                    break;
            }
            this.RefreshMovement();
        }

        public ShoeType ShoeType { get; private set; }
    }

    public enum ShoeType
    {
        None,
        Sneakers,
        Rollerblades,
        FlipFlops
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

            const float HalfCoverage = 45;

            if (TestDirection(rotation, -HalfCoverage, HalfCoverage) || rotation > 360 - HalfCoverage)
            {
                // Right
                return Movement.Right;
            }

            if (TestDirection(rotation, HalfCoverage, HalfCoverage * 3f))
            {
                // Right
                return Movement.Down;
            }

            if (TestDirection(rotation, HalfCoverage * 3f, HalfCoverage * 5f))
            {
                // Right
                return Movement.Left;
            }

            if (TestDirection(rotation, HalfCoverage * 5f, HalfCoverage * 7f))
            {
                // Right
                return Movement.Top;
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
        Down,
        Left,
        Top
    }

    public class MovementSprite : Component
    {
        private readonly string assetName;

        private readonly TimeSpan duration;

        private FragmentedSpriteSheet sheet;

        private EventValueAnimator anim;

        private bool started;

        private float scale = 2;

        public MovementSprite(string assetName, TimeSpan duration)
        {
            this.assetName = assetName;
            this.duration = duration;
        }

        public float Scale
        {
            get => this.scale;
            set
            {
                this.scale = value;
                if (this.sheet != null)
                {
                    this.sheet.Scale = new Vector2(value);
                }
            }
        }

        public override void Initialize(IResolver resolver)
        {
            this.sheet = new FragmentedSpriteSheet(this.assetName, new SpriteSheetFragment
            {
                Groups = new List<SpriteSheetFragmentGroup>
                {
                    CreateFragmentGroup("down", 0, 4),
                    CreateFragmentGroup("right", 1, 4),
                    CreateFragmentGroup("left", 2, 4),
                    CreateFragmentGroup("top", 3, 4)
                }
            });
            this.sheet.Scale = new Vector2(this.Scale);
            this.sheet.IsVisible = true;
            this.Children.Add(this.sheet);

            this.anim = new EventValueAnimator(this.duration, true);
            this.anim.ValueUpdated += (s, val) =>
            {
                var key = this.GetAnimationKey();
                if (string.IsNullOrEmpty(key))
                {
                    return;
                }
                var keys = this.sheet.Groups[key];
                var index = (int)(val * keys.Count);
                index = Math.Min(keys.Count - 1, index);
                this.sheet.RegionKey = keys[index];
            };
            this.anim.Easing = AnimationEase.Linear;
            this.anim.Loop = true;
            this.anim.IsPaused = false;
            this.Children.Add(this.anim);

            base.Initialize(resolver);

            if (this.started)
            {
                this.Start();
            }
        }

        private string GetAnimationKey()
        {
            switch (this.Direction)
            {
                case Movement.Right:
                    return "right";
                case Movement.Down:
                    return "down";
                case Movement.Left:
                    return "left";
                case Movement.Top:
                    return "top";
                default:
                    return null;
            }
        }

        public Movement Direction { get; set; }

        private static SpriteSheetFragmentGroup CreateFragmentGroup(string groupName, int yStart, int frameCount)
        {
            const int Size = 48;
            return new SpriteSheetFragmentGroup
            {
                TransformOrigin = new Vector2(.5f, .6f),
                GroupName = groupName,
                Frames = Enumerable.Range(0, frameCount).Select(i => new Rectangle(i * Size, yStart * Size, Size, Size)).ToList()
            };
        }

        public void Stop()
        {
            this.started = false;
            this.IsVisible = false;
            this.IsPaused = true;
            this.anim.Stop();
        }

        public void Start()
        {
            this.started = true;
            this.IsVisible = true;
            this.IsPaused = false;
            this.anim?.Restart();
        }
    }
}