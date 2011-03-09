using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace XNAUfoAttack
{

    public class Menu
    {

        private static int maxItems = 20;   // maximum items in menu
        private int menuItemCount;          // current amount of items in menu
        private int curMenuItem;            // index of currently selected item
        private string[] menuItems;         // array for the names of menu items
        private Vector2[] pos;              // array for the position of menu items
        private double[] scale;             // array for the scale of menu items
        private Color unselected;           // color for unselected items
        private Color selected;             // color for currently selected items
        private SpriteFont font;            // font object to render menu items
        private Boolean visibility;         // is menu currently visible or not
        private MenuChoice[] choice;        // for each item a unique identifier (custom enum type "MenuChoice") is saved

        private float minScale = 1.0f;      // the minimum scale for each item
        private float maxScale = 1.2f;      // the maximum scale for each item

        
        /// <summary>
        /// constructs a menu
        /// </summary>
        /// <param name="unselectedColor">the color for inactive items</param>
        /// <param name="selectedColor">the color for the active item</param>
        /// <param name="font">the font object to render text</param>
        /// <param name="visibility">determines if this menu is currently visible or not</param>
        public Menu(Color unselectedColor, Color selectedColor, SpriteFont font, Boolean visibility)
        {
            this.font = font;
            this.menuItems = new string[maxItems];
            this.pos = new Vector2[maxItems];
            this.scale = new double[maxItems];
            this.unselected = unselectedColor;
            this.selected = selectedColor;
            this.menuItemCount = 0;
            this.curMenuItem = 0;
            this.visibility = visibility;
            this.choice = new MenuChoice[maxItems];
        }

        /// <summary>
        /// adds a new menu item to the menu
        /// </summary>
        /// <param name="name">the name of the menu item to render</param>
        /// <param name="c">a unique identifier for that item (Enum type MenuChoice)</param>
        /// <param name="p">position of the new item</param>
        public void AddMenuItem(string name, MenuChoice c, Vector2 p)
        {   
            if (menuItemCount < maxItems)
            {
                menuItems[menuItemCount] = name;
                scale[menuItemCount] = minScale;
                choice[menuItemCount] = c;
                pos[menuItemCount++] = p;
            }
        }

        /// <summary>
        /// selects the next menu item
        /// </summary>
        public void SelectNext()
        {
            if (curMenuItem < menuItemCount - 1)
            {
                curMenuItem++;
            }
            else
            {
                curMenuItem = 0;
            }
        }

        /// <summary>
        /// selects the previous menu item
        /// </summary>
        public void SelectPrev()
        {
            if (curMenuItem > 0)
            {
                curMenuItem--;
            }
            else
            {
                curMenuItem = menuItemCount - 1;
            }
        }

        /// <summary>
        /// sets the visibility of the menu
        /// </summary>
        /// <param name="visibility">true if menu should be visible</param>
        public void setVisibilty(Boolean visibility)
        {
            this.visibility = visibility;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>the visibility of the menu</returns>
        public Boolean isVisible()
        {
            return visibility;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns>the unique identifier of the currently selected menu item</returns>
        public MenuChoice GetSelectedItem()
        {
            return choice[curMenuItem];
        }

        /// <summary>
        /// updates the menu (scaling for active and inactive menu items)
        /// the scale establishes a small animation effect (selected menu items are getting a little bit larger through the scale)
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            //Durchläuft alle Menüpunkte
            for (int i = 0; i < menuItemCount; i++)
            {
                //Lässt die Skalierung des aktuellen Punktes langsam größer werden
                if (i == curMenuItem)
                {
                    if (scale[i] < maxScale)
                    {
                        scale[i] += 0.04 + 10.0f * gameTime.ElapsedGameTime.Seconds;
                    }
                }
                //Lässt die Skalierung aller anderen Menüpunkte langsam kleiner werden
                else if (scale[i] > minScale && i != curMenuItem)
                {
                    scale[i] -= 0.04 + 10.0f * gameTime.ElapsedGameTime.Seconds;
                }
            }
        }

        
        /// <summary>
        /// draws the complete menu (with a smooth scaling effect for the currently selected menu item)
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="standalone">if true the menu is rendered "standalone", so that spriteBatch.Begin() and spriteBatch.End() are called inside this method</param>
        public void Draw(SpriteBatch spriteBatch, bool standalone)
        {

            if (isVisible()) // draw menu only if its visible
            {
                if (standalone)
                    spriteBatch.Begin();

                // render all menu items
                for (int i = 0; i < menuItemCount; i++)
                {
                    Vector2 p = pos[i];                 // p saves the position of every menu item
                    p.X -= (float)(22 * scale[i] / 2);  // move the position within the X-axis to center the name
                    p.Y -= (float)(22 * scale[i] / 2);  // move the position within the X-axis to center the name
                    spriteBatch.DrawString(font, menuItems[i], p, selected, 0.0f, new Vector2(0, 0), (float)scale[i], SpriteEffects.None, 0);                  
                }
   
                if (standalone)
                    spriteBatch.End();
            }
            
        }
    }
}
