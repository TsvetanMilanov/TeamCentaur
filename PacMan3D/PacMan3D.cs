using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

struct Element
{
    //adding comment
    // define new object type with structure
    // screen position x,y
    public int x;
    public int y;
    // type of game object is defined by ASCII char
    public char skin;
    public ConsoleColor colour;
    public string direction;
    public string lastDirection;

}

class PacMan3D
{
    //Global variables
    public static Element enemy = new Element();
    public static Element enemy2 = new Element();
    public static Element enemy3 = new Element();
    public static Element enemy4 = new Element();

    public static Element pacMan = new Element();
    public static string[] labyrinth;

    public static int playfieldHeight;
    public static int playfieldWidth;

    //Check if the game is over variable
    public static bool isGameOver = false;

    //Enemy Even move counter
    public static long enemyEvenMoveCounter = 1;

    
    
    static void Main()
    {
        //Method for creating playing field
        SetPlayfieldSize(out playfieldHeight, out playfieldWidth);

        StartupScreen();
        // initial pacman position in center of playfield
        pacMan.x = (playfieldWidth + 1) / 2 + 1;
        pacMan.y = playfieldHeight / 2;
        pacMan.skin = (char)9787; // utf8 decimal code 9787 (smile face) is our hero character
        pacMan.colour = ConsoleColor.Yellow;

        
        //Enemy stats 
        //Randomizing enmies positions COPY>>>>
        Random randomizerEnemy = new Random();
        int positionEnemyX = randomizerEnemy.Next(1, 5);
        int positionEnemyY = randomizerEnemy.Next(1, 5);
        enemy.x = (playfieldHeight - 3) / positionEnemyX;
        enemy.y = (playfieldWidth - 3) / positionEnemyY;
        enemy.skin = '\u2666'; // utf8 decimal code 2666 (black diamond) is our enemy character COPY>>>>>
        enemy.colour = ConsoleColor.Red;
        int enemyStartDirection = randomizerEnemy.Next(1, 5); // randomizining enemy start direction
        if (enemyStartDirection == 1)
        {
            enemy.direction = "right";
        }
        else if (enemyStartDirection == 2)
        {
            enemy.direction = "left";
        }
        else if (enemyStartDirection == 3)
        {
            enemy.direction = "up";
        }
        else if (enemyStartDirection == 4)
        {
            enemy.direction = "down";
        }
        

        positionEnemyX = randomizerEnemy.Next(1, 5);
        positionEnemyY = randomizerEnemy.Next(1, 5);
        enemy2.x = (playfieldHeight - 3) / positionEnemyX; ;
        enemy2.y = (playfieldWidth - 3) / positionEnemyY;
        enemy2.skin = '\u2666'; // utf8 decimal code 2666 (black diamond) is our enemy character COPY>>>>>>
        enemy2.colour = ConsoleColor.Green;
        int enemy2StartDirection = randomizerEnemy.Next(1, 5); // randomizining enemy start direction
        if (enemy2StartDirection == 1)
        {
            enemy2.direction = "right";
        }
        else if (enemy2StartDirection == 2)
        {
            enemy2.direction = "left";
        }
        else if (enemy2StartDirection == 3)
        {
            enemy2.direction = "up";
        }
        else if (enemy2StartDirection == 4)
        {
            enemy2.direction = "down";
        }

        positionEnemyX = randomizerEnemy.Next(1, 5);
        positionEnemyY = randomizerEnemy.Next(1, 5);
        enemy3.x = (playfieldHeight - 3) / positionEnemyX; ;
        enemy3.y = (playfieldWidth - 3) / positionEnemyY;
        enemy3.skin = '\u2666'; // utf8 decimal code 2666 (black diamond) is our enemy character COPY>>>>>>
        enemy3.colour = ConsoleColor.Cyan;
        int enemy3StartDirection = randomizerEnemy.Next(1, 5); // randomizining enemy start direction
        if (enemy3StartDirection == 1)
        {
            enemy3.direction = "right";
        }
        else if (enemy3StartDirection == 2)
        {
            enemy3.direction = "left";
        }
        else if (enemy3StartDirection == 3)
        {
            enemy3.direction = "up";
        }
        else if (enemy3StartDirection == 4)
        {
            enemy3.direction = "down";
        }

        positionEnemyX = randomizerEnemy.Next(1, 5);
        positionEnemyY = randomizerEnemy.Next(1, 5);
        enemy4.x = (playfieldHeight - 3) / positionEnemyX; ;
        enemy4.y = (playfieldWidth - 3) / positionEnemyY;
        enemy4.skin = '\u2666'; // utf8 decimal code 2666 (black diamond) is our enemy character COPY>>>>>>
        enemy4.colour = ConsoleColor.Magenta;
        int enemy4StartDirection = randomizerEnemy.Next(1, 5); // randomizining enemy start direction
        if (enemy4StartDirection == 1)
        {
            enemy4.direction = "right";
        }
        else if (enemy4StartDirection == 2)
        {
            enemy4.direction = "left";
        }
        else if (enemy4StartDirection == 3)
        {
            enemy4.direction = "up";
        }
        else if (enemy4StartDirection == 4)
        {
            enemy4.direction = "down";
        }
        //COPY>>>>>>
        
     

        int levelNumber = 0; //The number of the level which will be printed.
        int levelsCount = 4; //The count of all the levels in Levels.txt file.
        //2D string array which will contain all the levels.
        string[,] allLevels = new string[levelsCount, playfieldWidth];
        //Read all the levels from the file useing ReadLevelsFromFile().
        allLevels = ReadLevelsFromFile(playfieldHeight, playfieldWidth);

        // Main game logic
        while (true)
        {
            //The array labyrinth is equal to the current level we select.
            labyrinth = selectLevel(allLevels, levelNumber);

            // Move the enemy per 2 steps
            if (enemyEvenMoveCounter % 2 == 0)
            {
                MoveEnemy1();
                MoveEnemy2();
                MoveEnemy3();
                MoveEnemy4();
            }

            // pacman every step
            MovePacMan();

            //Checking for impact when you add enemy you must add the enemy in this method (CheckForImpact())
            CheckForImpact();


            //CheckForImpact gives true or false on isGameOver
            if (isGameOver)
            {

                Console.Clear();
                Console.WriteLine("Game Over");
                // remove some lives points etc.
                break;
            }

            PrintFrame();
            enemyEvenMoveCounter++;
            Thread.Sleep(100);  // control game speed
        }
    }
        
    private static void CheckForImpact()
    {
        //Check for impact
        if ((enemy.x == pacMan.x && enemy.y == pacMan.y) || (enemy2.x == pacMan.x && enemy2.y == pacMan.y) || (enemy3.x == pacMan.x && enemy3.y == pacMan.y) || (enemy4.x == pacMan.x && enemy4.y == pacMan.y))
        {
            Console.WriteLine("Game Over !");
            isGameOver = true;
        }
    }

    private static void PrintFrame()
    {
        Console.Clear();    // fast screen clear
        PrintLabyrinth(labyrinth);
        PrintElement(pacMan);
        PrintElement(enemy);
        PrintElement(enemy2);
        PrintElement(enemy3);
        PrintElement(enemy4);
    }


   
    private static void MovePacMan()
    {
        while (Console.KeyAvailable)
        {
            
            // we assign the pressed key value to a variable pressedKey
            ConsoleKeyInfo pressedKey = Console.ReadKey(true);
            // next we start checking the value of the pressed key and take action if neccessary
            if (pressedKey.Key == ConsoleKey.LeftArrow && pacMan.x > 1) // if left arrow is pressed then
            {
                if (labyrinth[pacMan.y][pacMan.x - 1] != '\u2588')
                {
                    pacMan.x = pacMan.x - 1;
                }
            }
            else if (pressedKey.Key == ConsoleKey.RightArrow && pacMan.x < playfieldWidth - 1)
            {
                if (labyrinth[pacMan.y][pacMan.x + 1] != '\u2588')
                {

                    pacMan.x = pacMan.x + 1;
                }
            }
            else if (pressedKey.Key == ConsoleKey.UpArrow && pacMan.y > 1)
            {
                if (labyrinth[pacMan.y - 1][pacMan.x] != '\u2588')
                {
                    pacMan.y = pacMan.y - 1;
                }
            }
            else if (pressedKey.Key == ConsoleKey.DownArrow && pacMan.y < playfieldHeight - 1)
            {
                if (labyrinth[pacMan.y + 1][pacMan.x] != '\u2588')
                {
                    pacMan.y = pacMan.y + 1;
                }
            }
        }
    }


    #region EnemyMovements

    private static void MoveEnemy1()
    {

        /*Move enemy
            1-right
            2-left
            3-up
            4-down

            */
       

        var lastDirection = enemy.lastDirection;
        //if enemy moves right START here COPY>>>>>
        if (enemy.direction == "right" && (labyrinth[enemy.y][enemy.x + 1] == '\u2588') && (labyrinth[enemy.y - 1][enemy.x] == '\u2588'))
        {
            enemy.direction = "down";
            enemy.lastDirection = "right";
        }

        if (enemy.direction == "right" && (labyrinth[enemy.y][enemy.x + 1] == '\u2588') && (labyrinth[enemy.y + 1][enemy.x] == '\u2588'))
        {
            enemy.direction = "up";
            enemy.lastDirection = "right";
        }

        if (enemy.direction == "right" && (labyrinth[enemy.y][enemy.x + 1] == '\u2588') && (labyrinth[enemy.y - 1][enemy.x] == '\u2588') && (labyrinth[enemy.y + 1][enemy.x] == '\u2588'))
        {
            enemy.direction = "left";
            enemy.lastDirection = "right";
        }

        if (enemy.direction == "right" && (labyrinth[enemy.y][enemy.x + 1] == '\u2588') && (labyrinth[enemy.y - 1][enemy.x] == ' ') && (labyrinth[enemy.y + 1][enemy.x] == ' '))
        {
            //randomizer for enemy movements.
            Random randomDirection = new Random();
            int enemyDirection = randomDirection.Next(2);
            if (enemyDirection == 0)
            {
                enemy.direction = "up";
            }
            else if (enemyDirection == 1)
            {
                enemy.direction = "down";
            }
            enemy.lastDirection = "right";
        }

        if (enemy.direction == "right" && (labyrinth[enemy.y][enemy.x + 1] == ' ') && (labyrinth[enemy.y + 1][enemy.x] == '\u2588') && (labyrinth[enemy.y][enemy.x - 1] == ' ') && (labyrinth[enemy.y - 1][enemy.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemyDirection = randomDirection.Next(9);
            if (enemyDirection % 8 == 0)
            {
                enemy.direction = "up";
            }
            else if (enemyDirection % 8 != 0)
            {
                enemy.direction = "right";
            }
            enemy.lastDirection = "right";
        }

        if (enemy.direction == "right" && (labyrinth[enemy.y][enemy.x + 1] == ' ') && (labyrinth[enemy.y - 1][enemy.x] == '\u2588') && (labyrinth[enemy.y][enemy.x - 1] == ' ') && (labyrinth[enemy.y + 1][enemy.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemyDirection = randomDirection.Next(9);
            if (enemyDirection % 8 == 0)
            {
                enemy.direction = "down";
            }
            else if (enemyDirection % 8 != 0)
            {
                enemy.direction = "right";
            }
            enemy.lastDirection = "right";
        }
        //if enemy moves right END here

        //if enemy moves down START here
        if (enemy.direction == "down" && (labyrinth[enemy.y][enemy.x + 1] == '\u2588') && (labyrinth[enemy.y + 1][enemy.x] == '\u2588'))
        {
            enemy.direction = "left";
            enemy.lastDirection = "down";
        }

        if (enemy.direction == "down" && (labyrinth[enemy.y][enemy.x - 1] == '\u2588') && (labyrinth[enemy.y + 1][enemy.x] == '\u2588'))
        {
            enemy.direction = "right";
            enemy.lastDirection = "down";
        }

        if (enemy.direction == "down" && (labyrinth[enemy.y][enemy.x - 1] == '\u2588') && (labyrinth[enemy.y + 1][enemy.x] == '\u2588') && (labyrinth[enemy.y][enemy.x + 1] == '\u2588'))
        {
            enemy.direction = "up";
            enemy.lastDirection = "down";
        }

        if (enemy.direction == "down" && (labyrinth[enemy.y][enemy.x + 1] == ' ') && (labyrinth[enemy.y + 1][enemy.x] == '\u2588') && (labyrinth[enemy.y][enemy.x - 1] == ' '))
        {
            Random randomDirection = new Random();
            int enemyDirection = randomDirection.Next(2);
            if (enemyDirection == 0)
            {
                enemy.direction = "right";
            }
            else if (enemyDirection == 1)
            {
                enemy.direction = "left";
            }
            enemy.lastDirection = "down";
        }

        if (enemy.direction == "down" && (labyrinth[enemy.y][enemy.x + 1] == '\u2588') && (labyrinth[enemy.y][enemy.x - 1] == ' ') && (labyrinth[enemy.y - 1][enemy.x] == ' ') && (labyrinth[enemy.y + 1][enemy.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemyDirection = randomDirection.Next(9);
            if (enemyDirection % 8 == 0)
            {
                enemy.direction = "left";
            }
            else if (enemyDirection % 8 != 0)
            {
                enemy.direction = "down";
            }
            enemy.lastDirection = "down";
        }

        if (enemy.direction == "down" && (labyrinth[enemy.y][enemy.x + 1] == ' ') && (labyrinth[enemy.y][enemy.x - 1] == '\u2588') && (labyrinth[enemy.y - 1][enemy.x] == ' ') && (labyrinth[enemy.y + 1][enemy.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemyDirection = randomDirection.Next(9);
            if (enemyDirection % 8 == 0)
            {
                enemy.direction = "right";
            }
            else if (enemyDirection % 8 != 0)
            {
                enemy.direction = "down";
            }
            enemy.lastDirection = "down";
        }		
        //if enemy moves down END here
        //if enemy moves left START here
        if (enemy.direction == "left" && (labyrinth[enemy.y][enemy.x - 1] == '\u2588') && (labyrinth[enemy.y + 1][enemy.x] == '\u2588'))
        {
            enemy.direction = "up";
            enemy.lastDirection = "left";
        }

        if (enemy.direction == "left" && (labyrinth[enemy.y][enemy.x - 1] == '\u2588') && (labyrinth[enemy.y - 1][enemy.x] == '\u2588'))
        {
            enemy.direction = "down";
            enemy.lastDirection = "left";
        }
         
        if (enemy.direction == "left" && (labyrinth[enemy.y][enemy.x - 1] == '\u2588') && (labyrinth[enemy.y + 1][enemy.x] == '\u2588') && (labyrinth[enemy.y - 1][enemy.x] == '\u2588'))
        {
            enemy.direction = "right";
            enemy.lastDirection = "left";
        }

        if (enemy.direction == "left" && (labyrinth[enemy.y][enemy.x - 1] == '\u2588') && (labyrinth[enemy.y + 1][enemy.x] == ' ') && (labyrinth[enemy.y - 1][enemy.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemyDirection = randomDirection.Next(2);
            if (enemyDirection == 0)
            {
                enemy.direction = "up";
            }
            else if (enemyDirection == 1)
            {
                enemy.direction = "down";
            }
            enemy.lastDirection = "left";
        }

        if (enemy.direction == "left" && (labyrinth[enemy.y - 1][enemy.x] == '\u2588') && (labyrinth[enemy.y + 1][enemy.x] == ' ') && (labyrinth[enemy.y][enemy.x - 1] == ' ') && (labyrinth[enemy.y][enemy.x + 1] == ' '))
        {
            Random randomDirection = new Random();
            int enemyDirection = randomDirection.Next(9);
            if (enemyDirection % 8 == 0)
            {
                enemy.direction = "down";
            }
            else if (enemyDirection % 8 != 0)
            {
                enemy.direction = "left";
            }
            enemy.lastDirection = "left";
        }

        if (enemy.direction == "left" && (labyrinth[enemy.y - 1][enemy.x] == ' ') && (labyrinth[enemy.y + 1][enemy.x] == '\u2588') && (labyrinth[enemy.y][enemy.x - 1] == ' ') && (labyrinth[enemy.y][enemy.x + 1] == ' '))
        {
            Random randomDirection = new Random();
            int enemyDirection = randomDirection.Next(9);
            if (enemyDirection % 8 == 0)
            {
                enemy.direction = "up";
            }
            else if (enemyDirection % 8 != 0)
            {
                enemy.direction = "left";
            }
            enemy.lastDirection = "left";
        }
        //if enemy moves left END here

        //if enemy moves up START here
        if (enemy.direction == "up" && (labyrinth[enemy.y][enemy.x - 1] == '\u2588') && (labyrinth[enemy.y - 1][enemy.x] == '\u2588'))
        {
            enemy.direction = "right";
            enemy.lastDirection = "up";
        }

        if (enemy.direction == "up" && (labyrinth[enemy.y][enemy.x + 1] == '\u2588') && (labyrinth[enemy.y - 1][enemy.x] == '\u2588'))
        {
            enemy.direction = "left";
            enemy.lastDirection = "up";
        }

        if (enemy.direction == "up" && (labyrinth[enemy.y][enemy.x - 1] == '\u2588') && (labyrinth[enemy.y - 1][enemy.x] == '\u2588') && (labyrinth[enemy.y][enemy.x + 1] == '\u2588'))
        {
            enemy.direction = "down";
            enemy.lastDirection = "up";
        }

        if (enemy.direction == "up" && (labyrinth[enemy.y][enemy.x - 1] == ' ') && (labyrinth[enemy.y - 1][enemy.x] == '\u2588') && (labyrinth[enemy.y][enemy.x + 1] == ' '))
        {
            Random randomDirection = new Random();
            int enemyDirection = randomDirection.Next(2);
            if (enemyDirection == 0)
            {
                enemy.direction = "left";
            }
            else if (enemyDirection == 1)
            {
                enemy.direction = "right";
            }
            enemy.lastDirection = "up";
        }

        if (enemy.direction == "up" && (labyrinth[enemy.y][enemy.x - 1] == ' ') && (labyrinth[enemy.y][enemy.x + 1] == '\u2588') && (labyrinth[enemy.y + 1][enemy.x] == ' ') && (labyrinth[enemy.y - 1][enemy.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemyDirection = randomDirection.Next(9);
            if (enemyDirection % 8 == 0)
            {
                enemy.direction = "left";
            }
            else if (enemyDirection % 8 != 0)
            {
                enemy.direction = "up";
            }
            enemy.lastDirection = "up";
        }

        if (enemy.direction == "up" && (labyrinth[enemy.y][enemy.x - 1] == '\u2588') && (labyrinth[enemy.y][enemy.x + 1] == ' ') && (labyrinth[enemy.y + 1][enemy.x] == ' ') && (labyrinth[enemy.y - 1][enemy.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemyDirection = randomDirection.Next(9);
            if (enemyDirection % 8 == 0)
            {
                enemy.direction = "right";
            }
            else if (enemyDirection % 8 != 0)
            {
                enemy.direction = "up";
            }
            enemy.lastDirection = "up";
        }
        //if enemy moves left Up here COPY>>>>




        if (enemy.direction == "right")
        {
            enemy.x += 1;
        }
        if (enemy.direction == "down")
        {
            enemy.y += 1;
        }
        if (enemy.direction == "left")
        {
            enemy.x -= 1;
        }
        if (enemy.direction == "up")
        {
            enemy.y -= 1;
        }
    }

    private static void MoveEnemy2()
    {
        /*Move enemy2
            1-right
            2-left
            3-up
            4-down

            */
        var lastDirection = enemy2.lastDirection;
        //if enemy2 moves right START here   COPY>>>>
        if (enemy2.direction == "right" && (labyrinth[enemy2.y][enemy2.x + 1] == '\u2588') && (labyrinth[enemy2.y - 1][enemy2.x] == '\u2588'))
        {
            enemy2.direction = "down";
            enemy2.lastDirection = "right";
        }

        if (enemy2.direction == "right" && (labyrinth[enemy2.y][enemy2.x + 1] == '\u2588') && (labyrinth[enemy2.y + 1][enemy2.x] == '\u2588'))
        {
            enemy2.direction = "up";
            enemy2.lastDirection = "right";
        }

        if (enemy2.direction == "right" && (labyrinth[enemy2.y][enemy2.x + 1] == '\u2588') && (labyrinth[enemy2.y - 1][enemy2.x] == '\u2588') && (labyrinth[enemy2.y + 1][enemy2.x] == '\u2588'))
        {
            enemy2.direction = "left";
            enemy2.lastDirection = "right";
        }

        if (enemy2.direction == "right" && (labyrinth[enemy2.y][enemy2.x + 1] == '\u2588') && (labyrinth[enemy2.y - 1][enemy2.x] == ' ') && (labyrinth[enemy2.y + 1][enemy2.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy2Direction = randomDirection.Next(2);
            if (enemy2Direction == 0)
            {
                enemy2.direction = "up";
            }
            else if (enemy2Direction == 1)
            {
                enemy2.direction = "down";
            }
            enemy2.lastDirection = "right";
        }

        if (enemy2.direction == "right" && (labyrinth[enemy2.y][enemy2.x + 1] == ' ') && (labyrinth[enemy2.y + 1][enemy2.x] == '\u2588') && (labyrinth[enemy2.y][enemy2.x - 1] == ' ') && (labyrinth[enemy2.y - 1][enemy2.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy2Direction = randomDirection.Next(9);
            if (enemy2Direction % 8 == 0)
            {
                enemy2.direction = "up";
            }
            else if (enemy2Direction % 8 != 0)
            {
                enemy2.direction = "right";
            }
            enemy2.lastDirection = "right";
        }

        if (enemy2.direction == "right" && (labyrinth[enemy2.y][enemy2.x + 1] == ' ') && (labyrinth[enemy2.y - 1][enemy2.x] == '\u2588') && (labyrinth[enemy2.y][enemy2.x - 1] == ' ') && (labyrinth[enemy2.y + 1][enemy2.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy2Direction = randomDirection.Next(9);
            if (enemy2Direction % 8 == 0)
            {
                enemy2.direction = "down";
            }
            else if (enemy2Direction % 8 != 0)
            {
                enemy2.direction = "right";
            }
            enemy2.lastDirection = "right";
        }
        //if enemy2 moves right END here

        //if enemy2 moves down START here
        if (enemy2.direction == "down" && (labyrinth[enemy2.y][enemy2.x + 1] == '\u2588') && (labyrinth[enemy2.y + 1][enemy2.x] == '\u2588'))
        {
            enemy2.direction = "left";
            enemy2.lastDirection = "down";
        }

        if (enemy2.direction == "down" && (labyrinth[enemy2.y][enemy2.x - 1] == '\u2588') && (labyrinth[enemy2.y + 1][enemy2.x] == '\u2588'))
        {
            enemy2.direction = "right";
            enemy2.lastDirection = "down";
        }

        if (enemy2.direction == "down" && (labyrinth[enemy2.y][enemy2.x - 1] == '\u2588') && (labyrinth[enemy2.y + 1][enemy2.x] == '\u2588') && (labyrinth[enemy2.y][enemy2.x + 1] == '\u2588'))
        {
            enemy2.direction = "up";
            enemy2.lastDirection = "down";
        }

        if (enemy2.direction == "down" && (labyrinth[enemy2.y][enemy2.x + 1] == ' ') && (labyrinth[enemy2.y + 1][enemy2.x] == '\u2588') && (labyrinth[enemy2.y][enemy2.x - 1] == ' '))
        {
            Random randomDirection = new Random();
            int enemy2Direction = randomDirection.Next(2);
            if (enemy2Direction == 0)
            {
                enemy2.direction = "right";
            }
            else if (enemy2Direction == 1)
            {
                enemy2.direction = "left";
            }
            enemy2.lastDirection = "down";
        }

        if (enemy2.direction == "down" && (labyrinth[enemy2.y][enemy2.x + 1] == '\u2588') && (labyrinth[enemy2.y][enemy2.x - 1] == ' ') && (labyrinth[enemy2.y - 1][enemy2.x] == ' ') && (labyrinth[enemy2.y + 1][enemy2.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy2Direction = randomDirection.Next(9);
            if (enemy2Direction % 8 == 0)
            {
                enemy2.direction = "left";
            }
            else if (enemy2Direction % 8 != 0)
            {
                enemy2.direction = "down";
            }
            enemy2.lastDirection = "down";
        }

        if (enemy2.direction == "down" && (labyrinth[enemy2.y][enemy2.x + 1] == ' ') && (labyrinth[enemy2.y][enemy2.x - 1] == '\u2588') && (labyrinth[enemy2.y - 1][enemy2.x] == ' ') && (labyrinth[enemy2.y + 1][enemy2.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy2Direction = randomDirection.Next(9);
            if (enemy2Direction % 8 == 0)
            {
                enemy2.direction = "right";
            }
            else if (enemy2Direction % 8 != 0)
            {
                enemy2.direction = "down";
            }
            enemy2.lastDirection = "down";
        }		
        //if enemy2 moves down END here
        //if enemy2 moves left START here
        if (enemy2.direction == "left" && (labyrinth[enemy2.y][enemy2.x - 1] == '\u2588') && (labyrinth[enemy2.y + 1][enemy2.x] == '\u2588'))
        {
            enemy2.direction = "up";
            enemy2.lastDirection = "left";
        }

        if (enemy2.direction == "left" && (labyrinth[enemy2.y][enemy2.x - 1] == '\u2588') && (labyrinth[enemy2.y - 1][enemy2.x] == '\u2588'))
        {
            enemy2.direction = "down";
            enemy2.lastDirection = "left";
        }

        if (enemy2.direction == "left" && (labyrinth[enemy2.y][enemy2.x - 1] == '\u2588') && (labyrinth[enemy2.y + 1][enemy2.x] == '\u2588') && (labyrinth[enemy2.y - 1][enemy2.x] == '\u2588'))
        {
            enemy2.direction = "right";
            enemy2.lastDirection = "left";
        }

        if (enemy2.direction == "left" && (labyrinth[enemy2.y][enemy2.x - 1] == '\u2588') && (labyrinth[enemy2.y + 1][enemy2.x] == ' ') && (labyrinth[enemy2.y - 1][enemy2.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy2Direction = randomDirection.Next(2);
            if (enemy2Direction == 0)
            {
                enemy2.direction = "up";
            }
            else if (enemy2Direction == 1)
            {
                enemy2.direction = "down";
            }
            enemy2.lastDirection = "left";
        }

        if (enemy2.direction == "left" && (labyrinth[enemy2.y - 1][enemy2.x] == '\u2588') && (labyrinth[enemy2.y + 1][enemy2.x] == ' ') && (labyrinth[enemy2.y][enemy2.x - 1] == ' ') && (labyrinth[enemy2.y][enemy2.x + 1] == ' '))
        {
            Random randomDirection = new Random();
            int enemy2Direction = randomDirection.Next(9);
            if (enemy2Direction % 8 == 0)
            {
                enemy2.direction = "down";
            }
            else if (enemy2Direction % 8 != 0)
            {
                enemy2.direction = "left";
            }
            enemy2.lastDirection = "left";
        }

        if (enemy2.direction == "left" && (labyrinth[enemy2.y - 1][enemy2.x] == ' ') && (labyrinth[enemy2.y + 1][enemy2.x] == '\u2588') && (labyrinth[enemy2.y][enemy2.x - 1] == ' ') && (labyrinth[enemy2.y][enemy2.x + 1] == ' '))
        {
            Random randomDirection = new Random();
            int enemy2Direction = randomDirection.Next(9);
            if (enemy2Direction % 8 == 0)
            {
                enemy2.direction = "up";
            }
            else if (enemy2Direction % 8 != 0)
            {
                enemy2.direction = "left";
            }
            enemy2.lastDirection = "left";
        }
        //if enemy2 moves left END here

        //if enemy2 moves up START here
        if (enemy2.direction == "up" && (labyrinth[enemy2.y][enemy2.x - 1] == '\u2588') && (labyrinth[enemy2.y - 1][enemy2.x] == '\u2588'))
        {
            enemy2.direction = "right";
            enemy2.lastDirection = "up";
        }

        if (enemy2.direction == "up" && (labyrinth[enemy2.y][enemy2.x + 1] == '\u2588') && (labyrinth[enemy2.y - 1][enemy2.x] == '\u2588'))
        {
            enemy2.direction = "left";
            enemy2.lastDirection = "up";
        }

        if (enemy2.direction == "up" && (labyrinth[enemy2.y][enemy2.x - 1] == '\u2588') && (labyrinth[enemy2.y - 1][enemy2.x] == '\u2588') && (labyrinth[enemy2.y][enemy2.x + 1] == '\u2588'))
        {
            enemy2.direction = "down";
            enemy2.lastDirection = "up";
        }

        if (enemy2.direction == "up" && (labyrinth[enemy2.y][enemy2.x - 1] == ' ') && (labyrinth[enemy2.y - 1][enemy2.x] == '\u2588') && (labyrinth[enemy2.y][enemy2.x + 1] == ' '))
        {
            Random randomDirection = new Random();
            int enemy2Direction = randomDirection.Next(2);
            if (enemy2Direction == 0)
            {
                enemy2.direction = "left";
            }
            else if (enemy2Direction == 1)
            {
                enemy2.direction = "right";
            }
            enemy2.lastDirection = "up";
        }

        if (enemy2.direction == "up" && (labyrinth[enemy2.y][enemy2.x - 1] == ' ') && (labyrinth[enemy2.y][enemy2.x + 1] == '\u2588') && (labyrinth[enemy2.y + 1][enemy2.x] == ' ') && (labyrinth[enemy2.y - 1][enemy2.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy2Direction = randomDirection.Next(9);
            if (enemy2Direction % 8 == 0)
            {
                enemy2.direction = "left";
            }
            else if (enemy2Direction % 8 != 0)
            {
                enemy2.direction = "up";
            }
            enemy2.lastDirection = "up";
        }

        if (enemy2.direction == "up" && (labyrinth[enemy2.y][enemy2.x - 1] == '\u2588') && (labyrinth[enemy2.y][enemy2.x + 1] == ' ') && (labyrinth[enemy2.y + 1][enemy2.x] == ' ') && (labyrinth[enemy2.y - 1][enemy2.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy2Direction = randomDirection.Next(9);
            if (enemy2Direction % 8 == 0)
            {
                enemy2.direction = "right";
            }
            else if (enemy2Direction % 8 != 0)
            {
                enemy2.direction = "up";
            }
            enemy2.lastDirection = "up";
        }
        //if enemy2 moves Up END here COPY>>>>>




        if (enemy2.direction == "right")
        {
            enemy2.x += 1;
        }
        if (enemy2.direction == "down")
        {
            enemy2.y += 1;
        }
        if (enemy2.direction == "left")
        {
            enemy2.x -= 1;
        }
        if (enemy2.direction == "up")
        {
            enemy2.y -= 1;
        }
    }

    private static void MoveEnemy3()
    {

        /*Move enemy3
            1-right
            2-left
            3-up
            4-down

            */


        var lastDirection = enemy3.lastDirection;
        //if enemy3 moves right START here COPY>>>>>
        if (enemy3.direction == "right" && (labyrinth[enemy3.y][enemy3.x + 1] == '\u2588') && (labyrinth[enemy3.y - 1][enemy3.x] == '\u2588'))
        {
            enemy3.direction = "down";
            enemy3.lastDirection = "right";
        }

        if (enemy3.direction == "right" && (labyrinth[enemy3.y][enemy3.x + 1] == '\u2588') && (labyrinth[enemy3.y + 1][enemy3.x] == '\u2588'))
        {
            enemy3.direction = "up";
            enemy3.lastDirection = "right";
        }

        if (enemy3.direction == "right" && (labyrinth[enemy3.y][enemy3.x + 1] == '\u2588') && (labyrinth[enemy3.y - 1][enemy3.x] == '\u2588') && (labyrinth[enemy3.y + 1][enemy3.x] == '\u2588'))
        {
            enemy3.direction = "left";
            enemy3.lastDirection = "right";
        }

        if (enemy3.direction == "right" && (labyrinth[enemy3.y][enemy3.x + 1] == '\u2588') && (labyrinth[enemy3.y - 1][enemy3.x] == ' ') && (labyrinth[enemy3.y + 1][enemy3.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy3Direction = randomDirection.Next(2);
            if (enemy3Direction == 0)
            {
                enemy3.direction = "up";
            }
            else if (enemy3Direction == 1)
            {
                enemy3.direction = "down";
            }
            enemy3.lastDirection = "right";
        }

        if (enemy3.direction == "right" && (labyrinth[enemy3.y][enemy3.x + 1] == ' ') && (labyrinth[enemy3.y + 1][enemy3.x] == '\u2588') && (labyrinth[enemy3.y][enemy3.x - 1] == ' ') && (labyrinth[enemy3.y - 1][enemy3.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy3Direction = randomDirection.Next(9);
            if (enemy3Direction % 8 == 0)
            {
                enemy3.direction = "up";
            }
            else if (enemy3Direction % 8 != 0)
            {
                enemy3.direction = "right";
            }
            enemy3.lastDirection = "right";
        }

        if (enemy3.direction == "right" && (labyrinth[enemy3.y][enemy3.x + 1] == ' ') && (labyrinth[enemy3.y - 1][enemy3.x] == '\u2588') && (labyrinth[enemy3.y][enemy3.x - 1] == ' ') && (labyrinth[enemy3.y + 1][enemy3.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy3Direction = randomDirection.Next(9);
            if (enemy3Direction % 8 == 0)
            {
                enemy3.direction = "down";
            }
            else if (enemy3Direction % 8 != 0)
            {
                enemy3.direction = "right";
            }
            enemy3.lastDirection = "right";
        }
        //if enemy3 moves right END here

        //if enemy3 moves down START here
        if (enemy3.direction == "down" && (labyrinth[enemy3.y][enemy3.x + 1] == '\u2588') && (labyrinth[enemy3.y + 1][enemy3.x] == '\u2588'))
        {
            enemy3.direction = "left";
            enemy3.lastDirection = "down";
        }

        if (enemy3.direction == "down" && (labyrinth[enemy3.y][enemy3.x - 1] == '\u2588') && (labyrinth[enemy3.y + 1][enemy3.x] == '\u2588'))
        {
            enemy3.direction = "right";
            enemy3.lastDirection = "down";
        }

        if (enemy3.direction == "down" && (labyrinth[enemy3.y][enemy3.x - 1] == '\u2588') && (labyrinth[enemy3.y + 1][enemy3.x] == '\u2588') && (labyrinth[enemy3.y][enemy3.x + 1] == '\u2588'))
        {
            enemy3.direction = "up";
            enemy3.lastDirection = "down";
        }

        if (enemy3.direction == "down" && (labyrinth[enemy3.y][enemy3.x + 1] == ' ') && (labyrinth[enemy3.y + 1][enemy3.x] == '\u2588') && (labyrinth[enemy3.y][enemy3.x - 1] == ' '))
        {
            Random randomDirection = new Random();
            int enemy3Direction = randomDirection.Next(2);
            if (enemy3Direction == 0)
            {
                enemy3.direction = "right";
            }
            else if (enemy3Direction == 1)
            {
                enemy3.direction = "left";
            }
            enemy3.lastDirection = "down";
        }

        if (enemy3.direction == "down" && (labyrinth[enemy3.y][enemy3.x + 1] == '\u2588') && (labyrinth[enemy3.y][enemy3.x - 1] == ' ') && (labyrinth[enemy3.y - 1][enemy3.x] == ' ') && (labyrinth[enemy3.y + 1][enemy3.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy3Direction = randomDirection.Next(9);
            if (enemy3Direction % 8 == 0)
            {
                enemy3.direction = "left";
            }
            else if (enemy3Direction % 8 != 0)
            {
                enemy3.direction = "down";
            }
            enemy3.lastDirection = "down";
        }

        if (enemy3.direction == "down" && (labyrinth[enemy3.y][enemy3.x + 1] == ' ') && (labyrinth[enemy3.y][enemy3.x - 1] == '\u2588') && (labyrinth[enemy3.y - 1][enemy3.x] == ' ') && (labyrinth[enemy3.y + 1][enemy3.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy3Direction = randomDirection.Next(9);
            if (enemy3Direction % 8 == 0)
            {
                enemy3.direction = "right";
            }
            else if (enemy3Direction % 8 != 0)
            {
                enemy3.direction = "down";
            }
            enemy3.lastDirection = "down";
        }		
        //if enemy3 moves down END here
        //if enemy3 moves left START here
        if (enemy3.direction == "left" && (labyrinth[enemy3.y][enemy3.x - 1] == '\u2588') && (labyrinth[enemy3.y + 1][enemy3.x] == '\u2588'))
        {
            enemy3.direction = "up";
            enemy3.lastDirection = "left";
        }

        if (enemy3.direction == "left" && (labyrinth[enemy3.y][enemy3.x - 1] == '\u2588') && (labyrinth[enemy3.y - 1][enemy3.x] == '\u2588'))
        {
            enemy3.direction = "down";
            enemy3.lastDirection = "left";
        }

        if (enemy3.direction == "left" && (labyrinth[enemy3.y][enemy3.x - 1] == '\u2588') && (labyrinth[enemy3.y + 1][enemy3.x] == '\u2588') && (labyrinth[enemy3.y - 1][enemy3.x] == '\u2588'))
        {
            enemy3.direction = "right";
            enemy3.lastDirection = "left";
        }

        if (enemy3.direction == "left" && (labyrinth[enemy3.y][enemy3.x - 1] == '\u2588') && (labyrinth[enemy3.y + 1][enemy3.x] == ' ') && (labyrinth[enemy3.y - 1][enemy3.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy3Direction = randomDirection.Next(2);
            if (enemy3Direction == 0)
            {
                enemy3.direction = "up";
            }
            else if (enemy3Direction == 1)
            {
                enemy3.direction = "down";
            }
            enemy3.lastDirection = "left";
        }

        if (enemy3.direction == "left" && (labyrinth[enemy3.y - 1][enemy3.x] == '\u2588') && (labyrinth[enemy3.y + 1][enemy3.x] == ' ') && (labyrinth[enemy3.y][enemy3.x - 1] == ' ') && (labyrinth[enemy3.y][enemy3.x + 1] == ' '))
        {
            Random randomDirection = new Random();
            int enemy3Direction = randomDirection.Next(9);
            if (enemy3Direction % 8 == 0)
            {
                enemy3.direction = "down";
            }
            else if (enemy3Direction % 8 != 0)
            {
                enemy3.direction = "left";
            }
            enemy3.lastDirection = "left";
        }

        if (enemy3.direction == "left" && (labyrinth[enemy3.y - 1][enemy3.x] == ' ') && (labyrinth[enemy3.y + 1][enemy3.x] == '\u2588') && (labyrinth[enemy3.y][enemy3.x - 1] == ' ') && (labyrinth[enemy3.y][enemy3.x + 1] == ' '))
        {
            Random randomDirection = new Random();
            int enemy3Direction = randomDirection.Next(9);
            if (enemy3Direction % 8 == 0)
            {
                enemy3.direction = "up";
            }
            else if (enemy3Direction % 8 != 0)
            {
                enemy3.direction = "left";
            }
            enemy3.lastDirection = "left";
        }
        //if enemy3 moves left END here

        //if enemy3 moves up START here
        if (enemy3.direction == "up" && (labyrinth[enemy3.y][enemy3.x - 1] == '\u2588') && (labyrinth[enemy3.y - 1][enemy3.x] == '\u2588'))
        {
            enemy3.direction = "right";
            enemy3.lastDirection = "up";
        }

        if (enemy3.direction == "up" && (labyrinth[enemy3.y][enemy3.x + 1] == '\u2588') && (labyrinth[enemy3.y - 1][enemy3.x] == '\u2588'))
        {
            enemy3.direction = "left";
            enemy3.lastDirection = "up";
        }

        if (enemy3.direction == "up" && (labyrinth[enemy3.y][enemy3.x - 1] == '\u2588') && (labyrinth[enemy3.y - 1][enemy3.x] == '\u2588') && (labyrinth[enemy3.y][enemy3.x + 1] == '\u2588'))
        {
            enemy3.direction = "down";
            enemy3.lastDirection = "up";
        }

        if (enemy3.direction == "up" && (labyrinth[enemy3.y][enemy3.x - 1] == ' ') && (labyrinth[enemy3.y - 1][enemy3.x] == '\u2588') && (labyrinth[enemy3.y][enemy3.x + 1] == ' '))
        {
            Random randomDirection = new Random();
            int enemy3Direction = randomDirection.Next(2);
            if (enemy3Direction == 0)
            {
                enemy3.direction = "left";
            }
            else if (enemy3Direction == 1)
            {
                enemy3.direction = "right";
            }
            enemy3.lastDirection = "up";
        }

        if (enemy3.direction == "up" && (labyrinth[enemy3.y][enemy3.x - 1] == ' ') && (labyrinth[enemy3.y][enemy3.x + 1] == '\u2588') && (labyrinth[enemy3.y + 1][enemy3.x] == ' ') && (labyrinth[enemy3.y - 1][enemy3.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy3Direction = randomDirection.Next(9);
            if (enemy3Direction % 8 == 0)
            {
                enemy3.direction = "left";
            }
            else if (enemy3Direction % 8 != 0)
            {
                enemy3.direction = "up";
            }
            enemy3.lastDirection = "up";
        }

        if (enemy3.direction == "up" && (labyrinth[enemy3.y][enemy3.x - 1] == '\u2588') && (labyrinth[enemy3.y][enemy3.x + 1] == ' ') && (labyrinth[enemy3.y + 1][enemy3.x] == ' ') && (labyrinth[enemy3.y - 1][enemy3.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy3Direction = randomDirection.Next(9);
            if (enemy3Direction % 8 == 0)
            {
                enemy3.direction = "right";
            }
            else if (enemy3Direction % 8 != 0)
            {
                enemy3.direction = "up";
            }
            enemy3.lastDirection = "up";
        }
        //if enemy3 moves Up END here COPY>>>>




        if (enemy3.direction == "right")
        {
            enemy3.x += 1;
        }
        if (enemy3.direction == "down")
        {
            enemy3.y += 1;
        }
        if (enemy3.direction == "left")
        {
            enemy3.x -= 1;
        }
        if (enemy3.direction == "up")
        {
            enemy3.y -= 1;
        }
    }


    private static void MoveEnemy4()
    {
        /*Move enemy4
            1-right
            2-left
            3-up
            4-down

            */
        var lastDirection = enemy4.lastDirection;
        //if enemy4 moves right START here   COPY>>>>
        if (enemy4.direction == "right" && (labyrinth[enemy4.y][enemy4.x + 1] == '\u2588') && (labyrinth[enemy4.y - 1][enemy4.x] == '\u2588'))
        {
            enemy4.direction = "down";
            enemy4.lastDirection = "right";
        }

        if (enemy4.direction == "right" && (labyrinth[enemy4.y][enemy4.x + 1] == '\u2588') && (labyrinth[enemy4.y + 1][enemy4.x] == '\u2588'))
        {
            enemy4.direction = "up";
            enemy4.lastDirection = "right";
        }

        if (enemy4.direction == "right" && (labyrinth[enemy4.y][enemy4.x + 1] == '\u2588') && (labyrinth[enemy4.y - 1][enemy4.x] == '\u2588') && (labyrinth[enemy4.y + 1][enemy4.x] == '\u2588'))
        {
            enemy4.direction = "left";
            enemy4.lastDirection = "right";
        }

        if (enemy4.direction == "right" && (labyrinth[enemy4.y][enemy4.x + 1] == '\u2588') && (labyrinth[enemy4.y - 1][enemy4.x] == ' ') && (labyrinth[enemy4.y + 1][enemy4.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy4Direction = randomDirection.Next(2);
            if (enemy4Direction == 0)
            {
                enemy4.direction = "up";
            }
            else if (enemy4Direction == 1)
            {
                enemy4.direction = "down";
            }
            enemy4.lastDirection = "right";
        }

        if (enemy4.direction == "right" && (labyrinth[enemy4.y][enemy4.x + 1] == ' ') && (labyrinth[enemy4.y + 1][enemy4.x] == '\u2588') && (labyrinth[enemy4.y][enemy4.x - 1] == ' ') && (labyrinth[enemy4.y - 1][enemy4.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy4Direction = randomDirection.Next(9);
            if (enemy4Direction % 8 == 0)
            {
                enemy4.direction = "up";
            }
            else if (enemy4Direction % 8 != 0)
            {
                enemy4.direction = "right";
            }
            enemy4.lastDirection = "right";
        }

        if (enemy4.direction == "right" && (labyrinth[enemy4.y][enemy4.x + 1] == ' ') && (labyrinth[enemy4.y - 1][enemy4.x] == '\u2588') && (labyrinth[enemy4.y][enemy4.x - 1] == ' ') && (labyrinth[enemy4.y + 1][enemy4.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy4Direction = randomDirection.Next(9);
            if (enemy4Direction % 8 == 0)
            {
                enemy4.direction = "down";
            }
            else if (enemy4Direction % 8 != 0)
            {
                enemy4.direction = "right";
            }
            enemy4.lastDirection = "right";
        }
        //if enemy4 moves right END here

        //if enemy4 moves down START here
        if (enemy4.direction == "down" && (labyrinth[enemy4.y][enemy4.x + 1] == '\u2588') && (labyrinth[enemy4.y + 1][enemy4.x] == '\u2588'))
        {
            enemy4.direction = "left";
            enemy4.lastDirection = "down";
        }

        if (enemy4.direction == "down" && (labyrinth[enemy4.y][enemy4.x - 1] == '\u2588') && (labyrinth[enemy4.y + 1][enemy4.x] == '\u2588'))
        {
            enemy4.direction = "right";
            enemy4.lastDirection = "down";
        }

        if (enemy4.direction == "down" && (labyrinth[enemy4.y][enemy4.x - 1] == '\u2588') && (labyrinth[enemy4.y + 1][enemy4.x] == '\u2588') && (labyrinth[enemy4.y][enemy4.x + 1] == '\u2588'))
        {
            enemy4.direction = "up";
            enemy4.lastDirection = "down";
        }

        if (enemy4.direction == "down" && (labyrinth[enemy4.y][enemy4.x + 1] == ' ') && (labyrinth[enemy4.y + 1][enemy4.x] == '\u2588') && (labyrinth[enemy4.y][enemy4.x - 1] == ' '))
        {
            Random randomDirection = new Random();
            int enemy4Direction = randomDirection.Next(2);
            if (enemy4Direction == 0)
            {
                enemy4.direction = "right";
            }
            else if (enemy4Direction == 1)
            {
                enemy4.direction = "left";
            }
            enemy4.lastDirection = "down";
        }

        if (enemy4.direction == "down" && (labyrinth[enemy4.y][enemy4.x + 1] == '\u2588') && (labyrinth[enemy4.y][enemy4.x - 1] == ' ') && (labyrinth[enemy4.y - 1][enemy4.x] == ' ') && (labyrinth[enemy4.y + 1][enemy4.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy4Direction = randomDirection.Next(9);
            if (enemy4Direction % 8 == 0)
            {
                enemy4.direction = "left";
            }
            else if (enemy4Direction % 8 != 0)
            {
                enemy4.direction = "down";
            }
            enemy4.lastDirection = "down";
        }

        if (enemy4.direction == "down" && (labyrinth[enemy4.y][enemy4.x + 1] == ' ') && (labyrinth[enemy4.y][enemy4.x - 1] == '\u2588') && (labyrinth[enemy4.y - 1][enemy4.x] == ' ') && (labyrinth[enemy4.y + 1][enemy4.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy4Direction = randomDirection.Next(9);
            if (enemy4Direction % 8 == 0)
            {
                enemy4.direction = "right";
            }
            else if (enemy4Direction % 8 != 0)
            {
                enemy4.direction = "down";
            }
            enemy4.lastDirection = "down";
        }		
        //if enemy4 moves down END here
        //if enemy4 moves left START here
        if (enemy4.direction == "left" && (labyrinth[enemy4.y][enemy4.x - 1] == '\u2588') && (labyrinth[enemy4.y + 1][enemy4.x] == '\u2588'))
        {
            enemy4.direction = "up";
            enemy4.lastDirection = "left";
        }

        if (enemy4.direction == "left" && (labyrinth[enemy4.y][enemy4.x - 1] == '\u2588') && (labyrinth[enemy4.y - 1][enemy4.x] == '\u2588'))
        {
            enemy4.direction = "down";
            enemy4.lastDirection = "left";
        }

        if (enemy4.direction == "left" && (labyrinth[enemy4.y][enemy4.x - 1] == '\u2588') && (labyrinth[enemy4.y + 1][enemy4.x] == '\u2588') && (labyrinth[enemy4.y - 1][enemy4.x] == '\u2588'))
        {
            enemy4.direction = "right";
            enemy4.lastDirection = "left";
        }

        if (enemy4.direction == "left" && (labyrinth[enemy4.y][enemy4.x - 1] == '\u2588') && (labyrinth[enemy4.y + 1][enemy4.x] == ' ') && (labyrinth[enemy4.y - 1][enemy4.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy4Direction = randomDirection.Next(2);
            if (enemy4Direction == 0)
            {
                enemy4.direction = "up";
            }
            else if (enemy4Direction == 1)
            {
                enemy4.direction = "down";
            }
            enemy4.lastDirection = "left";
        }

        if (enemy4.direction == "left" && (labyrinth[enemy4.y - 1][enemy4.x] == '\u2588') && (labyrinth[enemy4.y + 1][enemy4.x] == ' ') && (labyrinth[enemy4.y][enemy4.x - 1] == ' ') && (labyrinth[enemy4.y][enemy4.x + 1] == ' '))
        {
            Random randomDirection = new Random();
            int enemy4Direction = randomDirection.Next(9);
            if (enemy4Direction % 8 == 0)
            {
                enemy4.direction = "down";
            }
            else if (enemy4Direction % 8 != 0)
            {
                enemy4.direction = "left";
            }
            enemy4.lastDirection = "left";
        }

        if (enemy4.direction == "left" && (labyrinth[enemy4.y - 1][enemy4.x] == ' ') && (labyrinth[enemy4.y + 1][enemy4.x] == '\u2588') && (labyrinth[enemy4.y][enemy4.x - 1] == ' ') && (labyrinth[enemy4.y][enemy4.x + 1] == ' '))
        {
            Random randomDirection = new Random();
            int enemy4Direction = randomDirection.Next(9);
            if (enemy4Direction % 8 == 0)
            {
                enemy4.direction = "up";
            }
            else if (enemy4Direction % 8 != 0)
            {
                enemy4.direction = "left";
            }
            enemy4.lastDirection = "left";
        }
        //if enemy4 moves left END here

        //if enemy4 moves up START here
        if (enemy4.direction == "up" && (labyrinth[enemy4.y][enemy4.x - 1] == '\u2588') && (labyrinth[enemy4.y - 1][enemy4.x] == '\u2588'))
        {
            enemy4.direction = "right";
            enemy4.lastDirection = "up";
        }

        if (enemy4.direction == "up" && (labyrinth[enemy4.y][enemy4.x + 1] == '\u2588') && (labyrinth[enemy4.y - 1][enemy4.x] == '\u2588'))
        {
            enemy4.direction = "left";
            enemy4.lastDirection = "up";
        }

        if (enemy4.direction == "up" && (labyrinth[enemy4.y][enemy4.x - 1] == '\u2588') && (labyrinth[enemy4.y - 1][enemy4.x] == '\u2588') && (labyrinth[enemy4.y][enemy4.x + 1] == '\u2588'))
        {
            enemy4.direction = "down";
            enemy4.lastDirection = "up";
        }

        if (enemy4.direction == "up" && (labyrinth[enemy4.y][enemy4.x - 1] == ' ') && (labyrinth[enemy4.y - 1][enemy4.x] == '\u2588') && (labyrinth[enemy4.y][enemy4.x + 1] == ' '))
        {
            Random randomDirection = new Random();
            int enemy4Direction = randomDirection.Next(2);
            if (enemy4Direction == 0)
            {
                enemy4.direction = "left";
            }
            else if (enemy4Direction == 1)
            {
                enemy4.direction = "right";
            }
            enemy4.lastDirection = "up";
        }

        if (enemy4.direction == "up" && (labyrinth[enemy4.y][enemy4.x - 1] == ' ') && (labyrinth[enemy4.y][enemy4.x + 1] == '\u2588') && (labyrinth[enemy4.y + 1][enemy4.x] == ' ') && (labyrinth[enemy4.y - 1][enemy4.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy4Direction = randomDirection.Next(9);
            if (enemy4Direction % 8 == 0)
            {
                enemy4.direction = "left";
            }
            else if (enemy4Direction % 8 != 0)
            {
                enemy4.direction = "up";
            }
            enemy4.lastDirection = "up";
        }

        if (enemy4.direction == "up" && (labyrinth[enemy4.y][enemy4.x - 1] == '\u2588') && (labyrinth[enemy4.y][enemy4.x + 1] == ' ') && (labyrinth[enemy4.y + 1][enemy4.x] == ' ') && (labyrinth[enemy4.y - 1][enemy4.x] == ' '))
        {
            Random randomDirection = new Random();
            int enemy4Direction = randomDirection.Next(9);
            if (enemy4Direction % 8 == 0)
            {
                enemy4.direction = "right";
            }
            else if (enemy4Direction % 8 != 0)
            {
                enemy4.direction = "up";
            }
            enemy4.lastDirection = "up";
        }
        //if enemy4 moves left END here COPY>>>>>




        if (enemy4.direction == "right")
        {
            enemy4.x += 1;
        }
        if (enemy4.direction == "down")
        {
            enemy4.y += 1;
        }
        if (enemy4.direction == "left")
        {
            enemy4.x -= 1;
        }
        if (enemy4.direction == "up")
        {
            enemy4.y -= 1;
        }
    }

    #endregion
    static string[,] ReadLevelsFromFile(int playfieldHeight, int playfieldWidth)
    {
        //Read all the levels from the file.

        //Create StreamReader for Levels.txt file.
        StreamReader streamReader = new StreamReader("Levels.txt");

        //Create list for all levels.
        List<string[]> listOfLevels = new List<string[]>();

        //String for the current line read from the file.
        string currentLine = "";

        //Array for the whole level, read from the file.
        string[] currentLevel = new string[playfieldHeight];

        int i = 0;
        bool reachedEndOfFile = false;

        while (true)
        {
            //Read line from the file.
            currentLine = streamReader.ReadLine();

            //If the read line is equal to new line the ReadLine() will return "". This means we have finished reading the current level and it needs to be added to the list.
            //If we have reached the end of the file we add the level to the list.
            if ((currentLine == "" || currentLine == null) && reachedEndOfFile != true)
            {
                listOfLevels.Add(currentLevel);
                currentLevel = new string[playfieldHeight];
                i = 0;
            }
            else
            {
                if (currentLine != null)
                {
                    //Add the current line to the currentLevel array.
                    currentLevel[i] = (string)currentLine.Clone();
                    i++;
                }
            }
            if (reachedEndOfFile)
            {
                //Break out of the cycle when we reach the end of the file and we have finished adding the levels to the list.
                break;
            }
            if (currentLine == null)
            {
                //Set the flag for end of file to true.
                reachedEndOfFile = true;
            }


        }

        int count = listOfLevels.Count;
        

        //String array for all the levels.
        string[,] levels = new string[count, playfieldWidth];

        //Add the levels from the list to the string array.
        for (int j = 0; j < count; j++)
        {
            for (int k = 0; k < playfieldWidth; k++)
            {
                
                levels[j, k] = listOfLevels[j][k];
            }
        }

        //Return the 2D string array with all levels.
        return levels;
    }
    
    static string[] selectLevel(string[,] allLevels, int levelNumber)
    {
        //Select the wanted level from the 2D array.

        //Count of the rows of the level.
        int rowsCount = allLevels.GetLength(1);

        //Count of the cols of the level.
        int colsCount = allLevels[0, 0].Length;

        //String array for the selected level.
        string[] selectedLevel = new string[rowsCount];

        //Add all rows to the selectedLevel array.
        for (int i = 0; i < rowsCount; i++)
        {
            selectedLevel[i] = allLevels[levelNumber, i];
        }

        //Return string array with the selected level.
        return selectedLevel;
    }

    private static void SetPlayfieldSize(out int playfieldHeight, out int playfieldWidth)
    {
        // set console size (screen resolution)
        Console.BufferHeight = Console.WindowHeight = 21;
        Console.BufferWidth = Console.WindowWidth = 40;

        // set playfield size
        playfieldHeight = Console.WindowHeight - 1;
        playfieldWidth = Console.WindowWidth - 20;
    }

    static void PrintElement(Element thisObject)
    {
        // print object of type Element
        Console.SetCursorPosition(thisObject.x, thisObject.y);
        Console.ForegroundColor = thisObject.colour;
        Console.Write(thisObject.skin);
    }

    static void PrintLabyrinth(string[] thisArray)
    {
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.White;
        for (int i = 0; i < thisArray.Length; i++)
        {
            Console.WriteLine(thisArray[i]);
        }
    }

    static void StartupScreen()
    {
        //Two-dimensional border array (smileyface)

        char[,] smileyFace = new char[Console.BufferHeight, Console.BufferWidth];

        for (int col = 0; col < smileyFace.GetLength(1); col++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            smileyFace[Console.BufferHeight - 1, col] = (char)9787;
            smileyFace[1, col] = (char)9787;
        }

        for (int row = 0; row < smileyFace.GetLength(0); row++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            smileyFace[row, 0] = (char)9787;
            smileyFace[row, Console.BufferWidth - 1] = (char)9787;
        }

        PrintSmileyArray(smileyFace); //Printing border (smileyface array) and Pac Man "Logo"
        PrintPacManLogo(); 

        int cursorPositionX = Console.BufferWidth / 7; //Set print positions for menu options ("New Game" and "Read Instructions")
        int cursorPositionY = Console.BufferHeight - 5;
        Console.SetCursorPosition(cursorPositionX, cursorPositionY);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(">>New Game   Read Instructions");

        int pressedKeyValue = 0;

        var someKey = Console.ReadKey(true).Key;

        if (pressedKeyValue == 0 && someKey == ConsoleKey.Enter) //The ">>" is initially set at New Game, 
        {                                                        //hence if {ENTER} is pressed game will start.
            Console.Beep();
            return;
        }
        else
        {
            bool check = pressedKeyValue != 0 || pressedKeyValue != 1;

            while (check)
            {
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.LeftArrow)
                {
                    pressedKeyValue = 0;
                    Console.Beep();

                }
                else if (key == ConsoleKey.RightArrow)
                {
                    pressedKeyValue = 1;
                    Console.Beep();
                }

                switch (pressedKeyValue)
                {
                    case 0: //Case 0 moves ">>" to "New Game"
                        Console.Clear();

                        PrintSmileyArray(smileyFace);

                        Console.ForegroundColor = ConsoleColor.Gray;
                        PrintPacManLogo();

                        Console.SetCursorPosition(cursorPositionX, cursorPositionY);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(">>New Game    Read Instructions");
                        break;

                    case 1: //Case 1 moves ">>" to "Read Instructions"
                        Console.Clear();

                        PrintSmileyArray(smileyFace);
                        
                        Console.ForegroundColor = ConsoleColor.Gray;
                        PrintPacManLogo();

                        Console.SetCursorPosition(cursorPositionX, cursorPositionY);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("New Game    >>Read Instructions");
                        break;
                }

                if (pressedKeyValue == 0 && key == ConsoleKey.Enter) //">>" will be at "New Game" and after 
                {                                                    //pressing {ENTER} game will start.
                    Console.Beep();
                    break;
                }

                if (pressedKeyValue == 1 && key == ConsoleKey.Enter) //">>" will be at "Read Instructions" and after
                {                                                    //presing {Enter} Instructions submenu will appear.
                    Console.Beep();
                    Console.Clear();                                                       
                    PrintSmileyArray(smileyFace);
                    PrintInstructions();
                    
                    if (key == ConsoleKey.Escape) //Goes back to main start menu
                    {
                        pressedKeyValue = 0;
                    }
                }
            }
        }
        
    }

    static void PrintSmileyArray(char[,] smileyFace)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        for (int row = 0; row < smileyFace.GetLength(0); row++)
        {
            for (int col = 0; col < smileyFace.GetLength(1); col++)
            {
                Console.Write(smileyFace[row, col]);
            }
        }
    }

    static void PrintPacManLogo()
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        string[] P = {"\u2588\u2588\u2588\u2588", 
                      "\u2588  \u2588",
                      "\u2588\u2588\u2588\u2588",
                      "\u2588",
                      "\u2588"
                     };
        string[] A = {" \u2588\u2588", 
                      "\u2588  \u2588",
                      "\u2588\u2588\u2588\u2588",
                      "\u2588  \u2588",
                      "\u2588  \u2588"
                     };
        string[] C = {"\u2588\u2588\u2588\u2588", 
                      "\u2588",
                      "\u2588",
                      "\u2588",
                      "\u2588\u2588\u2588\u2588"
                     };
        string[] M = {"\u2588   \u2588", 
                      "\u2588\u2588 \u2588\u2588",
                      "\u2588 \u2588 \u2588",
                      "\u2588   \u2588",
                      "\u2588   \u2588"
                     };
        string[] N = {"\u2588   \u2588", 
                      "\u2588\u2588  \u2588",
                      "\u2588 \u2588 \u2588",
                      "\u2588  \u2588\u2588",
                      "\u2588   \u2588"
                     };
        string[] three = {"\u2588\u2588\u2588\u2588\u2588", 
                          "    \u2588",
                          "\u2588\u2588\u2588\u2588\u2588",
                          "    \u2588",
                          "\u2588\u2588\u2588\u2588\u2588"
                     };
        string[] D = {"\u2588\u2588\u2588", 
                      "\u2588   \u2588",
                      "\u2588   \u2588",
                      "\u2588   \u2588",
                      "\u2588\u2588\u2588"
                     };
        
        for (int i = 0; i < P.Length; i++)
			{
              Console.SetCursorPosition(4, i + 4);
			  Console.Write(P[i]);
			}

        Console.SetCursorPosition(9, 7);
        for (int i = 0; i < P.Length; i++)
        {
            Console.SetCursorPosition(9, i + 4);
            Console.Write(A[i]);
        }
        Console.SetCursorPosition(14, 7);
        for (int i = 0; i < P.Length; i++)
        {
            Console.SetCursorPosition(14, i + 4);
            Console.Write(C[i]);
        }
        Console.SetCursorPosition(20, 7);
        for (int i = 0; i < P.Length; i++)
        {
            Console.SetCursorPosition(20, i + 4);
            Console.Write(M[i]);
        }
        Console.SetCursorPosition(26, 7);
        for (int i = 0; i < P.Length; i++)
        {
            Console.SetCursorPosition(26, i + 4);
            Console.Write(A[i]);
        }
        Console.SetCursorPosition(31, 7);
        for (int i = 0; i < P.Length; i++)
        {
            Console.SetCursorPosition(31, i + 4);
            Console.Write(N[i]);
        }

        Console.SetCursorPosition(14, 10);
        for (int i = 0; i < P.Length; i++)
        {
            Console.SetCursorPosition(14, i + 10);
            Console.Write(three[i]);
        }
        Console.SetCursorPosition(20, 10);
        for (int i = 0; i < P.Length; i++)
        {
            Console.SetCursorPosition(20, i + 10);
            Console.Write(D[i]);
        }

    }

    static void PrintInstructions()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(Console.BufferWidth / 3, 2);
        Console.WriteLine("INSTRUCTIONS:");
        Console.SetCursorPosition(1, 4);
        Console.WriteLine("Pacman, our hero, moves around the");
        Console.SetCursorPosition(1, 5);
        Console.WriteLine("labyrinth, collecting all of the gold");
        Console.SetCursorPosition(1, 6);
        Console.WriteLine("pieces and earning points. Once the");
        Console.SetCursorPosition(1, 7);
        Console.WriteLine("player has collected a certain amount"); 
        Console.SetCursorPosition(1, 8);
        Console.WriteLine("of points, a secret portal opens to");
        Console.SetCursorPosition(1, 9);
        Console.WriteLine("the next level. If Pacman is caught by");
        Console.SetCursorPosition(1, 10);
        Console.WriteLine("by the enemy, the player loses a life.");

        Console.SetCursorPosition(Console.BufferWidth / 7, Console.BufferHeight - 5);
        Console.WriteLine("Press Esc to go back...");
    }
}