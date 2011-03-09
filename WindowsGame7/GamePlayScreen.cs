using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace WindowsGame7
{
    /// <summary>
    /// this class handles the screen during the game play
    /// </summary>
    class GamePlayScreen
    {
        // constants
        const float maxEnemyHeight = 0.1f;
        const float minEnemyHeight = 0.5f;
        const float maxEnemyVelocity = 5.0f;
        const float minEnemyVelocity = 1.0f;
        const float cannonSpeed = 5f;
        const int maxCannonBalls = 2;
        const int avgTimeOfSupergunGoodie = 15000;
        const int avgTimeOfShieldGoodie = 15000;
        const int timeOfSupergun = 10000;
        const int timeOfShield = 15000;

        // variable time values
        double supergunTime = 0;
        double supergunGoodieTime = 5000;
        double shieldGoodieTime = 10000;
        double shieldTime = 0;
        double healthGoodieTime = 5000;

        int lastScore;

        int score;                  // current score of player
        int lifes = 3;              // current amount of lifes (each player starts with 3 lifes)


        private Game1 game;         // the game object
        
        // textures
        Texture2D backgroundTexture;
        Texture2D lifeGraphics;

        // game objects
        GameObject cannon;                                          // standard game object
        GameObject cannon_backup;                                   // backup of the game object (during the goodie time)
        GameObject cannon_shield;                                   // extends the standard game object (with a shield)
        GameObject supergunGoodie;                                  // goodie for getting a better laser gun
        GameObject shieldGoodie;                                    // goodie for protecting the player from damaged
        GameObject healthGoodie;                                    // goodie for getting one more life
        GameObject[] cannonBalls;                                   // cannon balls of player
        GameObject lastEnemyShooted = null;                         // last enemy which shooted a cannon ball
        GameObject[] enemyCannonBalls = new GameObject[0];          // cannon balls of enemys
        GameObject[] enemies = new GameObject[0];                   // all enemies
        List<GameObject> damagedEnemies = new List<GameObject>();   // all enemies that are currently damaged (needed for sprite animations)


        KeyboardState oldState;                                     // previous keyboard state
        KeyboardState previousKeyboardState = Keyboard.GetState();

        Random random = new Random();

        Level currentLevel;                                         // game level
        Boolean paused = false;                                     // is game paused or not

        // Sprite animations
        SpriteAnimation damagedCannon;                              // sprite for animating damaged cannon
        SpriteAnimation reloadedCannon;                             // sprite for reloaded cannon (got a goodie)
        List<SpriteAnimation> damagedEnemiesSprites = new List<SpriteAnimation>(); // list of all currently sprite animations (for damaged enemies)

        SpriteFont font;                                            // game font

        // vectors for positioning
        Vector2 scoreDrawPoint = new Vector2(0.05f, 0.05f);         // position of the current score
        Vector2 levelDrawPoint = new Vector2(0.15f, 0.05f);         // position of the current level
        Vector2 goodieTimePoint = new Vector2(0.05f, 0.15f);        // position of the shield goodie time (if active)
        Vector2 lifesDrawPoint = new Vector2(1f, 0.05f);            // position of the remaining lifes of the player

        Rectangle viewportRect;                                     // the current viewport on screen

        // sound effects
        SoundEffect enemyShootSound;                                // sound when the enemy shoots
        SoundEffect cannonShootSound;                               // sound when the player shoots
        SoundEffect explodeSound;                                   // sound when a ball hits a enemy
        SoundEffect cannonExtShootSound;                            // sound of the better laser gun (when having the supergun goodie)
        SoundEffect doubleKillSound;                                // sound if you hit two enemies with one shot
        SoundEffect multiKillSound;                                 // sound if you hit more than two enemies with one shot
        SoundEffect killingSpreeSound;                              // sound if you get the supergun
        SoundEffect godlikeSound;                                   // sound if you get one more life (life goodie)
        SoundEffect unstoppableSound;                               // sound if you get the shield goodie

        Menu pauseMenu;                                             // pause menu
        
        /// <summary>
        /// constructor for the game play screen
        /// initializes all sounds and game objects (sprites)
        /// </summary>
        /// <param name="game"></param>
        public GamePlayScreen(Game1 game)
        {
            this.game = game;
            
            enemyShootSound = game.getContentManager().Load<SoundEffect>("Audio\\Waves\\enemyshoot");
            cannonShootSound = game.getContentManager().Load<SoundEffect>("Audio\\Waves\\cannonshoot");
            cannonExtShootSound = game.getContentManager().Load<SoundEffect>("Audio\\Waves\\cannonshoot_ext");
            explodeSound = game.getContentManager().Load<SoundEffect>("Audio\\Waves\\explode");
            doubleKillSound = game.getContentManager().Load<SoundEffect>("Audio\\Waves\\doublekill");
            multiKillSound = game.getContentManager().Load<SoundEffect>("Audio\\Waves\\monsterkill");
            killingSpreeSound = game.getContentManager().Load<SoundEffect>("Audio\\Waves\\killingspree");
            godlikeSound = game.getContentManager().Load<SoundEffect>("Audio\\Waves\\godlike");
            unstoppableSound = game.getContentManager().Load<SoundEffect>("Audio\\Waves\\unstoppable");
                       
            lifeGraphics = game.getContentManager().Load<Texture2D>("Sprites\\cannon_01_small");
            backgroundTexture = game.getContentManager().Load<Texture2D>("Sprites\\background-space");
            
            cannon = new GameObject(game.getContentManager().Load<Texture2D>(
                "Sprites\\cannon_01"));

            cannon_backup = new GameObject(game.getContentManager().Load<Texture2D>(
                "Sprites\\cannon_01"));

            cannon_shield = new GameObject(game.getContentManager().Load<Texture2D>(
                "Sprites\\cannon_01_shield"));

            supergunGoodie = new GameObject(game.getContentManager().Load<Texture2D>(
                "Sprites\\extra_gun"));
            healthGoodie = new GameObject(game.getContentManager().Load<Texture2D>(
                "Sprites\\extra_life"));
            shieldGoodie = new GameObject(game.getContentManager().Load<Texture2D>(
                "Sprites\\extra_shield"));

            cannon.position = new Vector2(
                game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height - 30);

            cannonBalls = new GameObject[maxCannonBalls];

            for (int i = 0; i < maxCannonBalls; i++)
            {
                cannonBalls[i] = new GameObject(
                    game.getContentManager().Load<Texture2D>(
                    "Sprites\\cannon_laser"));
            }

            // initialize level of the game
            currentLevel = new Level(game);
            currentLevel.initLevel(ref enemies, ref enemyCannonBalls);
            // load game font
            font = game.getContentManager().Load<SpriteFont>("Fonts\\GameFont");
            // initialize pause menu
            pauseMenu = new Menu(Color.White, Color.LightBlue, game.getContentManager().Load<SpriteFont>("Fonts\\GameFont"), false);
            // add menu items
            pauseMenu.AddMenuItem("Fortsetzen", MenuChoice.CONTINUE, new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 50, game.GraphicsDevice.Viewport.Height / 2 - 100));
            pauseMenu.AddMenuItem("Beenden", MenuChoice.EXIT, new Vector2(game.GraphicsDevice.Viewport.Width / 2 - 50, game.GraphicsDevice.Viewport.Height / 2 - 50));

            // create a Rectangle that represents the full
            // drawable area of the game screen.
            viewportRect = new Rectangle(0, 0,
                game.GraphicsDevice.Viewport.Width,
                game.GraphicsDevice.Viewport.Height);

            // increase game play sound to 80% (game sound is initially lower on the start screen)
            MediaPlayer.Volume = 0.8f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if the supergun goodie is currently enabled</returns>
        private bool supergunEnabled()
        {
            return supergunTime > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if the shield goodie is currently enabled</returns>
        private bool shieldEnabled()
        {
            return cannon_shield.alive;
        }

        /// <summary>
        /// updates all game objects
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {   
            KeyboardState keyboardState = Keyboard.GetState();

            // handle cannon movement
            if (keyboardState.IsKeyDown(Keys.Left) && cannon.position.X >= 0)
            {
                if (damagedCannon == null || damagedCannon.isAnimationOver()) // moving of cannon is only allowed if it gets not hitted
                    cannon.position.X -= cannonSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.Right) && cannon.position.X <= viewportRect.Width)
            {
                if (damagedCannon == null || damagedCannon.isAnimationOver()) // moving of cannon is only allowed if it gets not hitted
                    cannon.position.X += cannonSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.Escape) && !pauseMenu.isVisible())
            {
                // show pause menu
                BeginPause();
            }

            if (keyboardState.IsKeyDown(Keys.Down) && pauseMenu.isVisible() && !oldState.IsKeyDown(Keys.Down))
            {
                // select next menu item in pause menu
                pauseMenu.SelectNext();
            }

            if (keyboardState.IsKeyDown(Keys.Up) && pauseMenu.isVisible() && !oldState.IsKeyDown(Keys.Up))
            {
                // select previous menu item in pause menu
                pauseMenu.SelectPrev();
            }

            if (keyboardState.IsKeyDown(Keys.Enter) && pauseMenu.isVisible())
            {
                // user choose a menu item -> handle that by querying active menu item
                if (pauseMenu.GetSelectedItem().Equals(MenuChoice.CONTINUE))
                {
                    EndPause();
                }

                else if (pauseMenu.GetSelectedItem().Equals(MenuChoice.EXIT))
                {
                    game.ExitCurrentGame();
                }
            }

            // save current keyboard state as oldstate
            oldState = keyboardState;

            // update game only if it´s not paused
            if (!isGamePaused())    
            {
                // update supergun time
                if (supergunEnabled())
                    supergunTime -= gameTime.ElapsedGameTime.Milliseconds;

                // only fire cannon ball if player has pressed button
                // this update loop - do not fire cannon ball if
                // button is merely held down.
                if (keyboardState.IsKeyDown(Keys.Up) &&
                    previousKeyboardState.IsKeyUp(Keys.Up))
                {
                    FireCannonBall();
                }

                // handles fire of enemy cannon balls
                FireEnemyCannonBall();

                // update routines for all game objects (cannon balls, enemies, goodies)
                UpdateCannonBalls(gameTime);
                UpdateEnemyCannonBalls();
                UpdateEnemies();
                UpdateDamagedEnemies(gameTime);
                UpdateDamagedCannon(gameTime);
                UpdateSupergunGoodie(gameTime);
                UpdateHealthGoodie(gameTime);
                UpdateShieldGoodie(gameTime);
                UpdateCannonShield(gameTime);
                UpdateReloadedCannon(gameTime);
                UpdateLevel();

                // reset previous input states to current states
                // for next Update call.                
                previousKeyboardState = keyboardState;

            } // if game is not paused

            // update menu  
            pauseMenu.Update(gameTime);
        }

        /// <summary>
        /// enables the supergun goodie for the player
        /// </summary>
        private void enableSupergun()
        {
            supergunTime = timeOfSupergun;
        }

        /// <summary>
        /// enables the shield goodie for the player
        /// </summary>
        private void enableShield()
        {
            cannon_shield.alive = true;
            shieldTime = timeOfShield;
        }

        /// <summary>
        /// disables the shield goodie for the player
        /// </summary>
        private void disableShield()
        {
            cannon_shield.alive = false;
            shieldTime = 0;            
        }

        /// <summary>
        /// Increases the level if score is high enough.
        /// </summary>
        public void UpdateLevel()
        {
            // increase level on every 10 points
            if (score % 10 == 0 && score != lastScore)
            {
                currentLevel.increaseLevel(ref enemies, ref enemyCannonBalls);
            }

            lastScore = score;
        }

        /// <summary>
        /// updates enemy positions, kills them if
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
        /// places a cannon ball object into the game world
        /// by setting position and velocity
        /// </summary>
        public void FireCannonBall()
        {
            foreach (GameObject ball in cannonBalls)
            {
                if (!ball.alive)
                {
                    if (supergunEnabled())
                        ball.sprite = game.getContentManager().Load<Texture2D>(
                            "Sprites\\supercannon_laser");
                    else
                        ball.sprite = game.getContentManager().Load<Texture2D>(
                            "Sprites\\cannon_laser");

                    ball.alive = true;
                    ball.position = cannon.position;

                    if (shieldEnabled())
                        ball.position.X += 66;
                    
                    //Determine velocity using sine and
                    //cosine of the cannon's rotation angle,
                    //then scale by 5 to speed up the ball.
                    //Console.WriteLine(cannon.rotation);
                    float cannon_rotation = cannon.rotation - (float)Math.PI / 2;
                    ball.velocity = new Vector2(
                        (float)Math.Cos(cannon_rotation),
                        (float)Math.Sin(cannon_rotation))
                    * 20.0f;

                    if (supergunEnabled())
                        cannonExtShootSound.Play();
                    else
                        cannonShootSound.Play();

                    return;
                }
            }
        }

        /// <summary>
        /// places a shield goodie object into the game world if there isn´t one there
        /// </summary>
        public void FireShieldGoodie()
        {
            if (!shieldGoodie.alive)
            {
                shieldGoodie.alive = true;

                shieldGoodie.position = new Vector2(random.Next(10, viewportRect.Width - 10), 0);
                shieldGoodie.velocity = new Vector2(0, 5f);
            }
        }

        /// <summary>
        /// places a supergun goodie into the game world if there isn´t one there
        /// </summary>
        public void FireSupergunGoodie()
        {
            if (!supergunGoodie.alive)
            {
                supergunGoodie.alive = true;

                supergunGoodie.position = new Vector2(random.Next(10, viewportRect.Width - 10), 0);
                supergunGoodie.velocity = new Vector2(0, 5f);
            }
        }

        /// <summary>
        /// places a health goodie into the game world if there isn´t one there
        /// </summary>
        public void FireHealthGoodie()
        {
            if (!healthGoodie.alive && lifes < 3)
            {
                healthGoodie.alive = true;

                healthGoodie.position = new Vector2(random.Next(10, viewportRect.Width - 10), 0);
                healthGoodie.velocity = new Vector2(0, 5f);
            }
        }

        /// <summary>
        /// places a cannon ball object into the game world
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
                        enemyToShoot = enemies[random.Next(currentLevel.MaxEnemies)];
                    } while (enemyToShoot == lastEnemyShooted);

                    lastEnemyShooted = enemyToShoot;

                    ball.position = enemyToShoot.position;

                    //Determine velocity using vertical and horizontal
                    //distance between enemy and cannon
                    //then scale by currentLevel.EnemyCannonSpeed to speed up the ball.
                    Vector2 normV = new Vector2(
                        cannon.position.X - ball.position.X,
                        cannon.position.Y - ball.position.Y);

                    normV.Normalize();

                    ball.velocity = normV * currentLevel.EnemyCannonSpeed;

                    enemyShootSound.Play(0.5f);

                    return;
                }
            }
        }
        
        /// <summary>
        /// updates the cannon shield by reducing the shield time every update cycle.
        /// cannon shield will be disabled if shield time <= 0
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateCannonShield(GameTime gameTime)
        {
            if (shieldEnabled())
            {
                shieldTime -= gameTime.ElapsedGameTime.Milliseconds;    // Update shieldtime
            }

            if (shieldTime < 0 )
            {
                disableShield();
                cannon.sprite = game.getContentManager().Load<Texture2D>(
                "Sprites\\cannon_01");
                cannon.position = new Vector2(cannon.position.X + 66, cannon.position.Y + 50);
            }
        }
            
        /// <summary>
        /// updates the reloaded cannon (cannon with supergun)
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateReloadedCannon(GameTime gameTime)
        {
            if (reloadedCannon != null)
            {
                if (cannon_shield.alive)
                    reloadedCannon.Position = new Vector2(cannon.position.X+23, cannon.position.Y+13);
                else
                    reloadedCannon.Position = new Vector2(cannon.position.X - cannon.sprite.Width + 43, cannon.position.Y - 37);

                reloadedCannon.Update(gameTime);
            }
                
        }

        /// <summary>
        /// updates the animations for the damaged enemies
        /// and checks whether the sprite animations are over (in this case, remove all finished sprite animations)
        /// </summary>
        /// <param name="gameTime"></param>
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
        /// updates the cannon when they was hitted by a cannon ball
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateDamagedCannon(GameTime gameTime)
        {
            if (damagedCannon != null)
                damagedCannon.Update(gameTime);
        }

        /// <summary>
        /// updates the goodie game object for getting one more life
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateHealthGoodie(GameTime gameTime)
        {
            if (healthGoodie.alive)
            {
                healthGoodie.position += healthGoodie.velocity;

                if (!viewportRect.Contains(new Point(
                    (int)healthGoodie.position.X,
                    (int)healthGoodie.position.Y)))
                {
                    healthGoodie.alive = false;
                }

                Rectangle healthGoodieRect = new Rectangle(
                    (int)healthGoodie.position.X,
                    (int)healthGoodie.position.Y,
                    healthGoodie.sprite.Width,
                    healthGoodie.sprite.Height);

                Rectangle cannonRect = new Rectangle(
                    (int)cannon.position.X - 42,
                    (int)cannon.position.Y - 42,
                    cannon.sprite.Width,
                    cannon.sprite.Height);

                if (healthGoodieRect.Intersects(cannonRect))
                {
                    healthGoodie.alive = false;

                    godlikeSound.Play();
                  
                    // animate cannon
                    reloadedCannon = new SpriteAnimation(
                           game.getContentManager().Load<Texture2D>("Sprites\\cannon_01_reloaded"), 2);
                    reloadedCannon.Position = new Vector2(cannon.position.X - cannon.sprite.Width + 43, cannon.position.Y - 37);
                    reloadedCannon.IsLooping = false;
                    reloadedCannon.FramesPerSecond = 5;
                }

            }
            else
            {
                // healthGoodie in every third level (at random time in this level)
                if (healthGoodieTime <= 0 && currentLevel.GetLevel %3 == 0 && currentLevel.LastHealthLevel != currentLevel.GetLevel)
                {
                    FireHealthGoodie();
                    currentLevel.LastHealthLevel = currentLevel.GetLevel;
                    healthGoodieTime = random.Next(5000, 10000);
                }
                else
                {
                    healthGoodieTime -= gameTime.ElapsedGameTime.Milliseconds;
                }

            }
        }

        /// <summary>
        /// updates the goodie game object for getting a protection shield
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateShieldGoodie(GameTime gameTime)
        {
            if (shieldGoodie.alive)
            {
                shieldGoodie.position += shieldGoodie.velocity;
                if (!viewportRect.Contains(new Point(
                    (int)shieldGoodie.position.X,
                    (int)shieldGoodie.position.Y)))
                {
                    shieldGoodie.alive = false;
                }

                Rectangle shieldGoodieRect = new Rectangle(
                    (int)shieldGoodie.position.X,
                    (int)shieldGoodie.position.Y,
                    shieldGoodie.sprite.Width,
                    shieldGoodie.sprite.Height);

                Rectangle cannonRect = new Rectangle(
                    (int)cannon.position.X-42,
                    (int)cannon.position.Y-42,
                    cannon.sprite.Width,
                    cannon.sprite.Height);

                if (shieldGoodieRect.Intersects(cannonRect))
                {
                    shieldGoodie.alive = false;

                    // update cannon sprite
                    cannon_backup = cannon;
                    cannon.sprite = cannon_shield.sprite;                    
                    cannon.position = new Vector2(cannon.position.X - 66, cannon.position.Y - 50);
                    enableShield();
                    unstoppableSound.Play();
                }
            }
            else
            {
                if (shieldGoodieTime <= 0 && cannon_shield.alive == false)
                {
                    FireShieldGoodie();
                    shieldGoodieTime = avgTimeOfShieldGoodie + random.Next(-10000, 10000);
                }
                else
                {
                    shieldGoodieTime -= gameTime.ElapsedGameTime.Milliseconds;
                }

            }

        }

        /// <summary>
        /// updates the goodie game object for getting a supergun
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateSupergunGoodie(GameTime gameTime)
        {
            if (supergunGoodie.alive)
            {
                supergunGoodie.position += supergunGoodie.velocity;
                if (!viewportRect.Contains(new Point(
                    (int)supergunGoodie.position.X,
                    (int)supergunGoodie.position.Y)))
                {
                    supergunGoodie.alive = false;
                }

                Rectangle supergunGoodieRect = new Rectangle(
                    (int)supergunGoodie.position.X,
                    (int)supergunGoodie.position.Y,
                    supergunGoodie.sprite.Width,
                    supergunGoodie.sprite.Height);

                Rectangle cannonRect = new Rectangle(
                    (int)cannon.position.X - 42,
                    (int)cannon.position.Y - 42,
                    cannon.sprite.Width,
                    cannon.sprite.Height);

                if (supergunGoodieRect.Intersects(cannonRect))
                {
                    supergunGoodie.alive = false;

                    killingSpreeSound.Play();

                    // animate cannon
                    reloadedCannon = new SpriteAnimation(
                           game.getContentManager().Load<Texture2D>("Sprites\\cannon_01_reloaded"), 2);
                    reloadedCannon.Position = new Vector2(cannon.position.X - cannon.sprite.Width + 43, cannon.position.Y - 37);
                    reloadedCannon.IsLooping = false;
                    reloadedCannon.FramesPerSecond = 5;

                    enableSupergun();
                }
            }
            else
            {
                // no supergun in the world (check if a new supergun goodie can be placed into the world)
                if (supergunGoodieTime <= 0)
                {
                    FireSupergunGoodie();

                    supergunGoodieTime = avgTimeOfSupergunGoodie + random.Next(-5000, 5000);
                }
                else
                {
                    supergunGoodieTime -= gameTime.ElapsedGameTime.Milliseconds;
                }

            }
        }

        /// <summary>
        /// moves cannon balls, handles cannon balls
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

                        if (ball.killed == 2)
                        {
                            doubleKillSound.Play();
                        }
                        else if (ball.killed > 2)
                        {
                            multiKillSound.Play();
                        }

                        ball.killed = 0;

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
                            if (!supergunEnabled())
                                ball.alive = false;
                            else
                                ball.killed++;

                            enemy.alive = false;

                            //Hitting an enemy with a cannon ball
                            //counts as a score point.
                            score += 1;
                            damagedEnemies.Add(enemy);

                            // load sprite animation for damaged enemies
                            SpriteAnimation enemyAnim = new SpriteAnimation(
                            game.getContentManager().Load<Texture2D>("Sprites\\enemy_01_damagedSprite"), 4);
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
                        (int)cannon.position.X-42,
                        (int)cannon.position.Y-42,
                        //(int)cannon.position.X-cannon.sprite.Width/2,
                        //(int)cannon.position.Y-cannon.sprite.Height/2,
                        cannon.sprite.Width,
                        cannon.sprite.Height);

                    if (cannonBallRect.Intersects(cannonRect))
                    {
                        ball.alive = false;

                        //Hitting an enemy with a cannon ball
                        //counts as a score point.
                        if (lifes > 0 && !cannon_shield.alive)
                            lifes--;

                        // display hitted cannon
                        // load sprite animation for damaged enemies
                        if (!cannon_shield.alive)
                        {
                            damagedCannon = new SpriteAnimation(
                            game.getContentManager().Load<Texture2D>("Sprites\\cannon_01_damagedSprite"), 3);
                            damagedCannon.Position = new Vector2(cannon.position.X - cannon.sprite.Width + 43, cannon.position.Y - 37);
                            damagedCannon.IsLooping = false;
                            damagedCannon.FramesPerSecond = 20;
                            explodeSound.Play();                       
                        }
                        else
                            explodeSound.Play();                       

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// draws the current game world within one sprite batch
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {

            // draw the backgroundTexture sized to the width
            // and height of the screen.
            spriteBatch.Draw(backgroundTexture, viewportRect,
                Color.White);

            // draw goodies if they are currently alive
            if (supergunGoodie.alive)
                spriteBatch.Draw(supergunGoodie.sprite,
                    supergunGoodie.position, Color.White);

            if (shieldGoodie.alive)
                spriteBatch.Draw(shieldGoodie.sprite,
                    shieldGoodie.position, Color.White);

            if (healthGoodie.alive)
                spriteBatch.Draw(healthGoodie.sprite,
                    healthGoodie.position, Color.White);

            // draw all cannon balls if they are currently alive
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

            // draw the cannon game object
            spriteBatch.Draw(cannon.sprite,
                cannon.position, null, Color.White,
                cannon.rotation,
                cannon.center, 1.0f,
                SpriteEffects.None, 0);

            // draw enemies
            foreach (GameObject enemy in enemies)
            {
                if (enemy.alive)
                {
                    spriteBatch.Draw(enemy.sprite,
                        enemy.position, Color.White);
                }
            }

            // construct a score string and draw to
            // near top-left corner of the screen.
            spriteBatch.DrawString(font,
                "SCORE" + "\n" + score.ToString("#00000"),
                new Vector2(scoreDrawPoint.X * viewportRect.Width,
                scoreDrawPoint.Y * viewportRect.Height),
                Color.Turquoise);

            // construct a level string and draw to
            // near top-left corner of the screen.
            spriteBatch.DrawString(font,
                "LEVEL" + "\n" + currentLevel.ToString("#00000"),
                new Vector2(levelDrawPoint.X * viewportRect.Width,
                levelDrawPoint.Y * viewportRect.Height),
                Color.Yellow);

            // construct a shield goodie time string and draw it (only if shield is enabled)
            if (shieldEnabled())
            {
                spriteBatch.DrawString(font,
                "SHIELD TIME " + "\n" + shieldTime.ToString("#00000"),
                new Vector2(goodieTimePoint.X * viewportRect.Width,
                    goodieTimePoint.Y * viewportRect.Height), Color.Red);
            }
            else if (supergunEnabled())
            {
                // construct a shield goodie time string and draw it
                spriteBatch.DrawString(font,
                "SUPERGUN TIME " + "\n" + supergunTime.ToString("#00000"),
                new Vector2(goodieTimePoint.X * viewportRect.Width,
                    goodieTimePoint.Y * viewportRect.Height), Color.Red);
            }
                
            // draw life graphics
            for (int i = 0; i < lifes; i++)
                spriteBatch.Draw(lifeGraphics, new Vector2(lifesDrawPoint.X * viewportRect.Width - 30 * (lifes - i) - 30, lifesDrawPoint.Y * viewportRect.Height), Color.White);

            if (lifes < 1)                
                game.showGameOver(score);

            // draw menu
            pauseMenu.Draw(spriteBatch, false);

            // draw damaged enemies
            foreach (SpriteAnimation spriteAnim in damagedEnemiesSprites)
            {
                spriteAnim.Draw(spriteBatch);
            }

            // draw damaged cannon
            if (damagedCannon != null)
                damagedCannon.Draw(spriteBatch);

            // draw reloaded cannon
            if (reloadedCannon != null)
                reloadedCannon.Draw(spriteBatch);

            // reset cannon if animation is over
            if (damagedCannon != null && damagedCannon.isAnimationOver())
                damagedCannon = null;

            if (reloadedCannon != null && reloadedCannon.isAnimationOver())
                reloadedCannon = null;

            
            //test rectangle to check collission-area
            //Texture2D rectangle;
            //rectangle = CreateRectangle(cannon.sprite.Width, cannon.sprite.Height);
            //spriteBatch.Draw(rectangle, new Vector2(cannon.position.X-cannon.sprite.Width/2,cannon.position.Y-cannon.sprite.Height/2), Color.White);
            //spriteBatch.Draw(rectangle, new Vector2(cannon.position.X-42,cannon.position.Y-42), Color.White);
            
        }

        
        /// <summary>
        /// test routine for creating a rectangle in order to check collision-areas
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Texture2D CreateRectangle(int width, int height)
        {
            Texture2D rectangleTexture = new Texture2D(game.GraphicsDevice, width, height, 1, TextureUsage.None,
            SurfaceFormat.Color);// create the rectangle texture, ,but it will have no color! lets fix that
            Color[] color = new Color[width * height];//set the color to the amount of pixels

            for (int i = 0; i < color.Length; i++)//loop through all the colors setting them to whatever values we want
            {
                color[i] = Color.Red;
            }
            rectangleTexture.SetData(color);//set the color data on the texture
            return rectangleTexture;
        }
        

        /// <summary>
        /// enable pause of game
        /// </summary>
        public void BeginPause()
        {
            paused = true;
            pauseMenu.setVisibilty(true);
        }

        /// <summary>
        /// disable pause of game
        /// </summary>
        public void EndPause()
        {
            paused = false;
            pauseMenu.setVisibilty(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>return state of game (paused or not)</returns>
        public bool isGamePaused()
        {
            return paused;
        }
    }
}
