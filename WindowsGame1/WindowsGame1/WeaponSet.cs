using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class WeaponSet
    {
        public Weapon setWeapon(Weapon.weapon weapon)
        {
            Weapon weap;
            weap = new Weapon();
            if (weapon == Weapon.weapon.IronSword) weap = IronSword();
            if (weapon == Weapon.weapon.SlimSword) weap = SlimSword();
            if (weapon == Weapon.weapon.SteelSword) weap = SteelSword();
            if (weapon == Weapon.weapon.IronLance) weap = IronLance();
            if (weapon == Weapon.weapon.SlimLance) weap = SlimLance();
            if (weapon == Weapon.weapon.SteelLance) weap = SteelLance();
            if (weapon == Weapon.weapon.IronAxe) weap = IronAxe();
            if (weapon == Weapon.weapon.SteelAxe) weap = SteelAxe();
            return weap;
            
        }
        public Weapon IronSword()
        {
            Weapon weapon;
            weapon = new Weapon();
            weapon.name = Weapon.weapon.IronSword;
            weapon.weaponType = Weapon.triangle.sword;
            weapon.rank = Weapon.weaponRank.E;
            weapon.range = 1;
            weapon.wt = 5;
            weapon.mt = 5;
            weapon.hit = 90;
            weapon.crt = 0;
            weapon.uses = 46;
            weapon.wex = 1;
            weapon.worth = 460;
            return weapon;
        }
        public Weapon SlimSword()
        {
            Weapon weapon;
            weapon = new Weapon();
            weapon.name = Weapon.weapon.SlimSword;
            weapon.weaponType = Weapon.triangle.sword;
            weapon.rank = Weapon.weaponRank.E;
            weapon.range = 1;
            weapon.wt = 2;
            weapon.mt = 3;
            weapon.hit = 100;
            weapon.crt = 5;
            weapon.uses = 30;
            weapon.wex = 1;
            weapon.worth = 480;
            return weapon;
        }
        public Weapon SteelSword()
        {
            Weapon weapon;
            weapon = new Weapon();
            weapon.name = Weapon.weapon.SteelSword;
            weapon.rank = Weapon.weaponRank.D;
            weapon.weaponType = Weapon.triangle.sword;
            weapon.range = 1;
            weapon.wt = 10;
            weapon.mt = 8;
            weapon.hit = 75;
            weapon.crt = 0;
            weapon.uses = 30;
            weapon.wex = 1;
            weapon.worth = 600;
            return weapon;
        }
        public Weapon IronLance()
        {
            Weapon weapon;
            weapon = new Weapon();
            weapon.name = Weapon.weapon.IronLance;
            weapon.rank = Weapon.weaponRank.E;
            weapon.weaponType = Weapon.triangle.lance;
            weapon.range = 1;
            weapon.wt = 8;
            weapon.mt = 7;
            weapon.hit = 80;
            weapon.crt = 0;
            weapon.uses = 45;
            weapon.wex = 1;
            weapon.worth = 360;
            return weapon;
        }
        public Weapon SlimLance()
        {
            Weapon weapon;
            weapon = new Weapon();
            weapon.name = Weapon.weapon.SlimLance;
            weapon.rank = Weapon.weaponRank.E;
            weapon.weaponType = Weapon.triangle.lance;
            weapon.range = 1;
            weapon.wt = 4;
            weapon.mt = 4;
            weapon.hit = 85;
            weapon.crt = 5;
            weapon.uses = 30;
            weapon.wex = 1;
            weapon.worth = 420;
            return weapon;
        }
        public Weapon SteelLance()
        {
            Weapon weapon;
            weapon = new Weapon();
            weapon.name = Weapon.weapon.SteelLance;
            weapon.rank = Weapon.weaponRank.D;
            weapon.weaponType = Weapon.triangle.lance;
            weapon.range = 1;
            weapon.wt = 13;
            weapon.mt = 10;
            weapon.hit = 70;
            weapon.crt = 0;
            weapon.uses = 30;
            weapon.wex = 1;
            weapon.worth = 480;
            return weapon;
        }
        public Weapon IronAxe()
        {
            Weapon weapon;
            weapon = new Weapon();
            weapon.name = Weapon.weapon.IronAxe;
            weapon.rank = Weapon.weaponRank.E;
            weapon.weaponType = Weapon.triangle.axe;
            weapon.range = 1;
            weapon.wt = 10;
            weapon.mt = 8;
            weapon.hit = 75;
            weapon.crt = 0;
            weapon.uses = 45;
            weapon.wex = 1;
            weapon.worth = 270;
            return weapon;
        }
        public Weapon SteelAxe()
        {
            Weapon weapon;
            weapon = new Weapon();
            weapon.name = Weapon.weapon.SteelAxe;
            weapon.rank = Weapon.weaponRank.E;
            weapon.weaponType = Weapon.triangle.axe;
            weapon.range = 1;
            weapon.wt = 15;
            weapon.mt = 11;
            weapon.hit = 65;
            weapon.crt = 0;
            weapon.uses = 30;
            weapon.wex = 2;
            weapon.worth = 360;
            return weapon;
        }
    }
}
