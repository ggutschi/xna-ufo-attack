using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XNAUfoAttack
{
    /// <summary>
    /// renders the highscores (player name and their score)
    /// </summary>
    public class HighScoreScreen
    {
        private Texture2D texture;
        private Main game;
        private KeyboardState lastState;
        SpriteFont font;
        Rectangle viewportRect;

        /// <summary>
        /// constructs the high score screen (loads the neccessary font and texture objects)
        /// </summary>
        /// <param name="game"></param>
        public HighScoreScreen(Main game)
        {
            this.game = game;
            texture = game.getContentManager().Load<Texture2D>("Sprites\\start-screen");
            lastState = Keyboard.GetState();
            font = game.getContentManager().Load<SpriteFont>("Fonts\\HighscoreFont");
            // create a Rectangle that represents the full
            // drawable area of the game screen.
            viewportRect = new Rectangle(0, 0,
                game.GraphicsDevice.Viewport.Width,
                game.GraphicsDevice.Viewport.Height);
        }

        /// <summary>
        /// updates the highscore screen
        /// and checks user input
        /// </summary>
        public void Update()
        {
            if (game.highscores == null)
                game.highscores = Highscore.loadHighscores();

            KeyboardState keyboardState = Keyboard.GetState();

            // pressing ENTER on this screen means to exit and go back to the start screen
            if (keyboardState.IsKeyDown(Keys.Enter) && lastState.IsKeyUp(Keys.Enter))
            {
                game.highscores = null;
                game.ExitCurrentGame();
            }

            lastState = keyboardState;
        }

        /// <summary>
        /// draws the highscore screen
        /// iterates over all highscores and prints them out (one line for each highscore)
        /// </summary>
        /// <param name="spriteBatch"></param>
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
