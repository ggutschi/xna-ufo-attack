#region File Description
//-----------------------------------------------------------------------------
// GameObject.cs
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
    class GameObject
    {
        public Texture2D sprite;
        public Vector2 position;
        public float rotation;
        public Vector2 center;
        public Vector2 velocity;
        public bool alive;
        public int killed = 0;

        public GameObject(Texture2D loadedTexture)
        {
            rotation = 0.0f;
            position = Vector2.Zero;

            sprite = loadedTexture;

            //The "center" of a sprite is the point
            //halfway down its width and height.
            center = new Vector2(
                sprite.Width / 2, sprite.Height / 2);

            //All objects should start with zero velocity, and
            //be "dead".
            velocity = Vector2.Zero;
            alive = false;
        }
    }
}
