using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChasingGhosts.Windows.ViewModels
{
    public class PlayerViewModel
    {
        private float health = 20f;

        public float Health
        {
            get => this.health;
            set
            {
                var above0 = this.health > 0;
                this.health = value;
                if (this.health <= 0 && above0)
                {
                    this.Dies?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler Dies;

        public bool IsAlive => this.Health > 0f;
    }
}
