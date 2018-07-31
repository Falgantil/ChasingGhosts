using System;
using System.Diagnostics;
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
using Sharp2D.Engine.Common.UI.Controls;
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

            this.AddFootprints();

            this.WorldRoot.Add(this.player);

            this.physics = new PhysicsEngine();
            this.WorldRoot.Components.Add(this.physics);
        }

        private void AddFootprints()
        {
            var path = new[]
            {
                new Vector2(68f, 82f),
                new Vector2(103f, 143f),
                new Vector2(161f, 174f),
                new Vector2(229f, 183f),
                new Vector2(267f, 207f),
                new Vector2(277f, 249f),
                new Vector2(280f, 313f),
                new Vector2(263f, 391f),
                new Vector2(212f, 433f),
                new Vector2(182f, 467f),
                new Vector2(180f, 511f),
                new Vector2(141f, 530f),
                new Vector2(138f, 571f),
                new Vector2(105f, 631f),
                new Vector2(132f, 710f),
                new Vector2(257f, 731f),
                new Vector2(313f, 729f),
                new Vector2(373f, 702f),
                new Vector2(428f, 659f),
                new Vector2(452f, 593f),
                new Vector2(455f, 541f),
                new Vector2(428f, 510f),
                new Vector2(445f, 469f),
                new Vector2(495f, 449f),
                new Vector2(566f, 444f),
                new Vector2(651f, 450f),
                new Vector2(725f, 454f),
                new Vector2(770f, 500f),
                new Vector2(783f, 579f),
                new Vector2(768f, 640f),
                new Vector2(812f, 660f),
                new Vector2(854f, 645f),
                new Vector2(886f, 624f),
                new Vector2(902f, 557f),
                new Vector2(895f, 479f),
                new Vector2(868f, 416f),
                new Vector2(819f, 330f),
                new Vector2(745f, 273f),
                new Vector2(685f, 234f),
                new Vector2(679f, 186f),
                new Vector2(749f, 143f),
                new Vector2(838f, 102f),
                new Vector2(943f, 78f),
                new Vector2(1011f, 78f),
                new Vector2(1082f, 107f),
                new Vector2(1158f, 136f),
                new Vector2(1189f, 186f),
                new Vector2(1219f, 254f),
                new Vector2(1237f, 335f),
                new Vector2(1246f, 433f),
                new Vector2(1227f, 551f),
                new Vector2(1194f, 596f),
                new Vector2(1176f, 678f),
                new Vector2(1160f, 711f),
                new Vector2(1108f, 737f),
                new Vector2(1066f, 757f)
            };

            foreach (var p in path)
            {
                AddToPath(p);
            }

            path = path.Select(v => v * 5f).ToArray();

            var gamePath = new GamePath(path);

            this.WorldRoot.Add(gamePath);
            var diff = this.player.GlobalPosition - gamePath.GlobalPosition;
            gamePath.LocalPosition += diff;
        }

        private void ShitsAndGiggles()
        {
            //this.WorldRoot.Add(new Wall { LocalPosition = new Vector2(-400, 0), Components = { new Sprite(Color.Green, 128, 128) } });
            //this.WorldRoot.Add(new Wall { LocalPosition = new Vector2(-200, 0), Components = { new Sprite(Color.Green, 128, 128) } });

            //var generatedPath = new GeneratedPath();
            //this.WorldRoot.Add(generatedPath);
            //generatedPath.CreatePath(this.player.GlobalPosition);

            this.WorldRoot.Add(new Npc(this.player, this.playerVm)
            {
                LocalPosition = new Vector2(-100, -500)
            });
        }

        public override void Update(GameTime gameTime)
        {
            this.WorldRoot.IsPaused = !this.playerVm.IsAlive;

            if (InputManager.IsLeftButtonPressed)
            {
                var pos = InputManager.MousePosition;

                Debug.WriteLine($"new Vector2({pos.X}f, {pos.Y}f),");

                var print = this.AddToPath(pos);
                print.Tint = Color.Red;
            }

            base.Update(gameTime);
        }

        private ShoePrint AddToPath(Vector2 pos)
        {
            var shoePrint = new ShoePrint(pos, 0, ShoeFoot.Right);
            this.UiRoot.Add(shoePrint);
            return shoePrint;
        }
    }
}
