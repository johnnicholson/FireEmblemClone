using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    public class Weapon : InvObject
    {


        public enum weapon { IronSword, SlimSword, SteelSword,IronLance, SlimLance, SteelLance, IronAxe, SteelAxe }
        public enum triangle { sword, axe, lance }
        public enum weaponRank { E, D, C, B, A, S }
        public weapon name { get; set; }
        public triangle weaponType { get; set; }
        public int wt { get; set; }
        public int mt { get; set; }
        public int hit { get; set; }
        public int crt { get; set; }
        public int uses { get; set; }
        public int wex { get; set; }

        public int range { get; set; }
        public weaponRank rank { get; set; }
        public override string ToShortString()
        {
            string a = name.ToString();
            string b = wt.ToString();
            string c = mt.ToString();
            string d = worth.ToString();
            return a + "\nwt: " + b + " mt: " + c + "\ncost: " + d ;
        }
        public override string ToString()
        {
            string a = name.ToString();
            string b = wt.ToString();
            string c = mt.ToString();
            string d = hit.ToString();
            string e = crt.ToString();
            string f = uses.ToString();
            
            return a + " wt: " + b + " mt: " + c + " hit: " + d + " crt: " + e + " uses: " + f;
        }
    }
}
