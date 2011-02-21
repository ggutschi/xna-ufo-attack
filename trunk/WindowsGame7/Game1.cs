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
        GraphicsDeviceManager graphics;
        ContentManager content;

        Texture2D backgroundTexture;
        Rectangle viewportRect;
        SpriteBatch spriteBatch;

        GameObject cannon;
        const int maxCannonBalls = 3;
        GameObject[] cannonBalls;

        const int maxEnemyCannonBalls = 3;
        GameObject[] enemyCannonBalls;

        GamePadState previousGamePadState = GamePad.GetState(PlayerIndex.One);
        KeyboardState previousKeyboardState = Keyboard.GetState();

        const int maxEnemies = 3;
        const float maxEnemyHeight = 0.1f;
        const float minEnemyHeight = 0.5f;
        const float maxEnemyVelocity = 5.0f;
        const float minEnemyVelocity = 1.0f;
        Random random = new Random();
        GameObject[] enemies;
        
        // lists for animating damaged enemies
        List<GameObject> damagedEnemies = new List<GameObject>();
        List<SpriteAnimation> damagedEnemiesSprites =  new List<SpriteAnimation>();

        GameObject lastEnemyShooted = null;     // last enemy which shooted a cannon ball

        int score;

        int lifes = 5;
        Texture2D lifeGraphics;

        SpriteFont font;
        Vector2 scoreDrawPoint = new Vector2(0.05f, 0.05f);
        Vector2 lifesDrawPoint = new Vector2(0.80f, 0.05f);

        Song music;

        SoundEffect enemyShootSound;
        SoundEffect cannonShootSound;
        SoundEffect explodeSound;

        Menu pauseMenu;
        Boolean paused = false; // is game paused or not
        KeyboardState oldState;

        

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

            music = content.Load<Song>("Audio\\Mp3s\\music");

            MediaPlayer.Play(music);
            MediaPlayer.IsRepeating = true;

            enemyShootSound = content.Load<SoundEffect>("Audio\\Waves\\enemyshoot");
            cannonShootSound = content.Load<SoundEffect>("Audio\\Waves\\cannonshoot");
            explodeSound = content.Load<SoundEffect>("Audio\\Waves\\explode");

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
                lifeGraphics =
                    content.Load<Texture2D>("Sprites\\cannon_01_small");

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

               
                enemyCannonBalls = new GameObject[maxEnemyCannonBalls];
                for (int i = 0; i < maxEnemyCannonBalls; i++)
                {
                    enemyCannonBalls[i] = new GameObject(
                        content.Load<Texture2D>("Sprites\\cannonball"));
                }

                font = content.Load<SpriteFont>("Fonts\\GameFont");


                pauseMenu = new Menu(Color.White, Color.LightBlue, content.Load<SpriteFont>("Fonts\\GameFont"), false);
                //Menüpunkt hinzufügen
                pauseMenu.AddMenuItem("Fortsetzen", MenuChoice.CONTINUE, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - 50, graphics.GraphicsDevice.Viewport.Height / 2 - 100));
                pauseMenu.AddMenuItem("Beenden", MenuChoice.EXIT, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - 50, graphics.GraphicsDevice.Viewport.Height / 2 - 50));

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
            if (keyboardState.IsKeyDown(Keys.A) && cannon.position.X >= 0)
            {
                cannon.position.X -= 3f;
            }
            if (keyboardState.IsKeyDown(Keys.D) && cannon.position.X <= viewportRect.Width)
            {
                cannon.position.X += 3f;
            }
    
            if (keyboardState.IsKeyDown(Keys.Escape) && !pauseMenu.isVisible())
            {
                BeginPause();
            }

            if (keyboardState.IsKeyDown(Keys.Down) && pauseMenu.isVisible() && !oldState.IsKeyDown(Keys.Down))
            {                
                pauseMenu.SelectNext();                     
            }

            if (keyboardState.IsKeyDown(Keys.Up) && pauseMenu.isVisible() && !oldState.IsKeyDown(Keys.Up))
            {               
                pauseMenu.SelectPrev();
                
            }

            if (keyboardState.IsKeyDown(Keys.Enter) && pauseMenu.isVisible())
            {
                if (pauseMenu.GetSelectedItem().Equals(MenuChoice.CONTINUE))
                {
                    EndPause();
                }

                else if (pauseMenu.GetSelectedItem().Equals(MenuChoice.EXIT))
                {
                    this.Exit();
                }
            }

            // save current keyboard state as oldstate (for slowing down key presses)
            oldState = keyboardState;
            

      

#endif


            // update game only if it´s not paused
            if (!isGamePaused())
            {

                //Restrict cannon rotation to a 180-degree angle: clamp(value, min, max)
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
                if (keyboardState.IsKeyDown(Keys.Up) &&
                    previousKeyboardState.IsKeyUp(Keys.Up))
                {
                    FireCannonBall();
                }
#endif

                FireEnemyCannonBall();

                UpdateCannonBalls(gameTime);
                UpdateEnemyCannonBalls();
                UpdateEnemies();
                UpdateDamagedEnemies(gameTime);
                               

                //Reset previous input states to current states
                //for next Update call.
                previousGamePadState = gamePadState;
#if !XBOX
                previousKeyboardState = keyboardState;
#endif

            } // if !paused

            
            // update also menu  
            pauseMenu.Update(gameTime);

            base.Update(gameTime);
        }


        // enable pause of game
        public void BeginPause()
        {
            paused = true;
            pauseMenu.setVisibilty(true);
        }

        // disable pause of game
        public void EndPause()
        {
            paused = false;
            pauseMenu.setVisibilty(false);
        }

        // return state of game (paused or not)
        public bool isGamePaused()
        {
            return paused;
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

                    cannonShootSound.Play();
                   
                    return;
                }
            }
        }


        /// <summary>
        /// Places a cannon ball object into the game world
        /// by setting position and velocity
        /// </summary>
        public void FireEnemyCannonBall()
        {
            foreach (GameObject ball in enemyCannonBalls)
            {
                if (!ball.alive)
                {
                    ball.alive = true;

                    // Get random enemy
                    GameObject enemyToShoot;

                    do
                    {
                        enemyToShoot = enemies[random.Next(maxEnemyCannonBalls)];
                    } while (enemyToShoot == lastEnemyShooted);

                    lastEnemyShooted = enemyToShoot;

                    ball.position = enemyToShoot.position;

                    //Determine velocity using vertical and horizontal
                    //distance between enemy and cannon
                    //then scale by 3 to speed up the ball.
                    Vector2 normV = new Vector2(
                        cannon.position.X - ball.position.X,
                        cannon.position.Y - ball.position.Y);

                    normV.Normalize();

                    ball.velocity = normV * 2.0f;

                    enemyShootSound.Play(0.5f);

                    return;
                }
            }
        }

        public void UpdateDamagedEnemies(GameTime gameTime)
        {
            // remove all damaged enemies when the damage animation is already over
            SpriteAnimation[] newDamagedEnemies = new SpriteAnimation[damagedEnemiesSprites.Count];
            damagedEnemiesSprites.CopyTo(newDamagedEnemies, 0);
            foreach (SpriteAnimation spriteAnim in newDamagedEnemies)
            {
                if (spriteAnim.isAnimationOver())
                    damagedEnemiesSprites.Remove(spriteAnim);                
            }

            // draw damaged enemies
            foreach (SpriteAnimation spriteAnim in damagedEnemiesSprites)
            {
                spriteAnim.Update(gameTime);
            }           
        }


        /// <summary>
        /// Moves cannon balls, handles cannon balls
        /// that move off screen edges, and tests intersection
        /// between cannon balls and enemies
        /// </summary>
        public void UpdateCannonBalls(GameTime gameTime)
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
                            damagedEnemies.Add(enemy);

                             // load sprite animation for damaged enemies
                            SpriteAnimation enemyAnim = new SpriteAnimation(
                            content.Load<Texture2D>("Sprites\\enemy_01_damagedSprite"), 4);
                            enemyAnim.Position = new Vector2(enemy.position.X, enemy.position.Y);
                            enemyAnim.IsLooping = false;
                            enemyAnim.FramesPerSecond = 20;
                            damagedEnemiesSprites.Add(enemyAnim);               

                            explodeSound.Play();

                            break;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Moves cannon balls, handles cannon balls
        /// that move off screen edges, and tests intersection
        /// between cannon balls and cannon
        /// </summary>
        public void UpdateEnemyCannonBalls()
        {
            foreach (GameObject ball in enemyCannonBalls)
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

                    //Construct a collision rectangle around cannon, and
                    //check for an intersection.
                    Rectangle cannonRect = new Rectangle(
                        (int)cannon.position.X,
                        (int)cannon.position.Y,
                        cannon.sprite.Width,
                        cannon.sprite.Height);

                    if (cannonBallRect.Intersects(cannonRect))
                    {
                        ball.alive = false;

                        //Hitting an enemy with a cannon ball
                        //counts as a score point.
                        if (lifes > 0)
                            lifes--;

                        break;
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

            foreach (GameObject ball in enemyCannonBalls)
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

            //Draw life graphics
            for (int i = 0; i < lifes; i++)
                spriteBatch.Draw(lifeGraphics, new Vector2(lifesDrawPoint.X * viewportRect.Width + 30 * i, lifesDrawPoint.Y * viewportRect.Height), Color.White);

            if (lifes < 1)
                spriteBatch.DrawString(font, "!!! GAME OVER !!!", new Vector2(viewportRect.Width / 2 - 85, viewportRect.Height / 2 - 20), Color.Red);

            // draw menu
            pauseMenu.Draw(spriteBatch, false);

            // draw damaged enemies
            foreach (SpriteAnimation spriteAnim in damagedEnemiesSprites)
            {
                spriteAnim.Draw(spriteBatch);
            }
           
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
