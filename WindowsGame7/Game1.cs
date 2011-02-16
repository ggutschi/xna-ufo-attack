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
        GraphicsDeviceManager graphics;
        ContentManager content;

        Texture2D backgroundTexture;
        Rectangle viewportRect;
        SpriteBatch spriteBatch;

        GameObject cannon;
        const int maxCannonBalls = 3;
        GameObject[] cannonBalls;

        GamePadState previousGamePadState = GamePad.GetState(PlayerIndex.One);
        KeyboardState previousKeyboardState = Keyboard.GetState();

        const int maxEnemies = 3;
        const float maxEnemyHeight = 0.1f;
        const float minEnemyHeight = 0.5f;
        const float maxEnemyVelocity = 5.0f;
        const float minEnemyVelocity = 1.0f;
        Random random = new Random();
        GameObject[] enemies;

        int score;
        SpriteFont font;
        Vector2 scoreDrawPoint = new Vector2(0.1f, 0.1f);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            //ContentManager constructed with optional
            //second argument, looks in "Content" folder
            //on all calls to .Load.
            content = new ContentManager(Services, "Content");
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

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
            if (loadAllContent)
            {

                backgroundTexture =
                    content.Load<Texture2D>("Sprites\\background-space");

                cannon = new GameObject(content.Load<Texture2D>(
                    "Sprites\\cannon_01"));

                //Position is near the bottom-left of the screen.
                cannon.position = new Vector2(
                    graphics.GraphicsDevice.Viewport.Width/2, graphics.GraphicsDevice.Viewport.Height - 30);

                cannonBalls = new GameObject[maxCannonBalls];

                for (int i = 0; i < maxCannonBalls; i++)
                {
                    cannonBalls[i] = new GameObject(
                        content.Load<Texture2D>(
                        "Sprites\\cannonball"));
                }

                enemies = new GameObject[maxEnemies];
                for (int i = 0; i < maxEnemies; i++)
                {
                    enemies[i] = new GameObject(
                        content.Load<Texture2D>("Sprites\\enemy_01"));
                }

                font = content.Load<SpriteFont>("Fonts\\GameFont");
            }

            //Create a Rectangle that represents the full
            //drawable area of the game screen.
            viewportRect = new Rectangle(0, 0,
                graphics.GraphicsDevice.Viewport.Width,
                graphics.GraphicsDevice.Viewport.Height);
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
                // TODO: Unload any ResourceManagementMode.Automatic content
                content.Unload();
            }

            // TODO: Unload any ResourceManagementMode.Manual content
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            cannon.rotation += gamePadState.ThumbSticks.Left.X * 0.1f;

            //Restrict keyboard code to Windows-only using
            //#if !XBOX.
#if !XBOX
            KeyboardState keyboardState = Keyboard.GetState();

            if(keyboardState.IsKeyDown(Keys.Left))
            {
                cannon.rotation -= 0.1f;
            }
            if(keyboardState.IsKeyDown(Keys.Right))
            {
                cannon.rotation += 0.1f;
            }
#endif
            //Restrict cannon rotation toa ninety-degree angle: clamp(value, min, max)
            cannon.rotation = MathHelper.Clamp(
                cannon.rotation, -1f, 1f);

            //Only fire cannon ball if player has pressed button
            //this update loop - do not fire cannon ball if
            //button is merely held down.
            if (gamePadState.Buttons.A == ButtonState.Pressed &&
                previousGamePadState.Buttons.A == ButtonState.Released)
            {
                FireCannonBall();
            }

#if !XBOX
            if (keyboardState.IsKeyDown(Keys.Space) &&
                previousKeyboardState.IsKeyUp(Keys.Space))
            {
                FireCannonBall();
            }
#endif


            UpdateCannonBalls();
            UpdateEnemies();

            //Reset previous input states to current states
            //for next Update call.
            previousGamePadState = gamePadState;
#if !XBOX
            previousKeyboardState = keyboardState;
#endif

            base.Update(gameTime);
        }

        /// <summary>
        /// Updates enemy positions, kills them if
        /// they leave the screen, and resurrects
        /// dead enemies, giving them random position
        /// and velocity.
        /// </summary>
        public void UpdateEnemies()
        {
            foreach (GameObject enemy in enemies)
            {
                if (enemy.alive)
                {
                    //Check if the enemy is contained within
                    //the screen-sized rectangle. If not,
                    //kill the enemy.
                    enemy.position += enemy.velocity;
                    if (!viewportRect.Contains(new Point(
                        (int)enemy.position.X,
                        (int)enemy.position.Y)))
                    {
                        enemy.alive = false;
                    }
                }
                else
                {
                    enemy.alive = true;

                    //Establish random position and
                    //velocity for enemy using linear
                    //interpolation lerp: value1 + (value2 - value1) * amount
                    enemy.position = new Vector2(
                        viewportRect.Right,
                        MathHelper.Lerp(
                        (float)viewportRect.Height * minEnemyHeight,
                        (float)viewportRect.Height * maxEnemyHeight,
                        (float)random.NextDouble()));

                    //Velocity negated so ship moves
                    //right-to-left.
                    enemy.velocity = new Vector2(
                        MathHelper.Lerp(
                        -minEnemyVelocity,
                        -maxEnemyVelocity,
                        (float)random.NextDouble()), 0);
                }
            }
        }

        /// <summary>
        /// Places a cannon ball object into the game world
        /// by setting position and velocity
        /// </summary>
        public void FireCannonBall()
        {
            foreach (GameObject ball in cannonBalls)
            {
                if (!ball.alive)
                {
                    ball.alive = true;
                    ball.position = cannon.position;

                    //Determine velocity using sine and
                    //cosine of the cannon's rotation angle,
                    //then scale by 5 to speed up the ball.
                    //Console.WriteLine(cannon.rotation);
                    float cannon_rotation = cannon.rotation - 1.6f;
                    ball.velocity = new Vector2(
                        (float)Math.Cos(cannon_rotation),
                        (float)Math.Sin(cannon_rotation))
                    * 5.0f;

                   
                    return;
                }
            }
        }

        /// <summary>
        /// Moves cannon balls, handles cannon balls
        /// that move off screen edges, and tests intersection
        /// between cannon balls and enemies
        /// </summary>
        public void UpdateCannonBalls()
        {
            foreach (GameObject ball in cannonBalls)
            {
                if (ball.alive)
                {
                    //Check if the ball is contained within
                    //the screen-sized rectangle. If not,
                    //kill the ball.
                    ball.position += ball.velocity;
                    if (!viewportRect.Contains(new Point(
                        (int)ball.position.X,
                        (int)ball.position.Y)))
                    {
                        ball.alive = false;
                        continue;
                    }

                    //Construct a collision rectangle around
                    //the cannonball using current position.
                    Rectangle cannonBallRect = new Rectangle(
                        (int)ball.position.X,
                        (int)ball.position.Y,
                        ball.sprite.Width,
                        ball.sprite.Height);

                    //Loop through each enemy, construct a
                    //collision rectangle around enemy, and
                    //check for an intersection.
                    foreach (GameObject enemy in enemies)
                    {
                        Rectangle enemyRect = new Rectangle(
                            (int)enemy.position.X,
                            (int)enemy.position.Y,
                            enemy.sprite.Width,
                            enemy.sprite.Height);

                        if (cannonBallRect.Intersects(enemyRect))
                        {
                            ball.alive = false;
                            enemy.alive = false;

                            //Hitting an enemy with a cannon ball
                            //counts as a score point.
                            score += 1;

                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            //Draw the backgroundTexture sized to the width
            //and height of the screen.
            spriteBatch.Draw(backgroundTexture, viewportRect,
                Color.White);

            foreach (GameObject ball in cannonBalls)
            {
                if (ball.alive)
                {
                    spriteBatch.Draw(ball.sprite,
                        ball.position, Color.White);
                }
            }

            //Draw the cannon GameObject using rotation.
            spriteBatch.Draw(cannon.sprite,
                cannon.position, null, Color.White,
                cannon.rotation,
                cannon.center, 1.0f,
                SpriteEffects.None, 0);

            foreach (GameObject enemy in enemies)
            {
                if (enemy.alive)
                {
                    spriteBatch.Draw(enemy.sprite,
                        enemy.position, Color.White);
                }
            }

            //Construct a score string and draw to
            //near top-left corner of the screen.
            spriteBatch.DrawString(font,
                "Score: " + score.ToString(),
                new Vector2(scoreDrawPoint.X * viewportRect.Width,
                scoreDrawPoint.Y * viewportRect.Height),
                Color.Yellow);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
