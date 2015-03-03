using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Security;

struct Creature
{
    public int y;   // horizontal position
    public int x;   // vertical position
    public char skin; // type of game object is defined by ASCII char
    public ConsoleColor colour;
    // AI variables
    public string direction;
    public string lastDirection;

    // constructor
    public Creature(int x = 0, int y = 0, char skin = '?', ConsoleColor colour = ConsoleColor.Gray)
    {
        this.x = x;
        this.y = y;
        this.skin = skin;
        this.colour = colour;
        this.direction = "left";
        this.lastDirection = "right";
    }
}

class PacMan3DGame
{
#region Global variables

    const int playfieldHeight = 20;
    const int playfieldWidth = 20;

    static Creature pacMan = new Creature(playfieldHeight / 2, playfieldWidth / 2, (char)9787, ConsoleColor.Yellow);

    // enemies
    const int enemiesCount = 6;
    static Creature[] enemies = new Creature[enemiesCount];
    static long enemyEvenMoveCounter = 1;
    static Random randomizer = new Random();

    // labyrinths
    static string[] labyrinth;
    static string[,] allLevels = new string[levelsCount, playfieldWidth];
    static int levelNumber = 0;
    static int levelsCount = 4; 

    static bool isGameOver = false;

    // scoring
    static char[, ,] gold = new char[levelsCount, playfieldHeight, playfieldWidth];
    static int goldCount;
    static int score = 0;
    static int tempScore = 0;    
    static string heroName = "";

#endregion

    static void Main()
    {
        #region Initialization

        // set screen size  
        Console.WindowHeight = 21;
        Console.BufferHeight = Console.WindowHeight;
        Console.WindowWidth = 40;
        Console.BufferWidth = Console.WindowWidth;

        // load levels
        allLevels = ReadLevelsFromFile(playfieldHeight, playfieldWidth);
        labyrinth = selectLevel(allLevels, levelNumber);

        // enemy initializer
        for (int i = 0; i < enemiesCount; i++)
        {
            int x = (playfieldHeight - 3) / randomizer.Next(1, 5);
            int y = (playfieldWidth - 3) / randomizer.Next(1, 5);
            enemies[i] = new Creature(x, y, '\u2666', ConsoleColor.Red);

            // AI initializer
            int enemyStartDirection = randomizer.Next(1, 5);
            if (enemyStartDirection == 1)
            {
                enemies[i].direction = "right";
            }
            else if (enemyStartDirection == 2)
            {
                enemies[i].direction = "left";
            }
            else if (enemyStartDirection == 3)
            {
                enemies[i].direction = "up";
            }
            else if (enemyStartDirection == 4)
            {
                enemies[i].direction = "down";
            }
        }

        // gold initializer
        goldCount = randomizer.Next(10, 30);
        int counter = goldCount;
        while (counter != 0)
        {
            int goldRow = randomizer.Next(1, playfieldHeight - 2);
            int goldCol = randomizer.Next(1, playfieldWidth - 2);
            int goldLevel = randomizer.Next(0, levelsCount);

            if (allLevels[goldLevel, goldRow][goldCol] == ' ')
            {
                gold[goldLevel, goldRow, goldCol] = '$';
                counter--;
            }
        }

        #endregion

        StartupScreen();

        #region GameLoop

        while (true)
        {
            RenderEngine();

            PhysicsEngine();

            if (isGameOver)
                break;            
        }
               
        #endregion

        EndScreen();
    }

    private static void PhysicsEngine()
    {
        if (enemyEvenMoveCounter % 2 == 0)
        {
            MoveEnemies(enemies);
        }

        MovePacMan();

        CheckForImpact();
    }

    static void EndScreen()
    {
        Console.Clear();

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

        PrintSmileyArray(smileyFace);
        PrintPacManLogo();

        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("☻ ◄◄◄    GAME OVER     ►►►");
        Console.Write("☻ ◄◄◄ EMTER YOUR NAME  ►►►   ");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Red;
        heroName = Console.ReadLine();
        Console.ResetColor();
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("☻ ◄◄◄ {0,6}'s results ►►►     ", heroName); // 
        Console.Write("☻ ◄◄◄ Your hero earned ►►► ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("{0,3} Points  ", score);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("☻ ◄◄◄ Level reached    ►►► ", levelNumber + 1);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("{0,3} Level   ", levelNumber + 1);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("☻");
        Console.ResetColor();

        ResultsFileTxt(); // set the results in a txt file at the end of the game
    }

    static void CheckForImpact()
    {
        // check for enemy impact
        for (int i = 0; i < enemiesCount; i++)
        {
            if ((enemies[i].y == pacMan.y && enemies[i].x == pacMan.x) || goldCount == 0)
            {
                //Console.WriteLine("Game Over !");
                isGameOver = true;
            }
        }

        enemyEvenMoveCounter++;

        // check for portals
        if (labyrinth[pacMan.x][pacMan.y] == '\u0040')
        {
            levelNumber++;
            pacMan.x -= playfieldWidth - 2;
            pacMan.y -= 12;
            //The array labyrinth is equal to the current level we select.
            labyrinth = selectLevel(allLevels, levelNumber);
        }

        if (labyrinth[pacMan.x][pacMan.y] == '\u0023')
        {
            levelNumber--;
            pacMan.x += playfieldWidth - 2;
            pacMan.y += 12;
            //The array labyrinth is equal to the current level we select.
            labyrinth = selectLevel(allLevels, levelNumber);
        }

        // check for gold and bonuses
        if (gold[levelNumber, pacMan.x, pacMan.y] == '$')
        {
            gold[levelNumber, pacMan.x, pacMan.y] = ' ';
            score += 5; // + 5 points per each bonus
            goldCount--;
        }
    }

    static void RenderEngine()
    {
        Console.Clear();    // fast screen clear
        PrintLabyrinth(labyrinth);

        PrintElement(pacMan);

        // print all enemies
        for (int i = 0; i < enemiesCount; i++)
        {
            PrintElement(enemies[i]);
        }

        // print gold
        for (int i = 0; i < playfieldHeight; i++)
        {
            for (int j = 0; j < playfieldWidth; j++)
            {
                if (gold[levelNumber, i, j] == '$')
                {
                    Console.SetCursorPosition(j, i);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("$");
                }
            }
        }

        PrintMenu();

        #region Framerate control
        if (levelNumber == 0)
        {
            Thread.Sleep(150);
        }
        else if (levelNumber == 1)
        {
            Thread.Sleep(140);
        }
        else if (levelNumber == 2)
        {
            Thread.Sleep(130);
        }
        else if (levelNumber == 3)
        {
            Thread.Sleep(110);
        }
        else if (levelNumber == 4)
        {
            Thread.Sleep(90);
        }
        else
        {
            Thread.Sleep(150); //default case
        }
        #endregion
    }

    static void MovePacMan()
    {
        while (Console.KeyAvailable)
        {

            // we assign the pressed key value to a variable pressedKey
            ConsoleKeyInfo pressedKey = Console.ReadKey(true);
            // next we start checking the value of the pressed key and take action if neccessary
            if (pressedKey.Key == ConsoleKey.LeftArrow) // if left arrow is pressed then
            {
                if (labyrinth[pacMan.x][pacMan.y - 1] != '\u2588')
                {
                    pacMan.y = pacMan.y - 1;
                }
            }
            else if (pressedKey.Key == ConsoleKey.RightArrow)
            {
                if (labyrinth[pacMan.x][pacMan.y + 1] != '\u2588')
                {

                    pacMan.y = pacMan.y + 1;
                }
            }
            else if (pressedKey.Key == ConsoleKey.UpArrow)
            {
                if (labyrinth[pacMan.x - 1][pacMan.y] != '\u2588')
                {
                    pacMan.x = pacMan.x - 1;
                }
            }
            else if (pressedKey.Key == ConsoleKey.DownArrow)
            {
                if (labyrinth[pacMan.x + 1][pacMan.y] != '\u2588')
                {
                    pacMan.x = pacMan.x + 1;
                }
            }

        }

    }

    static void MoveEnemies(Creature[] enemy)
    {
        for (int i = 0; i < enemiesCount; i++)
        {
            var lastDirection = enemy[i].lastDirection;
            //if enemy[i] moves right START here COPY>>>>>
            if (enemy[i].direction == "right" && (labyrinth[enemy[i].x][enemy[i].y + 1] == '\u2588') && (labyrinth[enemy[i].x - 1][enemy[i].y] == '\u2588'))
            {
                enemy[i].direction = "down";
                enemy[i].lastDirection = "right";
            }

            if (enemy[i].direction == "right" && (labyrinth[enemy[i].x][enemy[i].y + 1] == '\u2588') && (labyrinth[enemy[i].x + 1][enemy[i].y] == '\u2588'))
            {
                enemy[i].direction = "up";
                enemy[i].lastDirection = "right";
            }

            if (enemy[i].direction == "right" && (labyrinth[enemy[i].x][enemy[i].y + 1] == '\u2588') && (labyrinth[enemy[i].x - 1][enemy[i].y] == '\u2588') && (labyrinth[enemy[i].x + 1][enemy[i].y] == '\u2588'))
            {
                enemy[i].direction = "left";
                enemy[i].lastDirection = "right";
            }

            if (enemy[i].direction == "right" && (labyrinth[enemy[i].x][enemy[i].y + 1] == '\u2588') && (labyrinth[enemy[i].x - 1][enemy[i].y] == ' ') && (labyrinth[enemy[i].x + 1][enemy[i].y] == ' '))
            {
                //randomizer for enemy[i] movements.
                Random randomDirection = new Random();
                int enemyDirection = randomDirection.Next(2);
                if (enemyDirection == 0)
                {
                    enemy[i].direction = "up";
                }
                else if (enemyDirection == 1)
                {
                    enemy[i].direction = "down";
                }
                enemy[i].lastDirection = "right";
            }

            if (enemy[i].direction == "right" && (labyrinth[enemy[i].x][enemy[i].y + 1] == ' ') && (labyrinth[enemy[i].x + 1][enemy[i].y] == '\u2588') && (labyrinth[enemy[i].x][enemy[i].y - 1] == ' ') && (labyrinth[enemy[i].x - 1][enemy[i].y] == ' '))
            {
                Random randomDirection = new Random();
                int enemyDirection = randomDirection.Next(6);
                if (enemyDirection % 5 == 0)
                {
                    enemy[i].direction = "up";
                }
                else if (enemyDirection % 5 != 0)
                {
                    enemy[i].direction = "right";
                }
                enemy[i].lastDirection = "right";
            }

            if (enemy[i].direction == "right" && (labyrinth[enemy[i].x][enemy[i].y + 1] == ' ') && (labyrinth[enemy[i].x - 1][enemy[i].y] == '\u2588') && (labyrinth[enemy[i].x][enemy[i].y - 1] == ' ') && (labyrinth[enemy[i].x + 1][enemy[i].y] == ' '))
            {
                Random randomDirection = new Random();
                int enemyDirection = randomDirection.Next(6);
                if (enemyDirection % 5 == 0)
                {
                    enemy[i].direction = "down";
                }
                else if (enemyDirection % 5 != 0)
                {
                    enemy[i].direction = "right";
                }
                enemy[i].lastDirection = "right";
            }
            //if enemy[i] moves right END here

            //if enemy[i] moves down START here
            if (enemy[i].direction == "down" && (labyrinth[enemy[i].x][enemy[i].y + 1] == '\u2588') && (labyrinth[enemy[i].x + 1][enemy[i].y] == '\u2588'))
            {
                enemy[i].direction = "left";
                enemy[i].lastDirection = "down";
            }
            //checking for portal
            if (enemy[i].direction == "down" && (labyrinth[enemy[i].x + 1][enemy[i].y] == '\u0040'))
            {
                enemy[i].direction = "right";
                enemy[i].lastDirection = "down";
            }
            if (enemy[i].direction == "down" && (labyrinth[enemy[i].x][enemy[i].y - 1] == '\u2588') && (labyrinth[enemy[i].x + 1][enemy[i].y] == '\u2588'))
            {
                enemy[i].direction = "right";
                enemy[i].lastDirection = "down";
            }

            if (enemy[i].direction == "down" && (labyrinth[enemy[i].x][enemy[i].y - 1] == '\u2588') && (labyrinth[enemy[i].x + 1][enemy[i].y] == '\u2588') && (labyrinth[enemy[i].x][enemy[i].y + 1] == '\u2588'))
            {
                enemy[i].direction = "up";
                enemy[i].lastDirection = "down";
            }

            if (enemy[i].direction == "down" && (labyrinth[enemy[i].x][enemy[i].y + 1] == ' ') && (labyrinth[enemy[i].x + 1][enemy[i].y] == '\u2588') && (labyrinth[enemy[i].x][enemy[i].y - 1] == ' '))
            {
                Random randomDirection = new Random();
                int enemyDirection = randomDirection.Next(2);
                if (enemyDirection == 0)
                {
                    enemy[i].direction = "right";
                }
                else if (enemyDirection == 1)
                {
                    enemy[i].direction = "left";
                }
                enemy[i].lastDirection = "down";
            }

            if (enemy[i].direction == "down" && (labyrinth[enemy[i].x][enemy[i].y + 1] == '\u2588') && (labyrinth[enemy[i].x][enemy[i].y - 1] == ' ') && (labyrinth[enemy[i].x - 1][enemy[i].y] == ' ') && (labyrinth[enemy[i].x + 1][enemy[i].y] == ' '))
            {
                Random randomDirection = new Random();
                int enemyDirection = randomDirection.Next(6);
                if (enemyDirection % 5 == 0)
                {
                    enemy[i].direction = "left";
                }
                else if (enemyDirection % 5 != 0)
                {
                    enemy[i].direction = "down";
                }
                enemy[i].lastDirection = "down";
            }

            if (enemy[i].direction == "down" && (labyrinth[enemy[i].x][enemy[i].y + 1] == ' ') && (labyrinth[enemy[i].x][enemy[i].y - 1] == '\u2588') && (labyrinth[enemy[i].x - 1][enemy[i].y] == ' ') && (labyrinth[enemy[i].x + 1][enemy[i].y] == ' '))
            {
                Random randomDirection = new Random();
                int enemyDirection = randomDirection.Next(6);
                if (enemyDirection % 5 == 0)
                {
                    enemy[i].direction = "right";
                }
                else if (enemyDirection % 5 != 0)
                {
                    enemy[i].direction = "down";
                }
                enemy[i].lastDirection = "down";
            }
            //if enemy[i] moves down END here
            //if enemy[i] moves left START here
            if (enemy[i].direction == "left" && (labyrinth[enemy[i].x][enemy[i].y - 1] == '\u2588') && (labyrinth[enemy[i].x + 1][enemy[i].y] == '\u2588'))
            {
                enemy[i].direction = "up";
                enemy[i].lastDirection = "left";
            }

            if (enemy[i].direction == "left" && (labyrinth[enemy[i].x][enemy[i].y - 1] == '\u2588') && (labyrinth[enemy[i].x - 1][enemy[i].y] == '\u2588'))
            {
                enemy[i].direction = "down";
                enemy[i].lastDirection = "left";
            }

            if (enemy[i].direction == "left" && (labyrinth[enemy[i].x][enemy[i].y - 1] == '\u2588') && (labyrinth[enemy[i].x + 1][enemy[i].y] == '\u2588') && (labyrinth[enemy[i].x - 1][enemy[i].y] == '\u2588'))
            {
                enemy[i].direction = "right";
                enemy[i].lastDirection = "left";
            }

            if (enemy[i].direction == "left" && (labyrinth[enemy[i].x][enemy[i].y - 1] == '\u2588') && (labyrinth[enemy[i].x + 1][enemy[i].y] == ' ') && (labyrinth[enemy[i].x - 1][enemy[i].y] == ' '))
            {
                Random randomDirection = new Random();
                int enemyDirection = randomDirection.Next(2);
                if (enemyDirection == 0)
                {
                    enemy[i].direction = "up";
                }
                else if (enemyDirection == 1)
                {
                    enemy[i].direction = "down";
                }
                enemy[i].lastDirection = "left";
            }

            if (enemy[i].direction == "left" && (labyrinth[enemy[i].x - 1][enemy[i].y] == '\u2588') && (labyrinth[enemy[i].x + 1][enemy[i].y] == ' ') && (labyrinth[enemy[i].x][enemy[i].y - 1] == ' ') && (labyrinth[enemy[i].x][enemy[i].y + 1] == ' '))
            {
                Random randomDirection = new Random();
                int enemyDirection = randomDirection.Next(6);
                if (enemyDirection % 5 == 0)
                {
                    enemy[i].direction = "down";
                }
                else if (enemyDirection % 5 != 0)
                {
                    enemy[i].direction = "left";
                }
                enemy[i].lastDirection = "left";
            }

            if (enemy[i].direction == "left" && (labyrinth[enemy[i].x - 1][enemy[i].y] == ' ') && (labyrinth[enemy[i].x + 1][enemy[i].y] == '\u2588') && (labyrinth[enemy[i].x][enemy[i].y - 1] == ' ') && (labyrinth[enemy[i].x][enemy[i].y + 1] == ' '))
            {
                Random randomDirection = new Random();
                int enemyDirection = randomDirection.Next(6);
                if (enemyDirection % 5 == 0)
                {
                    enemy[i].direction = "up";
                }
                else if (enemyDirection % 5 != 0)
                {
                    enemy[i].direction = "left";
                }
                enemy[i].lastDirection = "left";
            }
            //if enemy[i] moves left END here

            //if enemy[i] moves up START here
            if (enemy[i].direction == "up" && (labyrinth[enemy[i].x][enemy[i].y - 1] == '\u2588') && (labyrinth[enemy[i].x - 1][enemy[i].y] == '\u2588'))
            {
                enemy[i].direction = "right";
                enemy[i].lastDirection = "up";
            }
            if (enemy[i].direction == "up" && (labyrinth[enemy[i].x - 1][enemy[i].y] == '\u0023'))
            {
                enemy[i].direction = "right";
                enemy[i].lastDirection = "up";
            }
            if (enemy[i].direction == "up" && (labyrinth[enemy[i].x][enemy[i].y + 1] == '\u2588') && (labyrinth[enemy[i].x - 1][enemy[i].y] == '\u2588'))
            {
                enemy[i].direction = "left";
                enemy[i].lastDirection = "up";
            }

            if (enemy[i].direction == "up" && (labyrinth[enemy[i].x][enemy[i].y - 1] == '\u2588') && (labyrinth[enemy[i].x - 1][enemy[i].y] == '\u2588') && (labyrinth[enemy[i].x][enemy[i].y + 1] == '\u2588'))
            {
                enemy[i].direction = "down";
                enemy[i].lastDirection = "up";
            }

            if (enemy[i].direction == "up" && (labyrinth[enemy[i].x][enemy[i].y - 1] == ' ') && (labyrinth[enemy[i].x - 1][enemy[i].y] == '\u2588') && (labyrinth[enemy[i].x][enemy[i].y + 1] == ' '))
            {
                Random randomDirection = new Random();
                int enemyDirection = randomDirection.Next(2);
                if (enemyDirection == 0)
                {
                    enemy[i].direction = "left";
                }
                else if (enemyDirection == 1)
                {
                    enemy[i].direction = "right";
                }
                enemy[i].lastDirection = "up";
            }

            if (enemy[i].direction == "up" && (labyrinth[enemy[i].x][enemy[i].y - 1] == ' ') && (labyrinth[enemy[i].x][enemy[i].y + 1] == '\u2588') && (labyrinth[enemy[i].x + 1][enemy[i].y] == ' ') && (labyrinth[enemy[i].x - 1][enemy[i].y] == ' '))
            {
                Random randomDirection = new Random();
                int enemyDirection = randomDirection.Next(6);
                if (enemyDirection % 5 == 0)
                {
                    enemy[i].direction = "left";
                }
                else if (enemyDirection % 5 != 0)
                {
                    enemy[i].direction = "up";
                }
                enemy[i].lastDirection = "up";
            }

            if (enemy[i].direction == "up" && (labyrinth[enemy[i].x][enemy[i].y - 1] == '\u2588') && (labyrinth[enemy[i].x][enemy[i].y + 1] == ' ') && (labyrinth[enemy[i].x + 1][enemy[i].y] == ' ') && (labyrinth[enemy[i].x - 1][enemy[i].y] == ' '))
            {
                Random randomDirection = new Random();
                int enemyDirection = randomDirection.Next(6);
                if (enemyDirection % 5 == 0)
                {
                    enemy[i].direction = "right";
                }
                else if (enemyDirection % 5 != 0)
                {
                    enemy[i].direction = "up";
                }
                enemy[i].lastDirection = "up";
            }

            //if enemy[i] moves left Up here COPY>>>>
            if (enemy[i].direction == "right" && labyrinth[enemy[i].x][enemy[i].y + 1] != '\u2666' && labyrinth[enemy[i].x][enemy[i].y + 2] != '\u2666')
            {
                enemy[i].y += 1;
            }
            if (enemy[i].direction == "down" && labyrinth[enemy[i].x + 1][enemy[i].y] != '\u2666' && labyrinth[enemy[i].x + 2][enemy[i].y] != '\u2666')
            {
                enemy[i].x += 1;
            }
            if (enemy[i].direction == "left" && labyrinth[enemy[i].x][enemy[i].y - 1] != '\u2666' && labyrinth[enemy[i].x][enemy[i].y - 2] != '\u2666')
            {
                enemy[i].y -= 1;
            }
            if (enemy[i].direction == "up" && labyrinth[enemy[i].x - 1][enemy[i].y] != '\u2666' && labyrinth[enemy[i].x - 2][enemy[i].y] != '\u2666')
            {
                enemy[i].x -= 1;
            }

        }
    }

    static string[,] ReadLevelsFromFile(int playfieldHeight, int playfieldWidth)
    {
        //Read all the levels from the file.


        //Create list for all levels.
        List<string[]> listOfLevels = new List<string[]>();

        try
        {
            //Create StreamReader for Levels.txt file.
            using (StreamReader streamReader = new StreamReader(@"..\..\Levels.txt"))
            {
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
            }
        }
        catch (ArgumentNullException argumentNullException)
        {
            Console.WriteLine("An error occured while trying to read the levels from Levels.txt\n{0}", argumentNullException.Message);
        }
        catch (ArgumentException argumentException)
        {
            Console.WriteLine("An error occured while trying to read the levels from Levels.txt\n{0}", argumentException.Message);
        }
        catch (FileNotFoundException fileNotFoundException)
        {
            Console.WriteLine("An error occured while trying to read the levels from Levels.txt\n{0}", fileNotFoundException.Message);
        }
        catch (DirectoryNotFoundException directoryNotFoundException)
        {
            Console.WriteLine("An error occured while trying to read the levels from Levels.txt\n{0}", directoryNotFoundException.Message);
        }
        catch (OutOfMemoryException outOfMemoryException)
        {
            Console.WriteLine("An error occured while trying to read the levels from Levels.txt\n{0}", outOfMemoryException.Message);
        }
        catch (IOException ioException)
        {
            Console.WriteLine("An error occured while trying to read the levels from Levels.txt\n{0}", ioException.Message);
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
        int rowsCount = 0;
        try
        {
            rowsCount = allLevels.GetLength(1);
        }
        catch (IndexOutOfRangeException indexOutOfRangeException)
        {
            Console.WriteLine("An error occured while selecting the level from the levels array.\n[{0}]", indexOutOfRangeException.Message);
        }

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

    static void PrintElement(Creature thisObject)
    {
        // print object of type Element
        Console.SetCursorPosition(thisObject.y, thisObject.x);
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
        string[] three = {"\u2588\u2588\u2588\u2588", 
                          "    \u2588",
                          "\u2588\u2588\u2588\u2588",
                          "    \u2588",
                          "\u2588\u2588\u2588\u2588"
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

    static void PrintMenu()
    {

        #region Draw borders
        // top line border print
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.SetCursorPosition(20, 0);
        StringBuilder topLine = new StringBuilder();
        topLine.Append("  ");
        topLine.Append("►►►►►►►►");
        topLine.Append("↓");
        topLine.Append("◄◄◄◄◄◄◄◄");
        Console.WriteLine(topLine);


        //bottom line border print
        Console.SetCursorPosition(20, 19);
        StringBuilder bottomLine = new StringBuilder();
        bottomLine.Append("  ");
        bottomLine.Append("►►►►►►►►");
        bottomLine.Append("↑");
        bottomLine.Append("◄◄◄◄◄◄◄◄");
        Console.WriteLine(bottomLine);
        #endregion

        #region LifeCounter

        int lifesCounter = 4; // hardcored for now - take the global for lifes remaining after it is ready

        Console.BackgroundColor = ConsoleColor.DarkCyan;
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(23, 2);
        StringBuilder lifeCountFrame = new StringBuilder();
        lifeCountFrame.AppendLine("╔════════════╗");
        Console.WriteLine(lifeCountFrame);

        Console.SetCursorPosition(23, 3);
        StringBuilder lifeCountFrame2 = new StringBuilder();
        lifeCountFrame2.AppendLine(" Lifes ►      "); ;
        Console.WriteLine(lifeCountFrame2);

        Console.SetCursorPosition(23, 4);
        StringBuilder lifeCountFrame3 = new StringBuilder();
        lifeCountFrame3.AppendLine("╚════════════╝");
        Console.WriteLine(lifeCountFrame3);
        Console.ResetColor();

        switch (lifesCounter)
        {
            case 1:
                {
                    Console.SetCursorPosition(31, 3);
                    StringBuilder lifesLeft = new StringBuilder();
                    lifesLeft.AppendLine(new string((char)9787, 1));
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(lifesLeft);
                }
                break;

            case 2:
                {
                    Console.SetCursorPosition(31, 3);
                    StringBuilder lifesLeft = new StringBuilder();
                    lifesLeft.AppendLine(new string((char)9787, 2));
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(lifesLeft);
                }
                break;
            case 3:
                {
                    Console.SetCursorPosition(31, 3);
                    StringBuilder lifesLeft = new StringBuilder();
                    lifesLeft.AppendLine(new string((char)9787, 3));
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(lifesLeft);
                }
                break;
            case 4:
                {
                    Console.SetCursorPosition(31, 3);
                    StringBuilder lifesLeft = new StringBuilder();
                    lifesLeft.AppendLine(new string((char)9787, 4));
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(lifesLeft);
                }
                break;
            case 5:
                {
                    Console.SetCursorPosition(31, 3);
                    StringBuilder lifesLeft = new StringBuilder();
                    lifesLeft.AppendLine(new string((char)9787, 5));
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(lifesLeft);
                }
                break;
            default: break;
        }
        Console.ResetColor();
        #endregion // Lifecounter

        #region ScoreCounter

        Console.BackgroundColor = ConsoleColor.DarkYellow;
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(23, 6);
        StringBuilder scoreCounterFrameTop = new StringBuilder();
        scoreCounterFrameTop.AppendLine("╔════════════╗");
        Console.WriteLine(scoreCounterFrameTop);
        Console.SetCursorPosition(23, 7);
        Console.WriteLine(" SCORE ►{0,5} ", score); // added real score counter
        Console.SetCursorPosition(23, 8);
        StringBuilder scoreCounterFrame = new StringBuilder();
        scoreCounterFrame.AppendLine("╚════════════╝");
        Console.WriteLine(scoreCounterFrame);
        Console.ResetColor();



        #endregion

        #region LevelCounter


        string textLevel = " LEVEL ";
        int levels = levelNumber + 1; // real level counter added  - works 
        Console.SetCursorPosition(23, 8);
        switch (levels)
        {
            case 1:
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(23, 10);
                    StringBuilder levelFrameTOp = new StringBuilder();
                    levelFrameTOp.AppendLine("╔════════════╗");
                    Console.WriteLine(levelFrameTOp);
                    Console.SetCursorPosition(23, 11);
                    Console.Write("{1}► {0,4} ", levels, textLevel);

                    Console.SetCursorPosition(23, 12);
                    StringBuilder levelFrameBottom = new StringBuilder();
                    levelFrameBottom.AppendLine("╚════════════╝");
                    Console.WriteLine(levelFrameBottom);
                    Console.ResetColor();
                }
                break;
            case 2:
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(23, 10);
                    StringBuilder levelFrameTOp = new StringBuilder();
                    levelFrameTOp.AppendLine("╔════════════╗");
                    Console.WriteLine(levelFrameTOp);
                    Console.SetCursorPosition(23, 11);
                    Console.Write("{1}► {0,4} ", levels, textLevel);

                    Console.SetCursorPosition(23, 12);
                    StringBuilder levelFrameBottom = new StringBuilder();
                    levelFrameBottom.AppendLine("╚════════════╝");
                    Console.WriteLine(levelFrameBottom);
                    Console.ResetColor();
                }
                break;
            case 3:
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(23, 10);
                    StringBuilder levelFrameTOp = new StringBuilder();
                    levelFrameTOp.AppendLine("╔════════════╗");
                    Console.WriteLine(levelFrameTOp);
                    Console.SetCursorPosition(23, 11);
                    Console.Write("{1}► {0,4} ", levels, textLevel);

                    Console.SetCursorPosition(23, 12);
                    StringBuilder levelFrameBottom = new StringBuilder();
                    levelFrameBottom.AppendLine("╚════════════╝");
                    Console.WriteLine(levelFrameBottom);
                    Console.ResetColor();
                }
                break;
            case 4:
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(23, 10);
                    StringBuilder levelFrameTOp = new StringBuilder();
                    levelFrameTOp.AppendLine("╔════════════╗");
                    Console.WriteLine(levelFrameTOp);
                    Console.SetCursorPosition(23, 11);
                    Console.Write("{1}► {0,4} ", levels, textLevel);

                    Console.SetCursorPosition(23, 12);
                    StringBuilder levelFrameBottom = new StringBuilder();
                    levelFrameBottom.AppendLine("╚════════════╝");
                    Console.WriteLine(levelFrameBottom);
                    Console.ResetColor();
                }
                break;
            case 5:
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(23, 10);
                    StringBuilder levelFrameTOp = new StringBuilder();
                    levelFrameTOp.AppendLine("╔════════════╗");
                    Console.WriteLine(levelFrameTOp);
                    Console.SetCursorPosition(23, 11);
                    Console.Write("{1}► {0,4} ", levels, textLevel);

                    Console.SetCursorPosition(23, 12);
                    StringBuilder levelFrameBottom = new StringBuilder();
                    levelFrameBottom.AppendLine("╚════════════╝");
                    Console.WriteLine(levelFrameBottom);
                    Console.ResetColor();
                }
                break;
            default: break;
        }
        #endregion

        // Here is an additional intercativity to change the screen when Pacman ets a bonus , new level , ect .. needs a bool variable
        #region PacMan Area

        bool bonusTaken = false; // hardcored for now

        if (tempScore != score)
        {
            bonusTaken = true;
        }
        tempScore = score;

        if (bonusTaken)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(23, 14);
            StringBuilder pacmanFrameTop = new StringBuilder();
            Console.ForegroundColor = ConsoleColor.White;
            pacmanFrameTop.AppendLine("╔════════════╗");
            Console.WriteLine(pacmanFrameTop);
            Console.SetCursorPosition(23, 15);
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("              ");
            Console.SetCursorPosition(23, 16);
            Console.WriteLine(" BONUS POINTS ");
            Console.SetCursorPosition(23, 17);
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("              ");
            Console.SetCursorPosition(23, 18);
            StringBuilder pacmanFrameBottom = new StringBuilder();
            Console.ForegroundColor = ConsoleColor.White;
            pacmanFrameBottom.AppendLine("╚════════════╝");
            Console.WriteLine(pacmanFrameBottom);
            Console.ResetColor();

        }
        else
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(22, 14);
            StringBuilder pacmanFrameTop = new StringBuilder();
            pacmanFrameTop.AppendLine("╔═══════════════╗");
            Console.WriteLine(pacmanFrameTop);
            Console.SetCursorPosition(22, 15);
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("                 ");
            Console.SetCursorPosition(22, 16);
            Console.WriteLine("►►► PACMAN 3D ◄◄◄");
            Console.SetCursorPosition(22, 17);
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("                 ");
            Console.SetCursorPosition(22, 18);
            StringBuilder pacmanFrameBottom = new StringBuilder();
            pacmanFrameBottom.AppendLine("╚═══════════════╝");
            Console.WriteLine(pacmanFrameBottom);
            Console.ResetColor();
        }
        bonusTaken = false;
        #endregion
    }

    static void ResultsFileTxt()
    {
        try
        {
            StreamWriter streamWriter = new StreamWriter(@"..\..\results.txt", append: true);
            using (streamWriter)  //  ../about.html
            {
                int level = levelNumber + 1;// hardocered needs global variables
                int scoreFile = score;// hardocered needs global variables
                DateTime date = DateTime.Now;
                //for (int number = 1; number <= 5; number++)
                //{
                streamWriter.WriteLine("{3}'s result : {0,3} | Level reached : {1,2} | Date played : {2,10}", scoreFile, level, date, heroName);
                //}
            }
        }
        catch (ArgumentNullException argumentNullException)
        {
            Console.WriteLine("An error occured while trying to write to file results.txt\n{0}", argumentNullException.Message);
        }
        catch (ArgumentException argumentException)
        {
            Console.WriteLine("An error occured while trying to write to file results.txt\n{0}", argumentException.Message);
        }
        catch (UnauthorizedAccessException unauthorizedAccessException)
        {
            Console.WriteLine("An error occured while trying to write to file results.txt\n{0}", unauthorizedAccessException.Message);
        }
        catch (DirectoryNotFoundException directoryNotFoundException)
        {
            Console.WriteLine("An error occured while trying to write to file results.txt\n{0}", directoryNotFoundException.Message);
        }
        catch (ObjectDisposedException objectDisposedException)
        {
            Console.WriteLine("An error occured while trying to write to file results.txt\n{0}", objectDisposedException.Message);
        }
        catch (PathTooLongException pathTooLongException)
        {
            Console.WriteLine("An error occured while trying to write to file results.txt\n{0}", pathTooLongException.Message);
        }
        catch (IOException ioException)
        {
            Console.WriteLine("An error occured while trying to write to file results.txt\n{0}", ioException.Message);
        }
        catch (SecurityException securityException)
        {
            Console.WriteLine("An error occured while trying to write to file results.txt\n{0}", securityException.Message);
        }
        catch (FormatException formatException)
        {
            Console.WriteLine("An error occured while trying to write to file results.txt\n{0}", formatException.Message);
        }
    }
}