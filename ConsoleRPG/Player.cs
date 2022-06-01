using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRPG
{
    public class Player : Entity
    {
        //fields only the Player has:
        private int  xpToNextLevel = 100, pointsToDistribute = 0;
        private int strength = 1, magic = 1;
        private float chanceToHit = 60;
        private int healManaCost = 10;
        private int manaPotionCost = 10;
        private int enchantManaCost;

        //Properties only Player has
        public int XpToNextLevel { get; set; }
        public int PointsToDistribute { get; set; }
        public int Strength { get; set; }
        public int Magic { get; set; }
        public float ChanceToHit { get; set; }

        //Constructor of player wo/params
        //public Player() { }

        public Player(string _name, int _hp, int _mp, int _gold, int _xp, int _defense, int _damage, int _vitality,
            int _level, int _xpToNextLevel, int _pointsToDistribute, int _strength, int _magic, int _dexterity, 
            float _chanceToHit, int _maxHp, int _maxMp)
            : base(_name, _hp, _mp, _gold, _xp, _defense, _dexterity, _damage, _vitality, _level, _maxHp, _maxMp)
        {
            XpToNextLevel = _xpToNextLevel;
            PointsToDistribute = _pointsToDistribute;
            Strength = _strength;
            Magic = _magic;
            Dexterity = _dexterity;
            ChanceToHit = _chanceToHit;
        }

        new public void Damages(Entity _enemy)
        {
            if ((Damage - _enemy.Defense) > 0)
            {
                Random rnd = new Random();
                if ((rnd.Next((int)ChanceToHit, 100) -_enemy.Dexterity*2) > 70)
                {
                    _enemy.Hp -= (Damage - _enemy.Defense);
                    if (_enemy.Hp < 0) { _enemy.Hp = 0; }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} attackes {1}, {1}'s HP is {2}.", Name, _enemy.Name, _enemy.Hp);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine("{0} attacks {1}, but misses strike.", Name, _enemy.Name);
                }
            }
            else
            {
                Console.WriteLine("{0}'s attack was blocked due to {1}'s high Defense.", Name, _enemy.Name);
            }
        }

        public void Heal() {
            if (Mp < healManaCost)
            {   Console.ForegroundColor = ConsoleColor.Yellow; 
                Console.WriteLine("Not enough Mana to Heal.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else 
            {   Mp -= healManaCost;
                if ( Hp + 30/*healstrength*/ > MaxHp) { Hp = MaxHp; }
                else { Hp += 30/*healstrength*/; }
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("{0} uses magic to heal. Hp increases to {1}.", Name, Hp);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public void Enchant(Entity _enemy) {
            if (Mp >= enchantManaCost)
            {
                Mp -= enchantManaCost; _enemy.Dexterity -= 5;
                if (_enemy.Dexterity < 0) { _enemy.Dexterity = 0; }
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("{0} uses Enchantment on {1}, making {1} mesmerized by the magic. " +
                        "{1}'s Dexterity decreased to {2}.", Name, _enemy.Name, _enemy.Dexterity);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Not enough Mana to Enchant.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public void BuyManaPotion() {
            if (Gold >= manaPotionCost) { Mp = MaxMp; Gold -= manaPotionCost;
                Console.WriteLine("{0} buys and drinks a mana potion, restoring MP to {1}. ({2} gold left.)",
                    Name, MaxMp, Gold);
            }
        }

        new public void ShowStats()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            LevelUp();
            Console.WriteLine(
            "{0}'s stats are: Level {1}, HP {2}/{3}, MP {4}/{5}, XP {6}/{7},\n Damage {8}, Defense {9}, Vitality {10}, Dexterity {11}, Gold {12}, Points to distribute {13}.",
            Name, Level, Hp, MaxHp, Mp, MaxMp, Xp, XpToNextLevel, Damage, Defense, Vitality, Dexterity, Gold, PointsToDistribute);
            //CheckPoints();
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void LevelUp() {
            if (Level >= 15) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("CONGRATULATIONS, YOU WIN!");
                Console.ReadLine(); Console.ReadLine();
                Environment.Exit(0);
            }
            while (Xp >= XpToNextLevel)
            {
                Console.WriteLine("\nLEVEL UP!");
                Level++;
                Xp -= XpToNextLevel;
                XpToNextLevel += (int)Math.Ceiling(0.1*XpToNextLevel); 
                //10+Level;//BASE XP TO NEXT LVL HARDOCDED AS 10
                PointsToDistribute += 3;
            }
        }

        public void CheckPoints() {
            if (PointsToDistribute > 0)
            {
                bool validinput = false;
                while (!validinput)
                {
                    Console.Write("You have {0} undistributed points, would you like to modify your stats? (Y/N)", PointsToDistribute);
                    string input = Console.ReadLine().ToLower();
                    if (input == "y") { DistributePoints(); validinput = true; }
                    if (input == "n") { validinput = true; }
                }
            }
        }
        public void DistributePoints() {
            bool exit = false;
            while (!exit)
            {
                
                if (PointsToDistribute < 1) { exit = true; break; }
                else {
                    bool validinput = false;
                    while (!validinput) {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("({0} points left.)" +
                            "\nEnter the name of the attribute you would like to modify or enter \"exit\" to exit menu! " +
                            "\n(vit, str, def, mag, dex): ", PointsToDistribute);
                        string input = Console.ReadLine().ToLower();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        switch (input)
                        {
                            case "vit":
                                Vitality++; PointsToDistribute--; MaxHp = 100 + (Vitality * 10); Hp = MaxHp;
                                Console.WriteLine("Vitality is now {0}. Maximum HP increased to {1}!", Vitality, MaxHp);
                                validinput = true; break;
                            case "str":
                                Strength++; PointsToDistribute--; Damage = 5 + Strength; //BASE DAMAGE HARDCODED AS 5!
                                Console.WriteLine("Strength is now {0}. Damage increased to {1}!", Strength, Damage);
                                validinput = true; break;
                            case "def":
                                Defense++; PointsToDistribute--;
                                Console.WriteLine("Defense is now {0}!", Defense);
                                validinput = true; break;
                            case "mag":
                                Magic++; PointsToDistribute--; MaxMp += Magic * 5; Mp = MaxMp;
                                Console.WriteLine("Magic is now {0}! Maximum MP increased to {1}!", Magic, MaxMp);
                                validinput = true; break;
                            case "dex":
                                Dexterity++; PointsToDistribute--; ChanceToHit = 0.6f + Dexterity / (float)50; //BASE CTH HARDCODED AS 0.6
                                Console.WriteLine("Dexterity is now {0}. Chance to hit increased to {1}!", Dexterity, ChanceToHit);
                                validinput = true; break;
                            case "exit": validinput = true; exit = true; break;
                            default: validinput = false; break;
                        }
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}

