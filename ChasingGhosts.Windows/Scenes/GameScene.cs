using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using ChasingGhosts.Windows.Interfaces;
using ChasingGhosts.Windows.UI;
using ChasingGhosts.Windows.ViewModels;
using ChasingGhosts.Windows.World;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Sharp2D.Engine.Common;
using Sharp2D.Engine.Common.Components;
using Sharp2D.Engine.Common.Components.Audio;
using Sharp2D.Engine.Common.Scene;
using Sharp2D.Engine.Common.UI.Controls;
using Sharp2D.Engine.Common.UI.Enums;
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

        private IMusicManager musicManager;

        private Label lblScore;

        private int score;

        public GameScene(IResolver resolver)
            : base(resolver)
        {
            Sharp2DApplication.GameManager.BackgroundColor = Color.Gray;
        }

        public override async void Initialize(IResolver resolver)
        {
            this.playerVm = new PlayerViewModel();
            this.InitWorld();
            this.InitUi();

            this.musicManager = resolver.Resolve<IMusicManager>();
            this.musicManager.LoadSongs("Audio/LOOP_1", "Audio/LOOP_2", "Audio/LOOP_3", "Audio/LOOP_4", "Audio/LOOP_5");
            if (this.musicManager is Component musicComp)
            {
                this.WorldRoot.Components.Add(musicComp);
            }

            base.Initialize(resolver);

            this.IsPaused = true;

            this.playerVm.Dies += async (s, e) =>
            {
                await this.WaitForUpdate();
                foreach (var npc in this.WorldRoot.Children.OfType<Npc>())
                {
                    npc.PlayerDied();
                }

                this.musicManager.EndSongs();
                var shutdown = new AudioEffect("Audio/windows_shutdown")
                {
                    Volume = .75f
                };
                this.UiRoot.Components.Add(shutdown);
                shutdown.Stopped += (_s, _e) => this.UiRoot.Components.Remove(shutdown);
                shutdown.Play();
                await this.player.PlayDeathAnimation();
                this.UiRoot.Insert(0, new DeathScreen());
            };

            await Task.Delay(1500);
            await this.WaitForUpdate();

            this.IsPaused = false;
        }

        private void InitUi()
        {
            var fontDef = new FontDefinition("DefaultFont24", 24);
            this.lblScore = new Label(fontDef)
            {
                FontSize = 24,
                Text = $"Score: {this.score}",
                Alignment = TextAlignment.Center,
                LocalPosition = new Vector2(Resolution.VirtualScreen.X / 2, 50),
                DropShadowOffset = new Vector2(2, 1),
                DropShadowTint = Color.White,
                DropShadowOpacity = .6f,
                HasDropShadow = true
            };
            this.UiRoot.Add(this.lblScore);

            var bar = new HealthBar(this.playerVm);
            this.UiRoot.Add(bar);
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

            this.AddFootprints();

            this.AddNpcs(camera);

            this.AddShoes(camera);

            this.WorldRoot.Add(this.player);

            this.physics = new PhysicsEngine();
            this.physics.RemovedFootprint += (s, e) => this.score += 10;
            this.WorldRoot.Components.Add(this.physics);
        }

        private void AddShoes(Camera camera)
        {
            var sneakers = new ShoePowerup
            {
                LocalPosition = Resolution.TransformPointToWorld(new Vector2(123f, 612f) * MapSize, camera),
                Powerup = ShoeType.Sneakers
            };
            this.WorldRoot.Add(sneakers);

            var rollerblades = new ShoePowerup
            {
                LocalPosition = Resolution.TransformPointToWorld(new Vector2(468f, 460f) * MapSize, camera),
                Powerup = ShoeType.Rollerblades
            };
            this.WorldRoot.Add(rollerblades);

            var flipflops = new ShoePowerup
            {
                LocalPosition = Resolution.TransformPointToWorld(new Vector2(890f, 597f) * MapSize, camera),
                Powerup = ShoeType.FlipFlops
            };
            this.WorldRoot.Add(flipflops);
        }

        private void AddNpcs(Camera camera)
        {
            var positions = new[]
            {
                new Vector2(372f, 257f),
                new Vector2(72f, 464f),
                new Vector2(141f, 750f),
                new Vector2(175f, 712f),
                new Vector2(414f, 523f),
                new Vector2(708f, 551f),
                new Vector2(906f, 363f),
                new Vector2(674f, 119f),
                new Vector2(1102f, 171f),
                new Vector2(1164f, 427f),
                new Vector2(1249f, 598f),
                new Vector2(1164f, 550f)
            };

            positions = positions.Select(v => Resolution.TransformPointToWorld(v * MapSize, camera)).ToArray();

            foreach (var pos in positions)
            {
                var npc = new Npc(this.player, this.playerVm)
                {
                    LocalPosition = pos
                };
                this.WorldRoot.Add(npc);
            }
        }

        private const float MapSize = 5f;

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

            path = path.Select(v => v * MapSize).ToArray();

            var gamePath = new GamePath(path);

            this.WorldRoot.Add(gamePath);
            var diff = this.player.GlobalPosition - gamePath.GlobalPosition;
            gamePath.LocalPosition += diff;
        }

        public override void Update(GameTime gameTime)
        {
            this.CheckPowerups();

            this.lblScore.Text = $"Score: {this.GetScore()}";

            base.Update(gameTime);
        }

        private float GetScore() => this.score - (int)(100f - this.playerVm.Health);

        private void CheckPowerups()
        {
            var powerups = this.WorldRoot.Children.OfType<ShoePowerup>().ToArray();
            foreach (var powerup in powerups)
            {
                var pos = powerup.GlobalPosition.ToPoint();
                if (this.player.GlobalRegion.Contains(pos.X, pos.Y))
                {
                    var audio = new AudioEffect("Audio/newShoes");
                    this.player.Components.Add(audio);
                    audio.Play();
                    audio.Stopped += (s, e) => this.player.Components.Remove(audio);
                    this.player.EquipShoe(powerup.Powerup);
                    powerup.Dismiss();
                    break;
                }
            }
        }
    }
}
