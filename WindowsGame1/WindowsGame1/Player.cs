using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace WindowsGame1
{
    [Serializable]
    public class Player
    {
        [XmlIgnore]
        public AnimatedSprite sprite;
        public Vector2 location;
        public bool moved, attacked;
        public Inventory inv { get; set; }
        public int exp { get; set; }
        public int level { get; set; }

        public enum CLIST{Pirate = 0, Mercenary = 1, Pupil = 2, Knight = 3, Cleric = 4, Cavalier = 5,
				Soldier = 6, Myrmidon = 7, Swordlord = 8, Recruit = 9, Thief = 10, Monk = 11,
				Archer = 12, Axelord = 13, WyvernRider = 14}
        public CLIST CLASS;
        public Player() { }
        public Player(int x, int y, Texture2D texture, int rows, int columns, SpriteFont font)
        {
            this.location.X = x;
            this.location.Y = y;
            inv = new Inventory(font);
            moved = false;
            attacked = false;
            sprite = new AnimatedSprite(texture, rows, columns, 20);
            level = 1;
        }
        public void Update()
        {
            sprite.Update();
        }
        public void reset()
        {
            moved = false;
            attacked = false;
        }
        public int getSpeed()
        {
            if (this.inv.FirstWeapon() == null) return 0;
            if (this.inv.FirstWeapon().wt < this.con) return this.spd;
            int i = this.spd - this.inv.FirstWeapon().wt + this.con;
            return i;
        }
        public void MakeSprite(Texture2D texture, int rows, int columns)
        {
            sprite = new AnimatedSprite(texture, rows, columns, 20);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch, location);
        }
        public void DrawInventory(Vector2 location, SpriteBatch spriteBatch)
        {
            inv.PrintAll(location, spriteBatch);
        }
        public override string ToString()
        {

            return "Class: "  + CLASS + "   HP:  " + curhp + "   Str: " + str + "   Skl: " + skl + "   Spd: " + spd + "   Luk: " + luk + "   Def: " + def + "   Res: " + res;
        }
        public void LevelUp()
        {
            exp = 0;
            level++;
            if (level < 20)
            {
                if (RandomNext(0, 99) < hpgr) hp++;
                if (RandomNext(0, 99) < strgr) str++;
                if (RandomNext(0, 99) < sklgr) skl++;
                if (RandomNext(0, 99) < spdgr) spd++;
                if (RandomNext(0, 99) < lukgr) luk++;
                if (RandomNext(0, 99) < defgr) def++;
                if (RandomNext(0, 99) < resgr) res++;
            }
        }
        public int moves { get; set; }
        public int curhp { get; set; }
        public int hp { get; set; }
        public int str { get; set; }
        public int skl { get; set; }
        public int spd { get; set; }
        public int luk { get; set; }
        public int def { get; set; }
        public int res { get; set; }
        public int con { get; set; }
        public int hpgr { get; set; }
        public int strgr { get; set; }
        public int sklgr { get; set; }
        public int spdgr { get; set; }
        public int lukgr { get; set; }
        public int defgr { get; set; }
        public int resgr { get; set; }
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
