using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChasingGhosts.Windows.Interfaces;
using ChasingGhosts.Windows.UI;
using ChasingGhosts.Windows.ViewModels;

using Microsoft.Xna.Framework;

using Sharp2D.Engine.Common;
using Sharp2D.Engine.Common.Components.Animations;
using Sharp2D.Engine.Common.Components.Audio;
using Sharp2D.Engine.Common.Components.Sprites;
using Sharp2D.Engine.Common.ObjectSystem;
using Sharp2D.Engine.Common.World;
using Sharp2D.Engine.Drawing;
using Sharp2D.Engine.Infrastructure;

namespace ChasingGhosts.Windows.World
{
    public class Npc : GameObject, IMovableCharacter
    {
        private readonly Player player;

        private readonly PlayerViewModel viewModel;

        private GameTimer attackTimer;

        private MovementSprite spriteWalk;

        private bool hasSeenPlayer;

        public Npc(Player player, PlayerViewModel viewModel)
        {
            this.player = player;
            this.viewModel = viewModel;
        }

        public Movement Direction { get; private set; }

        public float MaxMovement { get; private set; } = 200;

        public Vector2 Movement { get; private set; }

        public override void Initialize(IResolver resolver)
        {
            this.Width = 72;
            this.Height = 72;
            var visual = new Sprite(Color.GreenYellow * .25f, (int)this.Width, (int)this.Height);
            visual.CenterObject();
            this.Components.Add(visual);
            visual.IsVisible = false;

            this.spriteWalk = new MovementSprite("npc_walk", TimeSpan.FromSeconds(.5f));
            this.spriteWalk.Start();
            this.Components.Add(this.spriteWalk);

            this.hasSeenPlayer = false;

            base.Initialize(resolver);
        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            if (this.IsPaused)
            {
                this.Movement = Vector2.Zero;
                return;
            }

            this.UpdateSeenPlayer();

            if (!this.hasSeenPlayer)
            {
                this.Movement = Vector2.Zero;
                return;
            }

            this.HandlePlayerRange();

            this.HandleMovement();

            this.UpdateDirection();
        }

        private void UpdateSeenPlayer()
        {
            if (this.hasSeenPlayer)
            {
                return;
            }

            var diff = this.GlobalPosition - this.player.GlobalPosition;
            var halfScreen = Resolution.VirtualScreen / 2f;
            if (Math.Abs(diff.X) <= halfScreen.X && Math.Abs(diff.Y) <= halfScreen.Y)
            {
                this.hasSeenPlayer = true;
                this.AddTextBubble();
            }
        }

        private static readonly string[] Quotes =
        {
            "Traitor!",
            "Why do you chase humans?!",
            "You horrible robot-being",
            "Life is futil-... eh machinery is futile!",
            "Human-lover!"
        };

        private void AddTextBubble()
        {
            var rnd = new Random();

            var speech = new SpeechBubble
            {
                LocalPosition = new Vector2(30, -50),
                Text = Quotes[rnd.Next(0, Quotes.Length)]
            };
            this.Add(speech);
            var timer = new GameTimer(TimeSpan.FromSeconds(5));
            timer.Expired += (s, e) =>
            {
                this.Components.Remove(timer);
                ValueAnimator.PlayAnimation(speech, f => speech.Opacity = 1 - f, TimeSpan.FromSeconds(.5f));
            };
            this.Components.Add(timer);
        }

        private void UpdateDirection()
        {
            var direction = MovementHelper.GetMovement(this.Movement);
            if (direction != World.Movement.None)
            {
                this.spriteWalk.Direction = direction;
            }
        }

        private void HandlePlayerRange()
        {
            if (this.attackTimer != null)
            {
                return;
            }
            if (!this.IsCloseEnoughToHit())
            {
                return;
            }

            this.attackTimer = new GameTimer(TimeSpan.FromSeconds(.75f));
            this.attackTimer.Expired += (s, e) =>
            {
                this.Components.Remove(this.attackTimer);
                this.Attack();
                this.StartCooldown();
            };
            this.Components.Add(this.attackTimer);
        }

        private void StartCooldown()
        {
            this.MaxMovement = 50;
            var cooldown = new GameTimer(TimeSpan.FromSeconds(1));
            cooldown.Expired += (s, e) =>
            {
                this.Components.Remove(cooldown);
                this.attackTimer = null;
                this.MaxMovement = 200;
            };
            this.Components.Add(cooldown);
        }

        private void Attack()
        {
            if (!this.IsCloseEnoughToHit())
            {
                return;
            }

            if (this.viewModel.IsInvulnerable)
            {
                return;
            }

            this.viewModel.DamagePlayer(15f);

            var rnd = new Random();
            var hit = new AudioEffect(rnd.Next(2) == 0 ? "Audio/hit" : "Audio/hit2");
            this.Components.Add(hit);
            hit.Play();
            hit.Stopped += (s, e) => this.Components.Remove(hit);
        }

        private bool IsCloseEnoughToHit() => Vector2.Distance(this.GlobalPosition, this.player.GlobalPosition) <= 100;

        private void HandleMovement()
        {
            //return;
            var m = this.player.GlobalPosition - this.GlobalPosition;
            m.Normalize();
            this.Movement = m;
        }

        public void PlayerDied()
        {
            this.IsPaused = true;
            this.Movement = Vector2.Zero;
        }
    }
}
