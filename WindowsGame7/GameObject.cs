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

namespace XNAUfoAttack
{
    /// <summary>
    /// Represents the objects in the game
    /// </summary>
    class GameObject
    {
        public Texture2D sprite;    // sprite for the object
        public Vector2 position;    // object position on screen
        public bool alive;          // is object alive?
        public int killed = 0;      // how many enemies were killed by the GameObject
        public Vector2 velocity;    // the velocity of a game object

        /// <summary>
        /// GameObject constructor
        /// </summary>
        /// <param name="loadedTexture">Texture for the game object</param>
        public GameObject(Texture2D loadedTexture)
        {
            position = Vector2.Zero;

            sprite = loadedTexture;

            // Object is initially dead
            alive = false;
        }
    }
}
