using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame7
{
    public class StartScreen
    {
        private Texture2D texture;
        private Game1 game;
        private bool returnFromScreen;
        
        Menu startMenu;
        SpriteFont font;

        KeyboardState oldState;

        Song music;


        public StartScreen(Game1 game, bool returnFromScreen)
        {
            this.game = game;

            // load music only once
            if (!returnFromScreen)
            {
                music = game.getContentManager().Load<Song>("Audio\\Mp3s\\music");
                MediaPlayer.Play(music);
                MediaPlayer.IsRepeating = true;                
            }

            // set game play sound to a low level
            MediaPlayer.Volume = (float)0.3;
            
           
            texture = game.getContentManager().Load<Texture2D>("Sprites\\start-screen");
            this.returnFromScreen = returnFromScreen;
            //lastState = Keyboard.GetState();

            font = game.getContentManager().Load<SpriteFont>("Fonts\\GameFont");

            startMenu = new Menu(Color.Gray, Color.Black, game.getContentManager().Load<SpriteFont>("Fonts\\GameFont"), true);
            //Menüpunkt hinzufügen
            startMenu.AddMenuItem("START GAME", MenuChoice.CONTINUE, new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 50, game.GraphicsDevice.Viewport.Height / 2 - 150));
            startMenu.AddMenuItem("HIGHSCORE", MenuChoice.HIGHSCORE, new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 50, game.GraphicsDevice.Viewport.Height / 2 - 100));
            startMenu.AddMenuItem("EXIT", MenuChoice.EXIT, new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 50, game.GraphicsDevice.Viewport.Height / 2 - 50));
        }

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
                    //if (!returnFromScreen)
                        game.StartGame();

                    
                }

                else if (startMenu.GetSelectedItem().Equals(MenuChoice.HIGHSCORE))
                {
                    //if (!returnFromScreen)
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

        public void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
                spriteBatch.Draw(texture, new Vector2(0f, 0f), Color.White);

            // draw menu
            startMenu.Draw(spriteBatch, false);
        }
    }
}
