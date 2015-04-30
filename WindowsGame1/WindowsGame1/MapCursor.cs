using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;

namespace WindowsGame1
{
    class MapCursor
    {
        public Vector2 location { get; set; }
        private bool visible;
        Texture2D texture;
        double elapsed;
        KeyboardState oldKeyBoard;
        int mapHeight { get; set; }
        int mapWidth { get; set; }

        public MapCursor(Vector2 _location, Texture2D _texture, int hieght, int width)
        {
            mapHeight = hieght;
            mapWidth = width;
            this.location = _location;
            texture = _texture;
            visible = true;
            elapsed = 0f;
            
        }
        public void Update(GameTime gametime, KeyboardState keyboard)
        {

            elapsed += gametime.ElapsedGameTime.TotalSeconds;
            if((elapsed>0.9f) && visible == true)
            {
                visible = !visible;
                elapsed = 0;
            }
            if ((elapsed > 0.1f) && visible == false)
            {
                visible = !visible;
                elapsed = 0;
            }
            positionUpdate(keyboard);
        }
        public void UpdateAnim(GameTime gametime)
        {

            elapsed += gametime.ElapsedGameTime.TotalSeconds;
            if ((elapsed > 0.9f) && visible == true)
            {
                visible = !visible;
                elapsed = 0;
            }
            if ((elapsed > 0.1f) && visible == false)
            {
                visible = !visible;
                elapsed = 0;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                
                spriteBatch.Begin();
                spriteBatch.Draw(texture, new Rectangle((int)location.X * 16,(int) location.Y * 16, 16, 16), Color.White*0.6f);
                spriteBatch.End();
            }
        }
        private void positionUpdate(KeyboardState keyBoard)
        {

            if (keyBoard.IsKeyDown(Keys.W) && oldKeyBoard.IsKeyUp(Keys.W) && location.Y > 0)
            {
                this.location = new Vector2(location.X, location.Y - 1);
            }
            if (keyBoard.IsKeyDown(Keys.S) && oldKeyBoard.IsKeyUp(Keys.S)  && location.Y < mapHeight - 1)
            {
                this.location = new Vector2(location.X, location.Y + 1);
            }
            if (keyBoard.IsKeyDown(Keys.A) && oldKeyBoard.IsKeyUp(Keys.A) && location.X > 0)
            {
                this.location = new Vector2(location.X - 1, location.Y);
            }
            if (keyBoard.IsKeyDown(Keys.D) && oldKeyBoard.IsKeyUp(Keys.D) && location.X < mapWidth - 1)
            {
                this.location = new Vector2(location.X + 1, location.Y);
            }

            oldKeyBoard = keyBoard;
        }
    }
}
