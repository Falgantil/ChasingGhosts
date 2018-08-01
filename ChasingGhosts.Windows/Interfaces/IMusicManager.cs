using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChasingGhosts.Windows.Interfaces
{
    public interface IMusicManager
    {
        void LoadSongs(params string[] songAssets);

        void Transition(int level);

        void EndSongs();
    }
}
