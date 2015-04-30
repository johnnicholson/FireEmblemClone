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
    class ShopMenuCursor
    {
        public int position { get; set; }
        private bool visible;
        Texture2D texture;
        double elapsed;
        KeyboardState oldKeyBoard;

        public ShopMenuCursor(int _position, Texture2D _texture)
        {
            this.position = _position;
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
        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {

                spriteBatch.Draw(texture, new Rectangle(50, position * 20 + 520, 90, 22), Color.Gold*0.6f);

            }
        }
        private void positionUpdate(KeyboardState keyBoard)
        {

            if (keyBoard.IsKeyDown(Keys.W) && oldKeyBoard.IsKeyUp(Keys.W) && position>0)
            {
                this.position -= 1;
            }
            if (keyBoard.IsKeyDown(Keys.S) && oldKeyBoard.IsKeyUp(Keys.S) && position <1)
            {
                this.position += 1;
            }

            oldKeyBoard = keyBoard;
        }
    }
}
