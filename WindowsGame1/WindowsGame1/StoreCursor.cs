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
    class StoreCursor
    {
        public int position { get; set; }
        private bool visible;
        Texture2D texture;
        double elapsed;
        KeyboardState oldKeyBoard;
        public int focus { get; set; }

        public StoreCursor(int _position, Texture2D _texture)
        {
            this.position = _position;
            texture = _texture;
            visible = true;
            elapsed = 0f;
            
        }
        public void Update(GameTime gametime, KeyboardState keyboard, int max)
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
            positionUpdate(keyboard, max);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {

                spriteBatch.Draw(texture, new Rectangle(35 + 150 * focus,80 + position * 90, 145, 70), Color.Gold*0.4f);

            }
        }
        private void positionUpdate(KeyboardState keyBoard, int max)
        {

            if (keyBoard.IsKeyDown(Keys.W) && oldKeyBoard.IsKeyUp(Keys.W) && position>0)
            {
                this.position -= 1;
            }
            if (keyBoard.IsKeyDown(Keys.S) && oldKeyBoard.IsKeyUp(Keys.S) && position < 4 && position < max - 1)
            {
                this.position += 1;
            }
            if (keyBoard.IsKeyDown(Keys.A) && oldKeyBoard.IsKeyUp(Keys.A) && focus > 0)
            {
                position = 0;
                focus -= 1;
            }
            if (keyBoard.IsKeyDown(Keys.D) && oldKeyBoard.IsKeyUp(Keys.D) && focus < 4)
            {
                position = 0;
                focus += 1;
            }
            oldKeyBoard = keyBoard;
        }
    }
}
