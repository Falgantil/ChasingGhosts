using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChasingGhosts.Windows.ViewModels;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Sharp2D.Engine.Common;
using Sharp2D.Engine.Common.Components.Animations;
using Sharp2D.Engine.Common.Components.Animations.Predefined;
using Sharp2D.Engine.Common.ObjectSystem;
using Sharp2D.Engine.Drawing;
using Sharp2D.Engine.Infrastructure;

namespace ChasingGhosts.Windows.UI
{
    public class HealthBar : GameObject
    {
        private readonly PlayerViewModel viewModel;

        private Texture2D text;

        private float animHealth;

        public HealthBar(PlayerViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.animHealth = this.viewModel.Health;
        }

        public override void Initialize(IResolver resolver)
        {
            base.Initialize(resolver);

            this.text = new Texture2D(resolver.Resolve<GraphicsDevice>(), 1, 1);
            this.text.SetData(new[]
            {
                Color.White
            });
        }

        public float Health => this.viewModel.Health;

        public override void Update(GameTime time)
        {
            base.Update(time);

            const int TriggersPerSecond = 20;

            if (this.animHealth != this.Health)
            {
                var diff = TriggersPerSecond * (float)time.ElapsedGameTime.TotalSeconds;
                if (this.animHealth > this.Health && this.animHealth - diff <= this.Health)
                {
                    this.animHealth = this.Health;
                }
                else if (this.animHealth < this.Health && this.animHealth - diff >= this.Health)
                {
                    this.animHealth = this.Health;
                }
                else
                {
                    this.animHealth += this.animHealth > this.Health ? -diff : diff;
                }
            }
        }

        public override void Draw(SharpDrawBatch batch, GameTime time)
        {
            base.Draw(batch, time);

            var screen = Resolution.VirtualScreen;
            var perc = 1f - (this.animHealth / 100f);

            batch.Draw(this.text, new Rectangle(0, 0, (int)screen.X, (int)screen.Y), Color.DarkRed * perc * .7f);
        }
    }
}
