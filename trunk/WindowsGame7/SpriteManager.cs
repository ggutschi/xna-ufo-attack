using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNAUfoAttack
{
    /// <summary>
    /// Class for managing the sprites and animations
    /// </summary>
    public class SpriteManager
    {
        protected Texture2D Texture;                // texture to handle
        public Vector2 Position = Vector2.Zero;     // position of the sprite
        public Color Color = Color.White;           // color of sprite
        public Vector2 Origin;                      // sprite origin
        public float Rotation = 0f;                 // sprite rotation
        public float Scale = 1f;                    // sprite scale
        public SpriteEffects SpriteEffect;          // effect of sprite
        protected Rectangle[] Rectangles;           // rectangle around a sprite frame
        protected int FrameIndex = 0;               // current index of sprite frames

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Texture">Texture of sprite</param>
        /// <param name="frames">Number of frames</param>
        public SpriteManager(Texture2D Texture, int frames)
        {
            this.Texture = Texture;
            int width = Texture.Width / frames;
            Rectangles = new Rectangle[frames];
            for (int i = 0; i < frames; i++)
                Rectangles[i] = new Rectangle(
                    i * width, 0, width, Texture.Height);
        }

        /// <summary>
        /// Draw spriteBatch
        /// </summary>
        /// <param name="spriteBatch">spriteBatch to draw</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Rectangles[FrameIndex],
                Color, Rotation, Origin, Scale, SpriteEffect, 0f);
        }
    }
}
