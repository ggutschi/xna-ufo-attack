using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame7
{
    public class GameOverScreen
    {
        private Texture2D texture;
        private Game1 game;
        private KeyboardState lastState;


        public GameOverScreen(Game1 game)
        {
            this.game = game;
            texture = game.getContentManager().Load<Texture2D>("Sprites\\GameOverScreen");
            lastState = Keyboard.GetState();
        }

        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Enter) && lastState.IsKeyUp(Keys.Enter))
            {
                game.ExitGame();
            }

            lastState = keyboardState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
                spriteBatch.Draw(texture, new Vector2(0f, 0f), Color.White);
        }
    }
}
