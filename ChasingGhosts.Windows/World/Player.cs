using ChasingGhosts.Windows.Interfaces;
using ChasingGhosts.Windows.ViewModels;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Sharp2D.Engine.Common;
using Sharp2D.Engine.Common.Components.Sprites;
using Sharp2D.Engine.Common.Scene;
using Sharp2D.Engine.Common.World;
using Sharp2D.Engine.Infrastructure;
using Sharp2D.Engine.Utility;

namespace ChasingGhosts.Windows.World
{
    public class Player : WorldObject, IMovableCharacter
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

            if (this.viewModel.IsAlive)
            {
                this.HandleMovement();
            }
        }

        private void HandleMovement()
        {
            var movement = Vector2.Zero;

            HandleKeyboard(ref movement);
            HandleGamepad(ref movement);
            this.HandleCursor(ref movement);

            this.Movement = movement * Mps;
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
}