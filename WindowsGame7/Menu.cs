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


namespace WindowsGame7
{

    public class Menu
    {

        private static int maxItems = 20;   //Gibt die maximale Anzahl der Men�eintr�ge an
        private int menuItemCount;          //Gibt die aktuelle Anzahl aller Men�eintr�ge an
        private int curMenuItem;            //Gibt den aktuellen Men�eintragindex an
        private string[] menuItems;         //Array mit allen Men�namen
        private Vector2[] pos;              //Array mit allen Men�positionen
        private double[] scale;             //Array mit allen Skalierungen der Men�eintr�ge
        private Color unselected;           //Farbe der aktuell nicht ausgew�hlten Eintr�ge
        private Color selected;             //Farbe des ausgew�hlten Eintrages
        private SpriteFont font;            //Fontdatei des Men�s
        private Boolean visibility;         //regelt die Sichtbarkeit des Men�s
        private MenuChoice[] choice;          //speichert den aktuell ausgew�hlten Men�punkt

        
        //Constructor des Men�s
        //Initialisiert Standardwerte
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

        //Methode zum Erstellen eines neuen Men�eintrages
        //�bergibt den Namen und Position
        public void AddMenuItem(string name, MenuChoice c, Vector2 p)
        {
            //Pr�ft ob bereits die maximale Anzahl erreicht wurde
            if (menuItemCount < maxItems)
            {
                menuItems[menuItemCount] = name;
                scale[menuItemCount] = 1.0f;
                choice[menuItemCount] = c;
                pos[menuItemCount++] = p;
            }
        }

        //Methode um den n�chsten Men�punkt auszuw�hlen
        //Nach letztem Men�punkt zum ersten zur�ckspringen
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

        //Methode um den letzten Men�punkt auszuw�hlen
        //Nach erstem Men�punkt zum letzten zur�ckspringen
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

        // setzt die Sichtbarkeit des Men�s
        public void setVisibilty(Boolean visibility)
        {
            this.visibility = visibility;
        }

        // liefert die Sichtbarkeit des Men�s zur�ck
        public Boolean isVisible()
        {
            return visibility;
        }



        //Gibt den aktuellen Namen des aktuellen Men�punktes zur�ck
        public MenuChoice GetSelectedItem()
        {
            return choice[curMenuItem];
        }

        //Updatet das Men�
        public void Update(GameTime gameTime)
        {
            //Durchl�uft alle Men�punkte
            for (int i = 0; i < menuItemCount; i++)
            {
                //L�sst die Skalierung des aktuellen Punktes langsam gr��er werden
                if (i == curMenuItem)
                {
                    if (scale[i] < 1.5f)
                    {
                        scale[i] += 0.04 + 10.0f * gameTime.ElapsedGameTime.Seconds;
                    }
                }
                //L�sst die Skalierung aller anderen Men�punkte langsam kleiner werden
                else if (scale[i] > 1.0f && i != curMenuItem)
                {
                    scale[i] -= 0.04 + 10.0f * gameTime.ElapsedGameTime.Seconds;
                }
            }
        }

        //Methode um das komplette Men� zu zeichnen
        //�bergibt die Spritebatch und einen Boolschen Wert der abgibt ob sich das Men� au�erhalb von
        //spriteBatch.Begin()/.End() befindet
        public void Draw(SpriteBatch spriteBatch, bool standalone)
        {

            if (isVisible()) // zeichne Men� nur dann, wenn es sichtbar sein soll
            {
                if (standalone)
                    spriteBatch.Begin();

                //Durchl�uft alle Men�punkte und zeichnet den Men�punkt mit den entsprechenden Eigenschaften
                for (int i = 0; i < menuItemCount; i++)
                {
                    if (i == curMenuItem)
                    {
                        Vector2 p = pos[i];                 //Weist die Position einer tempor�ren Variable zu
                        p.X -= (float)(22 * scale[i] / 2);  //Verschiebt die Position der x-Achse um den Namen mittig anzuzeigen
                        p.Y -= (float)(22 * scale[i] / 2);  //Verschiebt die Position der y-Achse um den Namen mittig anzuzeigen
                        spriteBatch.DrawString(font, menuItems[i], p, selected, 0.0f, new Vector2(0, 0), (float)scale[i], SpriteEffects.None, 0);
                    }
                    else
                    {
                        Vector2 p = pos[i];                 //Weist die Position einer tempor�ren Variable zu
                        p.X -= (float)(22 * scale[i] / 2);  //Verschiebt die Position der x-Achse um den Namen mittig anzuzeigen
                        p.Y -= (float)(22 * scale[i] / 2);  //Verschiebt die Position der y-Achse um den Namen mittig anzuzeigen
                        spriteBatch.DrawString(font, menuItems[i], p, unselected, 0.0f, new Vector2(0, 0), (float)scale[i], SpriteEffects.None, 0);
                    }
                }
   
                if (standalone)
                    spriteBatch.End();
            }
            
        }
    }
}
