using System;
using System.Linq;
using System.Threading.Tasks;

using ChasingGhosts.Windows.UI;
using ChasingGhosts.Windows.ViewModels;
using ChasingGhosts.Windows.World;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Sharp2D.Engine.Common;
using Sharp2D.Engine.Common.Components.Sprites;
using Sharp2D.Engine.Common.Scene;
using Sharp2D.Engine.Common.World.Camera;
using Sharp2D.Engine.Infrastructure;
using Sharp2D.Engine.Utility;

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
            Sharp2DApplication.GameManager.BackgroundColor = Color.Gray;
        }

        public override async void Initialize(IResolver resolver)
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

            this.IsPaused = true;

            await Task.Delay(1500);
            await this.WaitForUpdate();

            this.IsPaused = false;
        }

        private void InitUi()
        {
            var bar = new HealthBar(this.playerVm);
            this.UiRoot.Add(bar);

            //var timer = new GameTimer(TimeSpan.FromSeconds(1))
            //{
            //    Looped = true
            //};
            //timer.Expired += (s, e) => this.playerVm.DamagePlayer(15);
            //this.UiRoot.Components.Add(timer);
        }

        public override void Update(GameTime gameTime)
        {
            this.WorldRoot.IsPaused = !this.playerVm.IsAlive;

            base.Update(gameTime);
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

            this.WorldRoot.Add(this.player);

            this.ShitsAndGiggles();

            this.physics = new PhysicsEngine();
            this.WorldRoot.Components.Add(this.physics);
        }

        private void ShitsAndGiggles()
        {
            this.WorldRoot.Add(new Wall { LocalPosition = new Vector2(-400, 0), Components = { new Sprite(Color.Green, 128, 128) } });
            this.WorldRoot.Add(new Wall { LocalPosition = new Vector2(-200, 0), Components = { new Sprite(Color.Green, 128, 128) } });

            var generatedPath = new GeneratedPath();
            this.WorldRoot.Add(generatedPath);
            generatedPath.CreatePath(this.player.GlobalPosition);

            this.WorldRoot.Add(new Npc(this.player, this.playerVm)
            {
                LocalPosition = new Vector2(-100, -500)
            });
        }
    }
}
