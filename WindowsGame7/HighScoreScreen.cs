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
        private Main game;
        private KeyboardState lastState;
        SpriteFont font;
        Rectangle viewportRect;

        public HighScoreScreen(Main game)
        {
            this.game = game;
            texture = game.getContentManager().Load<Texture2D>("Sprites\\start-screen");
            lastState = Keyboard.GetState();
            font = game.getContentManager().Load<SpriteFont>("Fonts\\HighscoreFont");
            //Create a Rectangle that represents the full
            //drawable area of the game screen.
            viewportRect = new Rectangle(0, 0,
                game.GraphicsDevice.Viewport.Width,
                game.GraphicsDevice.Viewport.Height);
        }

        public void Update()
        {
            if (game.highscores == null)
                game.highscores = Highscore.loadHighscores();

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Enter) && lastState.IsKeyUp(Keys.Enter))
            {
                game.highscores = null;

                game.ExitCurrentGame();
            }

            lastState = keyboardState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture,viewportRect,
               Color.White);

            spriteBatch.DrawString(font,
               "Name",
               new Vector2(200, 100),
               Color.Black);

            spriteBatch.DrawString(font,
               "Points",
               new Vector2(600, 100),
               Color.Black);

            spriteBatch.DrawString(font,
               "--------------------------------------------",
               new Vector2(200, 120),
               Color.Black);

            if (game.highscores != null)
                for (int i = 0; i < game.highscores.Count; i++)
                {
                    spriteBatch.DrawString(font,
                       game.highscores[i].name,
                       new Vector2(200, 112 + 30 * (i + 1)),
                       Color.Black);

                    spriteBatch.DrawString(font,
                       game.highscores[i].score.ToString("#0000000"),
                       new Vector2(600, 112 + 30 * (i + 1)),
                       Color.Black);
                }
            else
                spriteBatch.DrawString(font,
                   "Loading scores...",
                   new Vector2(350,350),
                   Color.Black);
        }
    }
}
