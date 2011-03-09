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
   /// renders the game over screen, where the user can enter his player name
   /// </summary>
     
    public class GameOverScreen
    {
        // reference to main object 
        private Main game;

        private Texture2D texture;
       // last keyboard state
        private KeyboardState lastState;
        // current keyboard state
        private KeyboardState keyboardState;
        // Font for displaying text message
        SpriteFont font;
        // the final score of the player
        private int score;
        // the text to render on screen (for player name)
        string text = "";
        // initial info text on game over screen
        private const string initialtext = "Please enter your name";

        // valid input keys (to enter player name)
        Keys[] keysToCheck = new Keys[] {Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
                                         Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
                                         Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
                                         Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
                                         Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
                                         Keys.Z, Keys.Back, Keys.Space };

        // boolean to check whether user has entered already some keys (to remove initial text)
        private bool bFirstKeyHit = false; 

        /// <summary>
        /// constructor to create a game over screen
        /// </summary>
        /// <param name="game">the main game object</param>
        /// <param name="score">the final score of the player</param>
        public GameOverScreen(Main game, int score)
        {
            this.game = game;
            this.score = score;
            texture = game.getContentManager().Load<Texture2D>("Sprites\\game-over");
            lastState = Keyboard.GetState();
            font = game.getContentManager().Load<SpriteFont>("Fonts\\HighscoreFont");
        }

        /// <summary>
        /// updates the game over screen
        /// </summary>
        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Enter) && lastState.IsKeyUp(Keys.Enter))
            {
                Highscore.setHighscore(new Highscore(text, score));
                game.ExitCurrentGame();
            }
            
            foreach (Keys key in keysToCheck)
                if (keyboardState.IsKeyDown(key) && lastState.IsKeyUp(key))
                {
                    AddKeyToText(key);
                    bFirstKeyHit = true;
                    break;
                }

            lastState = keyboardState;
        }

        /// <summary>
        /// draws all objects on screen within one sprite batch
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
                spriteBatch.Draw(texture, new Vector2(0f, 0f), Color.White);

            if (!bFirstKeyHit)
            {
                // show message for user
                spriteBatch.DrawString(font,
               initialtext,
               new Vector2(370, 405),
               Color.Black);
            }
            else
            {
               // user input
               spriteBatch.DrawString(font,
               text,
               new Vector2(370, 405),
               Color.Black);
            }
            
        }

        /// <summary>
        /// adds an user entered key to the input field of the player name
        /// </summary>
        /// <param name="key">the key which was entered by the user</param>
        private void AddKeyToText(Keys key) {
            string newChar = "";

            if (text.Length >= 20 && key != Keys.Back)
                return;

             switch (key)
             {
                 case Keys.A:
                     newChar += "a";
                     break;
                 case Keys.B:
                     newChar += "b";
                     break;
                 case Keys.C:
                     newChar += "c";
                     break;
                 case Keys.D:
                     newChar += "d";
                     break;
                 case Keys.E:
                     newChar += "e";
                     break;
                 case Keys.F:
                     newChar += "f";
                     break;
                 case Keys.G:
                     newChar += "g";
                     break;
                 case Keys.H:
                     newChar += "h";
                     break;
                 case Keys.I:
                     newChar += "i";
                     break;
                 case Keys.J:
                     newChar += "j";
                     break;
                 case Keys.K:
                     newChar += "k";
                     break;
                 case Keys.L:
                     newChar += "l";
                     break;
                 case Keys.M:
                     newChar += "m";
                     break;
                 case Keys.N:
                     newChar += "n";
                     break;
                 case Keys.O:
                     newChar += "o";
                     break;
                 case Keys.P:
                     newChar += "p";
                     break;
                 case Keys.Q:
                     newChar += "q";
                     break;
                 case Keys.R:
                     newChar += "r";
                     break;
                 case Keys.S:
                     newChar += "s";
                     break;
                 case Keys.T:
                     newChar += "t";
                     break;
                 case Keys.U:
                     newChar += "u";
                     break;
                 case Keys.V:
                     newChar += "v";
                     break;
                 case Keys.W:
                     newChar += "w";
                     break;
                 case Keys.X:
                     newChar += "x";
                     break;
                 case Keys.Y:
                     newChar += "y";
                     break;
                 case Keys.Z:
                     newChar += "z";
                     break;
                 case Keys.Space:
                     newChar += " ";
                     break;
                 case Keys.Back:
                     if (text.Length != 0)
                         text = text.Remove(text.Length - 1);
                     return;
             }

            if (keyboardState.IsKeyDown(Keys.RightShift) ||
                keyboardState.IsKeyDown(Keys.LeftShift))
            {
                newChar = newChar.ToUpper();
            }

            text += newChar;
        }
    }
}
