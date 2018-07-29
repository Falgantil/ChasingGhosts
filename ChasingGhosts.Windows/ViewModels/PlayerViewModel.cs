using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChasingGhosts.Windows.ViewModels
{
    public class PlayerViewModel
    {
        public float Health { get; private set; } = 100f;

        public event EventHandler Dies;

        public bool IsAlive => this.Health > 0f;

        public void DamagePlayer(float damage)
        {
            var oldHealth = this.Health;
            this.Health -= damage;
            if (this.Health <= 0 && oldHealth > 0)
            {
                this.Dies?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
