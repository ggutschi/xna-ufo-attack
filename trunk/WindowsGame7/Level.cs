using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame7
{
    class Level
    {
        private Game1 game;

        int level = 1;
        int maxEnemies = 2;
        int maxEnemyCannonBalls = 1;
        float enemyCannonSpeed = 1.6f;
        int lastHealthLevel = 0;

        public Level(Game1 game)
        {
            this.game = game;
            
        }

        public int GetLevel
        {
            get
            {
                return this.level;
            }
          
        }

        public int LastHealthLevel
        {
            get
            {
                return this.lastHealthLevel;
            }
            set
            {
                this.lastHealthLevel = value;
            }

        }

        public int MaxEnemyCannonBalls
        {
            get
            {
                return this.maxEnemyCannonBalls;
            }
        }

        public int MaxEnemies
        {
            get
            {
                return this.maxEnemies;
            }
        }

        public float EnemyCannonSpeed
        {
            get
            {
                return this.enemyCannonSpeed;
            }
        }

        public void increaseLevel(ref GameObject[] enemies, ref GameObject[] enemyCannonBalls)
        {
            level++;

            if (level % 4 == 0)
                maxEnemies++;

            if (level % 2 == 0)
                maxEnemyCannonBalls++;

            if (level % 3 == 0)
                enemyCannonSpeed += 0.2f;

            initLevel(ref enemies, ref enemyCannonBalls);
        }

        public string ToString(String format)
        {
            return this.level.ToString(format);
        }

        public void initLevel(ref GameObject[] enemies, ref GameObject[] enemyCannonBalls)
        {
            GameObject[] tmpEnemies = new GameObject[maxEnemies];

            for (int i = 0; i < enemies.Length; i++)
                tmpEnemies[i] = enemies[i];

            for (int i = enemies.Length; i < maxEnemies; i++)
            {
                tmpEnemies[i] = new GameObject(
                    game.getContentManager().Load<Texture2D>("Sprites\\enemy_01"));
            }

            enemies = tmpEnemies;


            GameObject[] tmpEnemyCannonBalls = new GameObject[maxEnemyCannonBalls];

            for (int i = 0; i < enemyCannonBalls.Length; i++)
                tmpEnemyCannonBalls[i] = enemyCannonBalls[i];

            for (int i = enemyCannonBalls.Length; i < maxEnemyCannonBalls; i++)
            {
                tmpEnemyCannonBalls[i] = new GameObject(
                    game.getContentManager().Load<Texture2D>("Sprites\\cannonball"));
            }

            enemyCannonBalls = tmpEnemyCannonBalls;
        }
    }
}
