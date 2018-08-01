using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChasingGhosts.Windows.Interfaces;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

using Sharp2D.Engine.Common.Components;
using Sharp2D.Engine.Common.Components.Animations;
using Sharp2D.Engine.Common.ObjectSystem;

namespace ChasingGhosts.Windows.Services
{
    public class MusicManager : Component, IMusicManager
    {
        private readonly ContentManager contentManager;

        private readonly List<SoundEffectInstance> songs = new List<SoundEffectInstance>();

        private int currentLevel;

        public MusicManager(ContentManager contentManager)
        {
            this.contentManager = contentManager;
        }

        public void LoadSongs(params string[] songAssets)
        {
            foreach (var inst in this.songs.ToArray())
            {
                inst.Stop();
                inst.Dispose();
                this.songs.Remove(inst);
            }
            foreach (var asset in songAssets)
            {
                var inst = this.contentManager.Load<SoundEffect>(asset).CreateInstance();
                inst.IsLooped = true;
                inst.Volume = 0;
                inst.Play();
                this.songs.Add(inst);
            }

            this.songs.First().Volume = .8f;
        }

        public void Transition(int level)
        {
            if (this.currentLevel >= level)
            {
                return;
            }

            var current = this.songs[this.currentLevel];
            var next = this.songs[level];

            this.currentLevel = level;

            if (current == next)
            {
                current.Volume = 0.8f;
                return;
            }

            var time = TimeSpan.FromSeconds(1);
            ValueAnimator.PlayAnimation(this.Parent, val => current.Volume = (1f - val) * .8f, time);
            ValueAnimator.PlayAnimation(this.Parent, val => next.Volume = val * .8f, time);
        }

        public void EndSongs()
        {
            foreach (var pair in this.songs)
            {
                pair.Volume = 0f;
            }
        }
    }
}
