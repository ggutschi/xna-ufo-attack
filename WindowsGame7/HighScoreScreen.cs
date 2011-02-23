using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame7
{
    public class HighScoreScreen
    {
        private Texture2D texture;
        private Game1 game;
        private KeyboardState lastState;
        SpriteFont font;
        Rectangle viewportRect;

        public HighScoreScreen(Game1 game)
        {
            this.game = game;
            texture = game.getContentManager().Load<Texture2D>("Sprites\\start-screen");
            lastState = Keyboard.GetState();
            font = game.getContentManager().Load<SpriteFont>("Fonts\\GameFont");
            //Create a Rectangle that represents the full
            //drawable area of the game screen.
            viewportRect = new Rectangle(0, 0,
                game.GraphicsDevice.Viewport.Width,
                game.GraphicsDevice.Viewport.Height);
        }

        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Enter) && lastState.IsKeyUp(Keys.Enter))
            {
                game.ExitCurrentGame();
            }

            lastState = keyboardState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture,viewportRect,
               Color.White);

            spriteBatch.DrawString(font,
               "ufokiller " + "       959 points",
               new Vector2(100,100),
               Color.Yellow);
        }
    }
}
