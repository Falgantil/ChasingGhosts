using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChasingGhosts.Windows.World;

namespace ChasingGhosts.Windows.ViewModels
{
    public class PlayerViewModel
    {
        public float Health { get; private set; } = 100f;

        public event EventHandler Dies;

        public bool IsAlive => this.Health > 0f;

        public ShoeType ShoeType { get; set; } = ShoeType.Sneakers;

        public bool IsInvulnerable { get; set; } = false;

        public void DamagePlayer(float damage)
        {
            if (this.IsInvulnerable)
            {
                return;
            }

            var oldHealth = this.Health;
            this.Health -= damage;
            if (this.Health <= 0 && oldHealth > 0)
            {
                this.Dies?.Invoke(this, EventArgs.Empty);
            }
        }

        public void HealPlayer(float healing)
        {
            this.Health += healing;
            if (this.Health >= 100)
            {
                this.Health = 100;
            }
        }
    }
}
