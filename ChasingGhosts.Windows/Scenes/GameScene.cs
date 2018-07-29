using System;

using ChasingGhosts.Windows.UI;
using ChasingGhosts.Windows.ViewModels;
using ChasingGhosts.Windows.World;

using Microsoft.Xna.Framework;

using Sharp2D.Engine.Common;
using Sharp2D.Engine.Common.Components.Sprites;
using Sharp2D.Engine.Common.Scene;
using Sharp2D.Engine.Common.World.Camera;
using Sharp2D.Engine.Infrastructure;

namespace ChasingGhosts.Windows.Scenes
{
    public class GameScene : Scene
    {
        private Player player;

        private PhysicsEngine physics;

        private PlayerViewModel playerVm;

        public GameScene(IResolver resolver)
            : base(resolver)
        {
        }

        public override void Initialize(IResolver resolver)
        {
            this.playerVm = new PlayerViewModel();
            this.playerVm.Dies += async (s, e) =>
            {
                await this.UiRoot.WaitForUpdate();
                this.UiRoot.Add(new DeathScreen());
            };
            this.InitWorld();
            this.InitUi();

            base.Initialize(resolver);
        }

        private void InitUi()
        {
            var bar = new HealthBar(this.playerVm);
            this.UiRoot.Add(bar);

            var timer = new GameTimer(TimeSpan.FromSeconds(1))
            {
                Looped = true
            };
            timer.Expired += (s, e) => this.playerVm.Health = this.playerVm.Health - 15f;
            this.UiRoot.Components.Add(timer);
        }

        private void InitWorld()
        {
            this.player = new Player(this.playerVm);
            var camera = new Camera
            {
                MainCamera = true,
                EnableMovementTracking = true,
                Tracker = new CameraTracker { Target = this.player, EnablePositionTracking = true }
            };
            this.WorldRoot.Add(camera);

            this.ShitsAndGiggles();

            this.physics = new PhysicsEngine();
            this.WorldRoot.Components.Add(this.physics);

            this.WorldRoot.Add(this.player);
        }

        private void ShitsAndGiggles()
        {
            this.WorldRoot.Add(new Wall { LocalPosition = new Vector2(-400, 0), Components = { new Sprite(Color.Green, 128, 128) } });
            this.WorldRoot.Add(new Wall { LocalPosition = new Vector2(-200, 0), Components = { new Sprite(Color.Green, 128, 128) } });

            var generatedPath = new GeneratedPath();
            this.WorldRoot.Add(generatedPath);
            generatedPath.CreatePath(this.player.GlobalPosition);
        }
    }
}
