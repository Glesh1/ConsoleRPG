using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG
{
    public class Entity
    {
        //fields all entities have:
        private string name = "Default Béla";
        private int hp = 100, mp = 70, gold = 20, xp = 1, defense = 2, damage = 3, vitality = 100, 
            level = 1, maxHp = 100, maxMP=10, dexterity = 1;
        public bool isParrying, isWaiting;

        //Properties +getters/setters
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Hp { get; set; }
        public int Mp { get; set; }
        public int Gold { get; set; }
        public int Xp { get; set; }
        public int Defense { get; set; }
        public int Dexterity { get; set; }
        public int Damage { get; set; }
        public int Vitality { get; set; }

        public int Level { get; set; }

        public int MaxHp {get; set;}
        public int MaxMp { get; set; }



        //constructor wo/ params
        //public Entity() { }
        public Entity(string _name, int _hp, int _mp, int _gold, int _xp, int _defense, int _dexterity, int _damage, int _vitality, int _level, int _maxHp, int _maxMp)
        {
            Name = _name;
            Hp = _hp;
            Mp = _mp;
            Gold = _gold;
            Xp = _xp;
            Defense = _defense;
            Dexterity = _dexterity;
            Damage = _damage;
            Vitality = _vitality;
            Level = _level;
            MaxHp = _maxHp;
            MaxMp = _maxMp;
        }

        public void Damages(Entity _enemy)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if ((Damage - _enemy.Defense) > 0)
            {
                if ((GetChanceToHit() + Level) > 70)
                {
                    _enemy.Hp -= (Damage - _enemy.Defense);
                    if (_enemy.Hp < 0) { _enemy.Hp = 0; }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} attacks {1}, {1}'s HP is {2}.", Name, _enemy.Name, _enemy.Hp);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else 
                {
                    Console.WriteLine("{0} attacks {1}, but misses strike.", Name, _enemy.Name);
                }
            }
            else
            {
                Random rnd = new Random();
                if (rnd.Next(0, 4) == 0) { //25% chance to break through high defense
                    _enemy.Hp -= Damage; if (_enemy.Hp < 0) { _enemy.Hp = 0; }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} was able to break through {1}'s high Defense, damaging {1}. {1}'s HP is {2}.",
                        Name, _enemy.Name, _enemy.Hp);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine("{0}'s attack was blocked due to {1}'s high Defense.", Name, _enemy.Name);
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        
        public void Parry(Entity _enemy)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{0} gets ready to parry.", Name);
            if (_enemy.Dexterity > Dexterity) 
            {
                Console.WriteLine("{0} moves faster than expected...", _enemy.Name);
                Console.ForegroundColor = ConsoleColor.White;
                Random rnd = new Random();
                if (rnd.Next(0, 3) == 0)
                {   //33% chance to parry even if enemy dex is higher than players
                    //Console.WriteLine("Still, {1} manages to counter {0}'s attack.", Name, _enemy.Name);
                    ToggleState("parry");
                }
                else {
                    //Console.WriteLine("In an attempt to counter {0}'s attack {1} falls. " +
                    //    "{1}'s Defense is 0.", _enemy.Name, Name); 
                    ToggleState("idle"); }
                
            }
            else 
            {
                if (_enemy.isWaiting)
                {
                    if (Defense > 1) { Defense -= 1; }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("{0} takes a quick step backwards as {1} tries to parry making {1} fall to the ground. " +
    "{1}'s Defense is {2}.", _enemy.Name, Name, Defense);
                    Console.ForegroundColor = ConsoleColor.White;
                    ToggleState("idle");
                }
                else
                {
                    Console.WriteLine("It seems {0} is in luck, {1} is quite slow...", Name, _enemy.Name);
                    Console.ForegroundColor = ConsoleColor.White;
                    ToggleState("parry");
                }
            }
        }
        public void Wait(Entity _enemy)
        {
            if (_enemy.isParrying && _enemy.Defense > 0) {
                 _enemy.Defense -= 1; _enemy.ToggleState("idle"); ToggleState("wait"); 
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0} takes a quick step backwards as {1} " +
                    "tries to parry making {1} fall to the ground. " +
                    "{1}'s Defense is {2}.", Name, _enemy.Name, _enemy.Defense);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else { if (Hp > Math.Ceiling(Level/(float)2)) { Hp -= (int)Math.Ceiling(Level/(float)2); ToggleState("wait"); }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Standing still won't make you fit... {0}'s HP decreases to {1}.", Name, Hp); }
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void ToggleState(string state) {
            switch (state)
            {
                case "parry": isParrying = true; isWaiting = false; break;
                case "wait": isWaiting = true; isParrying = false; break;
                case "idle": isParrying = false; isWaiting = false; break;
                default:
                    break;
            }
        }
        public void ShowStats() {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(
            "{0}'s stats are: Level {1}, HP {2}, MP {3}, Damage {4}, Defense {5}, Dexterity {6}, Vitality {7}, Gold {8}, XP {9}.",
            Name, Level, Hp, Mp, Damage, Defense, Dexterity, Vitality, Gold, Xp);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public int GetChanceToHit()
        {
            Random rnd = new Random();
            return rnd.Next(50, 101);
        }
    }
}
