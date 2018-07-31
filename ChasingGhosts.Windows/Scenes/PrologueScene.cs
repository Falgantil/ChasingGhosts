using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

using Sharp2D.Engine.Common;
using Sharp2D.Engine.Common.Components.Animations;
using Sharp2D.Engine.Common.Scene;
using Sharp2D.Engine.Common.UI.Controls;
using Sharp2D.Engine.Common.UI.Enums;
using Sharp2D.Engine.Infrastructure;
using Sharp2D.Engine.Utility;

namespace ChasingGhosts.Windows.Scenes
{
    public class PrologueScene : Scene
    {
        private readonly IResolver resolver;

        private CancellationTokenSource tokenSource;

        public PrologueScene(IResolver resolver)
            : base(resolver)
        {
            this.resolver = resolver;
            Sharp2DApplication.GameManager.BackgroundColor = Color.Black;
        }

        public override async void Initialize(IResolver resolver)
        {
            base.Initialize(resolver);

            this.tokenSource = new CancellationTokenSource();

            try
            {
                await this.StartSequence(this.tokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                await this.LoadGame();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.GetHeldKeys().Length > 0)
            {
                this.tokenSource.Cancel();
            }
        }

        private async Task StartSequence(CancellationToken token)
        {
            const int SameLineMargin = 10;

            MediaPlayer.Volume = .8f;

            await this.Delay(1000, token);
            var lbl = this.CreateLabel("POST", Color.Gray);
            this.InsertLabelLine(lbl);
            await this.DelayDots(lbl, token);

            var lastLbl = lbl;
            var lastLblLength = GetLabelLength(lastLbl);
            lbl = this.CreateLabel("SUCCESS", Color.Green);
            lbl.LocalPosition = new Vector2(lastLbl.LocalPosition.X + lastLblLength + SameLineMargin, lastLbl.LocalPosition.Y);
            await this.Delay(500, token);

            lbl = this.CreateLabel("Initializing hardware drivers", Color.Gray);
            this.InsertLabelLine(lbl);
            await this.DelayDots(lbl, token);

            lastLbl = lbl;
            lastLblLength = GetLabelLength(lastLbl);
            lbl = this.CreateLabel("SUCCESS", Color.Green);
            lbl.LocalPosition = new Vector2(lastLbl.LocalPosition.X + lastLblLength + SameLineMargin, lastLbl.LocalPosition.Y);
            await this.Delay(500, token);

            var song = this.resolver.Resolve<ContentManager>().Load<Song>("windows_launch");
            MediaPlayer.Play(song);

            lbl = this.CreateLabel("Loading operation instructions", Color.Gray);
            this.InsertLabelLine(lbl);
            await this.DelayDots(lbl, token);

            song = this.resolver.Resolve<ContentManager>().Load<Song>("windows_error");
            lastLbl = lbl;
            lastLblLength = GetLabelLength(lastLbl);
            lbl = this.CreateLabel("SUCC", Color.Green);
            lbl.LocalPosition = new Vector2(lastLbl.LocalPosition.X + lastLblLength + SameLineMargin, lastLbl.LocalPosition.Y);
            await this.Delay(250, token);
            lbl.Text += ".-48tTQ#\"%rqawj912gGaf129'\"/";
            lbl.Tint = Color.IndianRed;
            MediaPlayer.Stop();
            MediaPlayer.Play(song);

            await this.Delay(1500, token);

            lbl = this.CreateLabel("Critical Error, restoring operating system \\\\ROBOT-OS\\backups\\shoe_obsession.bak", Color.Gray);
            this.InsertLabelLine(lbl);
            await this.Delay(1500, token);
            lbl = this.CreateLabel("...", Color.Gray);
            this.InsertLabelLine(lbl);
            await this.Delay(1500, token);
            lbl = this.CreateLabel("...", Color.Gray);
            this.InsertLabelLine(lbl);
            await this.Delay(1500, token);
            lbl = this.CreateLabel("...", Color.Gray);
            this.InsertLabelLine(lbl);
            song = this.resolver.Resolve<ContentManager>().Load<Song>("windows_bsod");
            await this.Delay(500, token);

            lbl = this.CreateLabel("System restored!", Color.Green);
            this.InsertLabelLine(lbl);

            MediaPlayer.Stop();
            MediaPlayer.Play(song);
            MediaPlayer.Volume = .25f;

            await this.Delay(3000, token);

            foreach (var label in this.UiRoot.Children.OfType<Label>().ToArray())
            {
                this.UiRoot.Children.Remove(label);
            }

            await this.Delay(1500, token);

            lbl = this.CreateLabel("\"What are these? Foot prints? From actual humans?!\"", Color.YellowGreen);
            this.InsertLabelLine(lbl);

            await this.Delay(3000, token);

            lbl = this.CreateLabel("\"I must find this specimen!\"", Color.YellowGreen);
            this.InsertLabelLine(lbl);

            await this.Delay(5000, token);

            ValueAnimator.PlayAnimation(
                this.UiRoot,
                val =>
                {
                    foreach (var label in this.UiRoot.Children.OfType<Label>())
                    {
                        label.Tint = Color.YellowGreen * (1f - val);
                    }
                },
                TimeSpan.FromSeconds(1f));

            await this.Delay(1500, token);

            await this.LoadGame();
        }

        private async Task LoadGame() => await Scene.Load(new GameScene(this.resolver));

        private async Task DelayDots(Label lbl, CancellationToken token)
        {
            await this.Delay(500, token);
            lbl.Text += ".";
            await this.Delay(500, token);
            lbl.Text += ".";
            await this.Delay(500, token);
            lbl.Text += ".";
            await this.Delay(500, token);
        }

        private static float GetLabelLength(Label label)
        {
            var text = label.FontDefinition.GetFont().MeasureString(label.Text);
            return text.X;
        }

        private async Task Delay(int ms, CancellationToken token)
        {
            await Task.Delay(ms, token);
            await this.WaitForUpdate();
        }

        private void InsertLabelLine(Label lbl)
        {
            lbl.LocalPosition = new Vector2(30, 30);
            foreach (var label in this.UiRoot.Children.OfType<Label>())
            {
                if (label == lbl)
                {
                    continue;
                }

                label.LocalPosition += new Vector2(0, 40);
            }
        }

        private Label CreateLabel(string text, Color textColor)
        {
            var fontDef = new FontDefinition("DefaultFont", 12);
            var lbl = new Label(fontDef)
            {
                FontSize = 12,
                Alignment = TextAlignment.Left,
                Tint = textColor,
                Text = text
            };
            this.UiRoot.Add(lbl);
            return lbl;
        }
    }
}
