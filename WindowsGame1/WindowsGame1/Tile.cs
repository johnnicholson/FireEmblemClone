using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    public class Tile
    {
        public enum Type { Grass, Mountain, River, Forest, Sand, Bridge, Fort }
        Point location;
        public Type type;
        AnimatedTile sprite;
        public bool highlighted;
        public int def { get; set; }
        public int avoid { get; set; }
        public bool highlightedRed;
        public int moveCost { get; set; }
        public Tile(int x, int y, char c, Texture2D texture)
        {
            this.highlighted = false;
            location.X = x;
            location.Y = y;
            if (c == 'b')
            {
                avoid = 0;
                def = 0;
                type = Type.Bridge;
                moveCost = 1;

            }
            if (c == 'g')
            {
                avoid = 0;
                def = 0;
                type = Type.Grass;
                moveCost = 1;

            }
            else if (c == 'r')
            {
                avoid = 0;
                def = 0;
                type = Type.River;
                moveCost = 15;
            }
            else if (c == 's')
            {
                avoid = 5;
                def = 0;
                type = Type.Sand;
                moveCost = 2;
            }
            else if (c == 't')
            {
                avoid = 20;
                def = 1;
                type = Type.Forest;
                moveCost = 2;
            }
            else if (c == 'm')
            {
                avoid = 30;
                def = 1;
                type = Type.Mountain;
                moveCost = 4;
            }
            else if (c == 'f')
            {
                avoid = 20;
                def = 2;
                type = Type.Fort;
                moveCost = 2;
            }
            sprite = new AnimatedTile(texture,type, 30);

        }
        public void Update()
        {
            sprite.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            sprite.Draw(spriteBatch, new Vector2(location.X, location.Y), highlighted, highlightedRed);
           
        }
    }
}
