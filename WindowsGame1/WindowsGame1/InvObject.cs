using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace WindowsGame1
{
    [XmlInclude(typeof(Weapon))]
    public abstract class InvObject
    {
        public enum type { Weapon, Potion }
        public type objtype { get; set; }
        public int worth { get; set; }
        abstract public string ToShortString();
    }
}
