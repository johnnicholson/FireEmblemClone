using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    class PQ
    {
        Queue<Pair>[] arr;
        public struct Pair
        {
            public Pair(Vector2 value, int key)
            {
                this.Value = value;
                this.Key = key;
            }
            public Vector2 Value;
            public int Key;
        }
        public PQ()
        {
           

            arr = new Queue<Pair>[50];
            arr[0] = new Queue<Pair>();
            arr[1] = new Queue<Pair>();
            arr[2] = new Queue<Pair>();
            arr[3] = new Queue<Pair>();
            arr[4] = new Queue<Pair>();
            arr[5] = new Queue<Pair>();
            arr[6] = new Queue<Pair>();
            arr[7] = new Queue<Pair>();
            arr[8] = new Queue<Pair>();
            arr[9] = new Queue<Pair>();
            arr[10] = new Queue<Pair>();
            arr[11] = new Queue<Pair>();
            arr[12] = new Queue<Pair>();
            arr[13] = new Queue<Pair>();
            arr[14] = new Queue<Pair>();
            arr[15] = new Queue<Pair>();
            arr[16] = new Queue<Pair>();
            arr[17] = new Queue<Pair>();
            arr[18] = new Queue<Pair>();
            arr[19] = new Queue<Pair>();
            arr[20] = new Queue<Pair>();
            arr[21] = new Queue<Pair>();
            arr[22] = new Queue<Pair>();
            arr[23] = new Queue<Pair>();
            arr[24] = new Queue<Pair>();
            arr[25] = new Queue<Pair>();
            arr[26] = new Queue<Pair>();
            arr[27] = new Queue<Pair>();
            arr[28] = new Queue<Pair>();
            arr[29] = new Queue<Pair>();
            arr[30] = new Queue<Pair>();
            arr[31] = new Queue<Pair>();
            arr[32] = new Queue<Pair>();
            arr[33] = new Queue<Pair>();
            arr[34] = new Queue<Pair>();
            arr[35] = new Queue<Pair>();
            arr[36] = new Queue<Pair>();
            arr[37] = new Queue<Pair>();
            arr[38] = new Queue<Pair>();
            arr[39] = new Queue<Pair>();
            arr[40] = new Queue<Pair>();
            arr[41] = new Queue<Pair>();
            arr[42] = new Queue<Pair>();
            arr[43] = new Queue<Pair>();
            arr[44] = new Queue<Pair>();
            arr[45] = new Queue<Pair>();
            arr[46] = new Queue<Pair>();
            arr[47] = new Queue<Pair>();
            arr[48] = new Queue<Pair>();
            arr[49] = new Queue<Pair>();

        }
        public void Add(int key,Vector2 vector)
        {
            if (key < 50)
            arr[key].Enqueue(new Pair(vector, key));
        }
        public Pair Remove()
        {
            int x = 0;
            for(x = 0; x <= 50; x++){
                if (arr[x].Count > 0)
                {
                    return arr[x].Dequeue();
                }
            }
            return new Pair(new Vector2(-1, -1), 100);
        }
        public Pair Peek()
        {
            int x = 0;
            for (x = 0; x <= 50; x++)
            {
                if (arr[x].Count > 0)
                {
                    return arr[x].Peek();
                }
            }
            return new Pair(new Vector2(-1, -1), 100);
        }
        public bool empty()
        {
            {
                int x = 0;
                for (x = 0; x < 50; x++)
                {
                    if (arr[x].Count > 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
