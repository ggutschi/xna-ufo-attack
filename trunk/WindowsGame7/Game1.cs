#region File Description
//-----------------------------------------------------------------------------
// Game1.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace WindowsGame7
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private ContentManager content;

        private SpriteBatch spriteBatch;

        public List<Highscore> highscores;

        enum Screen
        {
            StartScreen,
            GamePlayScreen,
            HighScoreScreen,
            GameOverScreen
        }

        private StartScreen startScreen;
        private Screen currentScreen;
        private GamePlayScreen gamePlayScreen;
        private GameOverScreen gameOverScreen;
        private HighScoreScreen highScoreScreen;

      
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            // set default screen size to 1024x768
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();

            //ContentManager constructed with optional
            //second argument, looks in "Content" folder
            //on all calls to .Load.
            content = new ContentManager(Services, "Content");
            
        }

        public ContentManager getContentManager()
        {
            return content;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            startScreen = new StartScreen(this, false);
            currentScreen = Screen.StartScreen;
            
            base.LoadContent();
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        
        /// <summary>
        /// Load your graphics content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
  
        }

        /// <summary>
        /// Unload your graphics content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent)
            {
                content.Unload();
            }
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            switch (currentScreen)
            {
                case Screen.StartScreen:
                    if (startScreen != null)
                        startScreen.Update(gameTime);
                    break;
                case Screen.GamePlayScreen:
                    if (gamePlayScreen != null)
                        gamePlayScreen.Update(gameTime);
                    break;
                case Screen.GameOverScreen:
                    if (gameOverScreen != null)
                        gameOverScreen.Update();
                    break;
                case Screen.HighScoreScreen:
                    if (highScoreScreen != null)
                        highScoreScreen.Update();
                    break;
            }
            base.Update(gameTime);    
        }   



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            //spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Begin();

            switch (currentScreen)
            {
                case Screen.StartScreen:
                    if (startScreen != null)
                        startScreen.Draw(spriteBatch);
                    break;
                case Screen.GamePlayScreen:
                    if (gamePlayScreen != null)
                        gamePlayScreen.Draw(spriteBatch);
                    break;
                case Screen.GameOverScreen:
                    if (gameOverScreen != null)
                        gameOverScreen.Draw(spriteBatch);
                    break;
                case Screen.HighScoreScreen:
                    if (highScoreScreen != null)
                        highScoreScreen.Draw(spriteBatch);
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
            
        }

        public void StartGame()
        {
            gamePlayScreen = new GamePlayScreen(this);
            currentScreen = Screen.GamePlayScreen;

            startScreen = null;
        }

        public void showGameOver(int score)
        {
            gameOverScreen = new GameOverScreen(this, score);
            currentScreen = Screen.GameOverScreen;
            
            gamePlayScreen = null;
            
        }

        public void ShowHighScore()
        {
            currentScreen = Screen.HighScoreScreen;
            highScoreScreen = new HighScoreScreen(this);
            startScreen = null;
        }

        public void ExitCurrentGame()
        {
            startScreen = new StartScreen(this, true);
            currentScreen = Screen.StartScreen;
            gamePlayScreen = null;
            highScoreScreen = null;
        }
    }


}
