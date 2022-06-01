using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace ConsoleRPG
{
    class Program
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int MAXIMIZE = 3;

        public static bool gameOver = false;
        public static bool isEnemyTurn = false;
        public static Player player = new Player("defaultName", 100, 10, 10, 0, 0, 5, 0, 1, 10, 0, 1, 1, 1, 0.5f, 100, 10);
        public static Entity enemy1 = new Entity("Skeleton", 10, 0, 2, 11, 1, 1, 1, 1, 1, 10, 0);
        public static Entity lockedEnemy;
        public static int storedDef;
        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            ShowWindow(ThisConsole, MAXIMIZE);
            Intro();
            //GAMELOOP
            while (!gameOver) 
            { 
                if (!isEnemyTurn)
                {
                    //Console.WriteLine("BEGINING OF PLAYER ROUND");
                    if (CheckPulse(lockedEnemy))
                    { NextRound(); }
                    else
                    { EnemyKilled(); }

                    if (CheckPulse(lockedEnemy))
                    {
                        //Console.WriteLine("ENEMY PULSE CHECK TRUE.");
                        ManageInput(lockedEnemy, player, EnemyChoice());
                        CheckPulse(player);
                    }
                    else { EnemyKilled(); }

                    isEnemyTurn = !isEnemyTurn;
                    //Console.WriteLine("END OF PLAYER ROUND");
                }
                else 
                {
                    //Console.WriteLine("BEGINING OF ENEMY ROUND");
                    if (CheckPulse(lockedEnemy))
                    {
                        //Console.WriteLine("ENEMY PULSE CHECK TRUE.");
                        ManageInput(lockedEnemy, player, EnemyChoice());
                        CheckPulse(player);
                    }
                    else { EnemyKilled(); }

                    NextRound();
                    isEnemyTurn = !isEnemyTurn;
                    //Console.WriteLine("END OF ENEMY ROUND");
                }
            }
            Console.WriteLine("GAME OVER :(");
            Console.ReadLine();
            Console.ReadLine();
        }

        public static void Intro() {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Welcome to ConsoleRPG by GyoGyo!");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Please, enter your name: ");
            player.Name = Console.ReadLine().ToString();
            storedDef = player.Defense;
            lockedEnemy = enemy1;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Hello {0}! \nNow You enter The Dark Dungeon " +
              "and an enemy {1} appears in front of you, \nwhat would you like to do?", player.Name, lockedEnemy.Name);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void NextRound()
        {
            //Console.WriteLine("ENEMY PULSE CHECK TRUE.");
            bool validInput = false;
            while (!validInput)
            {
                Console.WriteLine("Your options are: (A)ttack, (P)arry, (W)ait, (H)eal, (E)nchant, (B)uy Mana potion, (S)how stats.");
                var input = Console.ReadLine().ToLower();
                //Console.Clear();
                validInput = ManageInput(player, lockedEnemy, input);
                if (!validInput) { Console.WriteLine("Please repeat instruction."); }
            }
        }

        public static bool ManageInput(Entity _active, Entity _passive, string _input)
        {
            switch (_input)
            {
                case "a":
                    _active.ToggleState("idle");
                    if (_passive.isParrying)
                    {
                        if (_active.Defense > 0) { _active.Defense -= 1; _passive.ToggleState("idle");
                            /*Console.WriteLine("DEBUG: passive parry state is: {0} waiting is: {1}",
                            _passive.isParrying, _passive.isWaiting);*/
                        } //if parry successful enemy defense goes DOOOWWWN;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("{0} parries {1}'s attack. {1}'s Defense decreased to {2}", 
                            _passive.Name, _active.Name, _active.Defense);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        _active.Damages(_passive);
                    }
                    return true;
                case "p":
                    _active.Parry(_passive);
                    return true;

                case "w":
                    _active.Wait(_passive);
                    //_active.ToggleState("wait");
                    //Console.ForegroundColor = ConsoleColor.Green;
                    //Console.WriteLine("{0} decides, it's best to rest for a round.", _active.Name);
                    //Console.ForegroundColor = ConsoleColor.White;
                    return true;

                case "h": player.Heal();
                    //_active.ToggleState("idle"); 
                    return true;
                case "s": player.ShowStats(); lockedEnemy.ShowStats();
                    return false;
                case "b": player.BuyManaPotion();
                    return true;
                case "e": player.Enchant(_passive);
                    return true;

                default: return false;
            }
        }

        public static string EnemyChoice() {
            Random rnd = new Random();
            var rndint = rnd.Next(1, 7);
            if (rndint % 6 == 0 ) { return "w"; }
            if (rndint % 6 == 1) { return "p"; }
            else { 
                return "a";
            } //enemy can parry and wait
        }

        public static bool CheckPulse(Entity _entity)
        {
           if (_entity.Hp > 0) { return true; }

           if (_entity.Name == player.Name && _entity.Hp <= 0) { /*Player DEAD*/
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} was killed.", _entity.Name);
                Console.WriteLine("GAME OVER :(");
                Console.ReadLine();
                Console.ReadLine();
                Environment.Exit(0);
                //gameOver = true; 
            }
           if (_entity.Name != player.Name && _entity.Hp <= 0)  { /*LOOT*/ 
                player.Xp += _entity.Xp; player.Gold += _entity.Gold;
                player.Defense = storedDef; //unnecessary
                //Console.WriteLine("DEBUG: player Def restored");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0} was killed by {1}. {1} receives {2} XP and {3} gold.",
                _entity.Name, player.Name, _entity.Xp, _entity.Gold);
                //player.ShowStats();
                Console.ForegroundColor = ConsoleColor.White;
            }

            return false;
        }

        public static void EnemyKilled()
        {
            player.Defense = storedDef;
            //Console.WriteLine("DEBUG: player Def restored.");
            player.ShowStats(); player.CheckPoints();
            storedDef = player.Defense;
            //Console.WriteLine("DEBUG: player Def stored.");
            NewEnemyEncounter();
        }

        public static void NewEnemyEncounter() {
            Entity newEnemy = GenerateNewEntity();
            //MakeSubtype(newEnemy);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nNew enemy arises from the dark: {0}", newEnemy.Name);
            Console.ForegroundColor = ConsoleColor.White;
            newEnemy.ShowStats(); //Introduce enemy
            lockedEnemy = newEnemy;
            isEnemyTurn = false;
        }

        public static Entity GenerateNewEntity() {
            //name hp mp gold xp def dam vit lvl
            Random rnd = new Random();
            int lvl = rnd.Next(1, 5);
            if (player.Level <= 3) { lvl = rnd.Next(1, 5); }
            if (player.Level > 3 && player.Level<=6) { lvl = rnd.Next(3, 7); }
            if (player.Level > 6 && player.Level <= 10) { lvl = rnd.Next(6, 10); }
            if (player.Level > 10) { lvl = rnd.Next(10, 14); }
            
            int hp = rnd.Next(lvl*4, player.Level*10);
            int gold = rnd.Next(0, lvl*2);
            int def = rnd.Next(lvl, lvl*2);
            int dex = rnd.Next(1, 4);
            int dam = rnd.Next(lvl, lvl*3);
            int xp = (int)((hp + dam) / rnd.Next(2, 6));
            string name = "Enemy";
            Entity newEnemy = new Entity(name, hp, 0, gold, xp, def, dex, dam, 1, lvl, hp, 0);
           
            switch (player.Level)
            {
                case 3: newEnemy.Name = "Skeleton"; newEnemy.Dexterity = 10; break;
                case 5: newEnemy.Name = "Werewolf"; newEnemy.Defense = 10; break;
                case 7: newEnemy.Name = "Warlock"; newEnemy.Gold = 10; break;
                case 10: newEnemy.Name = "Dragon"; newEnemy.Damage = 25;  break;
                case 13: newEnemy.Name = "Béla"; newEnemy.Hp = 50; break;
                case 14: newEnemy.Name = "The Real Fucking Béla"; 
                    newEnemy.Hp = player.Hp*2; newEnemy.Damage = player.Damage*2;
                    newEnemy.Defense = player.Defense*2; break;
                default: break;
            }
            return newEnemy;
        }

        //public static  Entity MakeSubtype(Entity newEnemy) {
        //    Random rnd = new Random();
        //    switch (rnd.Next(0, 5))
        //    {
        //        case 0: newEnemy.Name = "Skeleton"; newEnemy.Dexterity = 10;  break;
        //        case 1: newEnemy.Name = "Werewolf"; newEnemy.Defense = 10; break;
        //        case 2: newEnemy.Name = "Warlock"; newEnemy.Gold = 10; break;
        //        case 3: newEnemy.Name = "Dragon"; newEnemy.Hp = 50;break;
        //        case 4: newEnemy.Name = "Béla"; newEnemy.Damage = 25; newEnemy.Xp = 25; break;
        //    }
        //    return newEnemy;
        //}
    }
}
