using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNAUfoAttack
{
    /// <summary>
    /// Represents an Animation
    /// </summary>
    class SpriteAnimation : SpriteManager
    {
        private float timeElapsed;          // elapsed time since start of animation
        public bool IsLooping = false;      // animation loops
        public bool animationOver = false;  // animation is over

        // default to 20 frames per second
        private float timeToUpdate = 0.05f;

        /// <summary>
        /// Setter for timeToUpdate
        /// </summary>
        public int FramesPerSecond
        {
            set { timeToUpdate = (1f / value); }
        }

        /// <summary>
        /// Set the SpriteAnimation (calls Constructor of SpriteManager)
        /// </summary>
        /// <param name="Texture">texture of sprite</param>
        /// <param name="frames">frames of animation</param>
        public SpriteAnimation(Texture2D Texture, int frames)
            : base(Texture, frames)
        {
        }

        /// <summary>
        /// Updates the animation
        /// </summary>
        /// <param name="gameTime">Current game time</param>
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

        /// <summary>
        /// Determines if the animation is over
        /// </summary>
        /// <returns>true, if over, false, otherwhise</returns>
        public bool isAnimationOver()
        {
            return animationOver;
        }
    }
}

