using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XNAUfoAttack
{
    /// <summary>
    /// renders the start screen of the game including a menu to start the game and to see the highscore
    /// </summary>
    public class StartScreen
    {
        private Texture2D texture;
        private Main game;
        private bool returnFromScreen;
        
        Menu startMenu;                 // the start menu
        KeyboardState oldState;         // last keyboard state
        Song music;                     // a nice game music


        /// <summary>
        /// constructs the start screen of the game
        /// </summary>
        /// <param name="game">the main game object</param>
        /// <param name="returnFromScreen">indicates whether the startscreen is loaded for the very first time or not</param>
        public StartScreen(Main game, bool returnFromScreen)
        {
            this.game = game;

            // load music only once (if we are not comming back from game play screen)
            if (!returnFromScreen)
            {
                music = game.getContentManager().Load<Song>("Audio\\Mp3s\\music");
                MediaPlayer.Play(music);
                MediaPlayer.IsRepeating = true;                
            }

            // set game play sound to a low level
            MediaPlayer.Volume = (float)0.3;
            
            // load start screen background
            texture = game.getContentManager().Load<Texture2D>("Sprites\\start-screen");
            this.returnFromScreen = returnFromScreen;
            // create the start menu
            startMenu = new Menu(Color.Gray, Color.Black, game.getContentManager().Load<SpriteFont>("Fonts\\GameFont"), true);
            // add items to the start menu
            startMenu.AddMenuItem("START GAME", MenuChoice.CONTINUE, new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 50, game.GraphicsDevice.Viewport.Height / 2 - 150));
            startMenu.AddMenuItem("HIGHSCORE", MenuChoice.HIGHSCORE, new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 50, game.GraphicsDevice.Viewport.Height / 2 - 100));
            startMenu.AddMenuItem("EXIT", MenuChoice.EXIT, new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 50, game.GraphicsDevice.Viewport.Height / 2 - 20));
        }
        
        /// <summary>
        /// updates the start screen
        /// and checks the user input (menu item selection)
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {

            KeyboardState keyboardState = Keyboard.GetState();

            if (returnFromScreen)
            {
                oldState = keyboardState;
                returnFromScreen = false;
            }

            if (keyboardState.IsKeyDown(Keys.Down) && !oldState.IsKeyDown(Keys.Down))
            {
                startMenu.SelectNext();
            }

            if (keyboardState.IsKeyDown(Keys.Up) && !oldState.IsKeyDown(Keys.Up))
            {
                startMenu.SelectPrev();

            }

            if (keyboardState.IsKeyDown(Keys.Enter) && !oldState.IsKeyDown(Keys.Enter))
            {
                
                if (startMenu.GetSelectedItem().Equals(MenuChoice.CONTINUE))
                {
                    Console.WriteLine(oldState.IsKeyDown(Keys.Enter));
                    oldState = keyboardState;
                    game.StartGame();
                }

                else if (startMenu.GetSelectedItem().Equals(MenuChoice.HIGHSCORE))
                {
                    game.ShowHighScore();                    
                }

                else if (startMenu.GetSelectedItem().Equals(MenuChoice.EXIT))
                {
                    if (!returnFromScreen)
                        game.Exit();
                }
            }

            // save current keyboard state as oldstate (for slowing down key presses)
            oldState = keyboardState;
            
            // update start menu  
            startMenu.Update(gameTime);
        }

        /// <summary>
        /// draws the start screen (texture and menu)
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
                spriteBatch.Draw(texture, new Vector2(0f, 0f), Color.White);

            // draw menu
            startMenu.Draw(spriteBatch, false);
        }
    }
}
