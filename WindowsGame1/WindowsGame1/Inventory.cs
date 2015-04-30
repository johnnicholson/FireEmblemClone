using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Xml.Serialization;

namespace WindowsGame1
{
    [Serializable]
    public class Inventory
    {
        public InvObject[] arr;
        public int totalSize;
        public int Used;
        [XmlIgnore]
        public SpriteFont Font;
        public int view { get; set; }
        public Inventory() { }
        public Inventory(SpriteFont font)
        {
            view = 0;
            Font = font;
            arr = new InvObject[10];
            Used = 0;
            totalSize = 10;
        }
        public InvObject Check(int i)
        {
            return arr[i];
        }
        public void Add(InvObject obj)
        {
            if (Used < totalSize)
            {
                arr[Used] = obj;
            }
            Used++;
        }
        public InvObject Remove(int position)
        {
            InvObject obj;
            obj = arr[position];
            if (position == Used){
                arr[position] = null;

        }
            if (position < Used)
            {
                int x = 0;
                while (position + x + 1 < Used)
                {
                    arr[position+ x] = arr[position + x + 1];
                    x++;
                }
                arr[Used - 1] = null;
                Used--;
            }
            return obj;
        }
        public int HowMany()
        {
            return Used;
        }
        public void Insert(int position, InvObject obj)
        {
            if (position == Used + 1)
            {
                arr[position] = obj;
            }
            if (position <= Used)
            {
                int x = 0;
                while (position + x <Used)
                {
                    arr[Used - x] = arr[Used - 1 - x];
                    x++;
                }
                arr[position] = obj;
                Used++;
            }
        }
        public Weapon FirstWeapon()
        {
            foreach(Weapon value in arr){
                return value;
            }
            return null;
        }
        public void PrintAll(Vector2 location, SpriteBatch spriteBatch)
        {
            if(Used <= 5)
                for (int x = 0; x < Used; x++)
                {
                    spriteBatch.DrawString(Font, arr[x].ToShortString(), new Vector2(location.X, location.Y + 90 * x), Color.Black);
                }
            if (Used > 5)
                for (int x = 0; x < 5; x++)
                {
                    spriteBatch.DrawString(Font, arr[x + view].ToShortString(), new Vector2(location.X, location.Y + 90 * x), Color.Black);
                }
        }
    }
}
