using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Sharp2D.Engine.Common.Components.Sprites;
using Sharp2D.Engine.Common.ObjectSystem;
using Sharp2D.Engine.Common.UI.Controls;
using Sharp2D.Engine.Common.UI.Enums;
using Sharp2D.Engine.Common.World;
using Sharp2D.Engine.Drawing;
using Sharp2D.Engine.Infrastructure;
using Sharp2D.Engine.Infrastructure.Loading;
using Sharp2D.Engine.Utility;

namespace ChasingGhosts.Windows.UI
{
    public class SpeechBubble : GameObject
    {
        private string text = "Speech bubble";

        private Label lbl;

        private float opacity = 1f;

        private NinePatchSprite patch;

        public override void Initialize(IResolver resolver)
        {
            var fontDef = new FontDefinition("DefaultFont", 12);
            this.lbl = new Label(fontDef)
            {
                FontSize = 12,
                Alignment = TextAlignment.Left,
                Tint = Color.White,
                Text = this.Text
            };
            this.Add(this.lbl);

            var inst = new NinePatchInstruction
            {
                TopLeft = new Rectangle(0, 0, 12, 12),
                TopCenter = new Rectangle(12, 0, 8, 12),
                TopRight = new Rectangle(20, 0, 12, 12),
                MiddleLeft = new Rectangle(0, 12, 12, 8),
                MiddleCenter = new Rectangle(12, 12, 16, 16),
                MiddleRight = new Rectangle(20, 12, 12, 8),
                BottomLeft = new Rectangle(0, 20, 12, 12),
                BottomCenter = new Rectangle(12, 20, 8, 12),
                BottomRight = new Rectangle(20, 20, 12, 12),
            };
            this.patch = new NinePatchSprite(new TextureAssetInstruction { Asset = "ninepatch" }, inst);
            this.lbl.Components.Add(this.patch);

            this.Opacity = this.Opacity;

            base.Initialize(resolver);
        }

        public string Text
        {
            get => this.text;
            set
            {
                this.text = value;
                if (this.lbl == null)
                {
                    return;
                }
                this.lbl.Text = value;
            }
        }

        public float Opacity
        {
            get => this.opacity;
            set
            {
                this.opacity = value;

                if (this.lbl == null)
                {
                    return;
                }
                this.lbl.Tint = Color.White * value;
                if (this.patch == null)
                {
                    return;
                }
                this.patch.Tint = Color.White * value;
            }
        }
    }

    public class NinePatchSprite : Sprite
    {
        private readonly SpriteFrame topLeft;
        private readonly SpriteFrame topCenter;
        private readonly SpriteFrame topRight;

        private readonly SpriteFrame middleLeft;
        private readonly SpriteFrame middleCenter;
        private readonly SpriteFrame middleRight;

        private readonly SpriteFrame bottomLeft;
        private readonly SpriteFrame bottomCenter;
        private readonly SpriteFrame bottomRight;

        private Vector2 area = new Vector2(100, 50);

        private Vector2 offset;

        public NinePatchSprite(LoadInstruction<Texture2D> instruction, NinePatchInstruction patch)
        {
            this.Tint = Color.White;

            this.topLeft = new SpriteFrame
            {
                Instruction = instruction,
                Region = patch.TopLeft
            };
            this.topCenter = new SpriteFrame
            {
                Instruction = instruction,
                Region = patch.TopCenter
            };
            this.topRight = new SpriteFrame
            {
                Instruction = instruction,
                Region = patch.TopRight
            };

            this.middleLeft = new SpriteFrame
            {
                Instruction = instruction,
                Region = patch.MiddleLeft
            };
            this.middleCenter = new SpriteFrame
            {
                Instruction = instruction,
                Region = patch.MiddleCenter
            };
            this.middleRight = new SpriteFrame
            {
                Instruction = instruction,
                Region = patch.MiddleRight
            };

            this.bottomLeft = new SpriteFrame
            {
                Instruction = instruction,
                Region = patch.BottomLeft
            };
            this.bottomCenter = new SpriteFrame
            {
                Instruction = instruction,
                Region = patch.BottomCenter
            };
            this.bottomRight = new SpriteFrame
            {
                Instruction = instruction,
                Region = patch.BottomRight
            };

            this.Children.Add(this.topLeft);
            this.Children.Add(this.topCenter);
            this.Children.Add(this.topRight);

            this.Children.Add(this.middleLeft);
            this.Children.Add(this.middleCenter);
            this.Children.Add(this.middleRight);

            this.Children.Add(this.bottomLeft);
            this.Children.Add(this.bottomCenter);
            this.Children.Add(this.bottomRight);
        }

        public override void Initialize(IResolver resolver)
        {
            if (this.Parent is Label lbl)
            {
                this.SetupAsLabel(lbl);
            }

            base.Initialize(resolver);
        }

        private void SetupAsLabel(Label lbl)
        {
            var font = lbl.FontDefinition.GetFont();
            var size = font.MeasureString(lbl.Text);
            this.area = size;
            this.offset = new Vector2(0, 24);
        }

        public override void Draw(SharpDrawBatch batch, GameTime time, Vector2 position, Color tint, float rotation, Vector2 scale, SpriteEffects effects, float depth)
        {
            var patchWidth = (int)this.area.X;
            var patchHeight = (int)this.area.Y;

            var tlReg = this.topLeft.Region;
            var tcReg = this.topCenter.Region;
            var trReg = this.topRight.Region;

            var mlReg = this.middleLeft.Region;
            var mcReg = this.middleCenter.Region;
            var mrReg = this.middleRight.Region;

            var blReg = this.bottomLeft.Region;
            var bcReg = this.bottomCenter.Region;
            var brReg = this.bottomRight.Region;

            var pos = ((position - this.offset) - new Vector2(this.topLeft.Region.Width, 0)).ToPoint();

            var tex = this.topLeft.Texture;
            batch.Draw(tex, new Rectangle(pos.X, pos.Y, tlReg.Width, tlReg.Height), tlReg, tint);

            tex = this.topCenter.Texture;
            batch.Draw(tex, new Rectangle(pos.X + tlReg.Width, pos.Y, patchWidth, tcReg.Height), tcReg, tint);

            tex = this.topRight.Texture;
            batch.Draw(tex, new Rectangle(pos.X + tlReg.Width + patchWidth, pos.Y, trReg.Width, trReg.Height), trReg, tint);



            tex = this.middleLeft.Texture;
            batch.Draw(tex, new Rectangle(pos.X, pos.Y + tlReg.Height, mlReg.Width, patchHeight), mlReg, tint);

            tex = this.middleCenter.Texture;
            var centerWidth = (tlReg.Width - mlReg.Width) + (trReg.Width - mrReg.Width);
            var centerHeight = (tlReg.Height - tcReg.Height) + (brReg.Height - bcReg.Height);
            batch.Draw(tex, new Rectangle(pos.X + mlReg.Width, pos.Y + tcReg.Height, patchWidth + centerWidth, patchHeight + centerHeight), mcReg, tint);

            tex = this.middleRight.Texture;
            batch.Draw(tex, new Rectangle(pos.X + tlReg.Width + patchWidth + (tlReg.Width - mrReg.Width), pos.Y + trReg.Height, mrReg.Width, patchHeight), mrReg, tint);

            tex = this.bottomLeft.Texture;
            batch.Draw(tex, new Rectangle(pos.X, pos.Y + trReg.Height + patchHeight, blReg.Width, blReg.Height), blReg, tint);

            tex = this.bottomCenter.Texture;
            batch.Draw(tex, new Rectangle(pos.X + blReg.Width, pos.Y + trReg.Height + patchHeight + (blReg.Height - bcReg.Height), patchWidth, bcReg.Height), bcReg, tint);

            tex = this.bottomRight.Texture;
            batch.Draw(tex, new Rectangle(pos.X + blReg.Width + patchWidth, pos.Y + trReg.Height + patchHeight, brReg.Width, brReg.Height), brReg, tint);




            // var destRect = new Rectangle((int)position.X, (int)position.Y, this.topLeft.Region.Width, this.topLeft.Region.Height);
            // batch.Draw(tex, destRect, this.topLeft.Region, tint, rotation, position, effects, depth);
            // batch.Draw(tex, position, this.topLeft.Region, tint);
        }
    }

    public class NinePatchInstruction
    {
        public Rectangle TopLeft { get; set; }
        public Rectangle TopCenter { get; set; }
        public Rectangle TopRight { get; set; }

        public Rectangle MiddleLeft { get; set; }
        public Rectangle MiddleCenter { get; set; }
        public Rectangle MiddleRight { get; set; }

        public Rectangle BottomLeft { get; set; }
        public Rectangle BottomCenter { get; set; }
        public Rectangle BottomRight { get; set; }
    }
}