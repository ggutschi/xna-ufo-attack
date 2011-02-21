using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame7
{
    class SpriteAnimation : SpriteManager
    {
        private float timeElapsed;
        public bool IsLooping = false;
        public bool animationOver = false;

        // default to 20 frames per second
        private float timeToUpdate = 0.05f;
        public int FramesPerSecond
        {
            set { timeToUpdate = (1f / value); }
        }


        public SpriteAnimation(Texture2D Texture, int frames)
            : base(Texture, frames)
        {
        }

        public void Update(GameTime gameTime)
        {
            timeElapsed += (float)
                gameTime.ElapsedGameTime.TotalSeconds;

            if (timeElapsed > timeToUpdate)
            {
                timeElapsed -= timeToUpdate;

                if (FrameIndex < Rectangles.Length - 1)
                    FrameIndex++;
                else if (IsLooping)
                    FrameIndex = 0;
                else
                    animationOver = true;
            }
        }

        public bool isAnimationOver()
        {
            return animationOver;
        }
    }
}

