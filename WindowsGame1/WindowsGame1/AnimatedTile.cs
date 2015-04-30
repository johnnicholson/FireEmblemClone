using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Security.Cryptography;

namespace WindowsGame1
{
    public class AnimatedTile
    {
        public Texture2D Texture { get; set; }
        public int row { get; set; }
        private int currentFrame;
        private int delay;
        
        public AnimatedTile(Texture2D texture, Tile.Type type, int delayValue)
        {
            if (type == Tile.Type.Grass)
            {
                row = 0;
                currentFrame = RandomNext(0, 8);
            }
            if (type == Tile.Type.River)
            {
                row = 7;
                currentFrame = 0;
            }
            if (type == Tile.Type.Mountain)
            {
                row = 17;
                currentFrame = 0;
            }
            if (type == Tile.Type.Forest)
            {
                row = 21;
                currentFrame = 1;
            }
            if (type == Tile.Type.Sand)
            {
                row = 13;
                currentFrame = 0;
            }
            if (type == Tile.Type.Bridge)
            {
                row = 3;
                currentFrame = 0;
            }
            if (type == Tile.Type.Fort)
            {
                row = 11;
                currentFrame = 0;
            }
            

            Texture = texture;
            delay = delayValue;
        }
        public void Update()
        {
            /*if (count == delay)
            {
                currentFrame++;
                count = 0;
                if (currentFrame == totalFrames)
                    currentFrame = 0;
            }
            count++;
        
             */
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 location, bool Highlighted, bool Red)
        {
            int width = 17;
            int height = 17;
            Rectangle sourceRectangle = new Rectangle(17 * currentFrame, row * 17, 17, 17);
            Rectangle destinationRectangle = new Rectangle((int)location.X*16, (int)location.Y*16, width, height);

            spriteBatch.Begin();

                spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
            if (Highlighted)
                spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.LightBlue*.9f);
            if (Red)
                spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.PaleVioletRed * .8f);
            spriteBatch.End();
        }
        static RandomNumberGenerator _rand = RandomNumberGenerator.Create();

        static int RandomNext(int min, int max)
        {
            if (min > max) throw new ArgumentOutOfRangeException("min");

            byte[] bytes = new byte[4];

            _rand.GetBytes(bytes);

            uint next = BitConverter.ToUInt32(bytes, 0);

            int range = max - min;

            return (int)((next % range) + min);
        }
    }
}
