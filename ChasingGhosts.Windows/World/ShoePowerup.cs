using Microsoft.Xna.Framework;

using Sharp2D.Engine.Common.Components.Sprites;
using Sharp2D.Engine.Common.ObjectSystem;
using Sharp2D.Engine.Infrastructure;
using Sharp2D.Engine.Utility;

namespace ChasingGhosts.Windows.World
{
    public class ShoePowerup : GameObject
    {
        public ShoeType Powerup { get; set; }

        public override void Initialize(IResolver resolver)
        {
            const string IconAsset = "shoes_icon";
            var sheet = new SpriteSheet<int>
            {
                Regions = new SpriteRegions<int>
                {
                    { 0, new SpriteFrame(IconAsset, new Rectangle(0, 0, 16, 16)) },
                    { 1, new SpriteFrame(IconAsset, new Rectangle(16, 0, 16, 16)) },
                    { 2, new SpriteFrame(IconAsset, new Rectangle(32, 0, 16, 16)) },
                },
                Scale = new Vector2(3)
            };
            sheet.CenterObject();

            switch (this.Powerup)
            {
                case ShoeType.Sneakers:
                    sheet.RegionKey = 0;
                    break;
                case ShoeType.Rollerblades:
                    sheet.RegionKey = 1;
                    break;
                case ShoeType.FlipFlops:
                    sheet.RegionKey = 2;
                    break;
            }

            this.Components.Add(sheet);

            base.Initialize(resolver);
        }

        public void Dismiss()
        {
            this.Parent.Remove(this);
        }
    }
}