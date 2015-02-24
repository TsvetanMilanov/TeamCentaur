using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

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
    public Creature(int x=0, int y=0, char skin='?', ConsoleColor colour = ConsoleColor.Gray)
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
    // Global variables
    private const int playfieldHeight = 20;
    private const int playfieldWidth = 20;

    private static Creature pacMan = new Creature(playfieldHeight / 2, playfieldWidth / 2, (char)9787, ConsoleColor.Yellow);

    private static int enemiesCount = 4;

    private static Creature[] enemies = new Creature[enemiesCount];

    private static string[] labyrinth;

    // Check if the game is over variable
    private static bool isGameOver = false;

    // Enemy Even move counter
    private static long enemyEvenMoveCounter = 1;
    private static Random randomizer = new Random();
    //Gold
    private static Creature gold = new Creature(1, 1, '$', ConsoleColor.Yellow);
    private static int score = 0;
    
    static void Main()
    {
        // set console size (screen resolution)
        Console.BufferHeight = Console.WindowHeight = 21;
        Console.BufferWidth = Console.WindowWidth = 40;

        // labyrinth initializer
        int levelNumber = 0; //The number of the level which will be printed.
        int levelsCount = 4; //The count of all the levels in Levels.txt file.
        //2D string array which will contain all the levels.
        string[,] allLevels = new string[levelsCount, playfieldWidth];
        //Read all the levels from the file useing ReadLevelsFromFile().
        allLevels = ReadLevelsFromFile(playfieldHeight, playfieldWidth);
        labyrinth = selectLevel(allLevels, levelNumber);

        // enemy initializer
        for (int i = 0; i < enemiesCount; i++)
        {
            int x = (playfieldHeight - 3) / randomizer.Next(1, 5);
            int y = (playfieldWidth - 3) / randomizer.Next(1, 5);
            enemies[i] = new Creature(x, y, '\u2666', ConsoleColor.Red);

            int enemyStartDirection = randomizer.Next(1, 5); // randomizining enemy start direction
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
       

        StartupScreen();

        // Main game logic
        while (true)
        {
            //The array labyrinth is equal to the current level we select.
            labyrinth = selectLevel(allLevels, levelNumber);

            // Move the enemy per 2 steps
            if (enemyEvenMoveCounter % 2 == 0)
            {                
                MoveEnemies(enemies);
            }

            // detect pacman movement every frame redraw
            MovePacMan();
            if ((pacMan.x == gold.x) && (pacMan.y == gold.y) && gold.skin == '$')
            {
                gold.skin = ' ';
                score++;
            }

            //Checking for impact when you add enemy you must add the enemy in this method (CheckForImpact())
            CheckForImpact();

            //CheckForImpact gives true or false on isGameOver
            if (isGameOver)
            {
                Console.Clear();
                Console.WriteLine("Game Over");
                break;
            }

            PrintFrame();
            enemyEvenMoveCounter++;
            Thread.Sleep(100);  // control game speed
        }
    }
        
    private static void CheckForImpact()
    {
        for (int i = 0; i < enemiesCount; i++)
        {
            if (enemies[i].y == pacMan.y && enemies[i].x == pacMan.x)
            {
                Console.WriteLine("Game Over !");
                isGameOver = true;
            }
        }
    }

    private static void PrintFrame()
    {
        Console.Clear();    // fast screen clear
        PrintLabyrinth(labyrinth);
        PrintElement(gold);
        PrintElement(pacMan);
        // print all enemies
        for (int i = 0; i < enemiesCount; i++)
        {
            PrintElement(enemies[i]);            
        }
    }
       
    private static void MovePacMan()
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
            else if (pressedKey.Key == ConsoleKey.UpArrow )
            {
                if (labyrinth[pacMan.x - 1][pacMan.y] != '\u2588')
                {
                    pacMan.x = pacMan.x - 1;
                }
            }
            else if (pressedKey.Key == ConsoleKey.DownArrow )
            {
                if (labyrinth[pacMan.x + 1][pacMan.y] != '\u2588')
                {
                    pacMan.x = pacMan.x + 1;
                }
            }
          
        }
       
    }
    
    private static void MoveEnemies(Creature[] enemy)
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
                int enemyDirection = randomDirection.Next(9);
                if (enemyDirection % 8 == 0)
                {
                    enemy[i].direction = "up";
                }
                else if (enemyDirection % 8 != 0)
                {
                    enemy[i].direction = "right";
                }
                enemy[i].lastDirection = "right";
            }

            if (enemy[i].direction == "right" && (labyrinth[enemy[i].x][enemy[i].y + 1] == ' ') && (labyrinth[enemy[i].x - 1][enemy[i].y] == '\u2588') && (labyrinth[enemy[i].x][enemy[i].y - 1] == ' ') && (labyrinth[enemy[i].x + 1][enemy[i].y] == ' '))
            {
                Random randomDirection = new Random();
                int enemyDirection = randomDirection.Next(9);
                if (enemyDirection % 8 == 0)
                {
                    enemy[i].direction = "down";
                }
                else if (enemyDirection % 8 != 0)
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
                int enemyDirection = randomDirection.Next(9);
                if (enemyDirection % 8 == 0)
                {
                    enemy[i].direction = "left";
                }
                else if (enemyDirection % 8 != 0)
                {
                    enemy[i].direction = "down";
                }
                enemy[i].lastDirection = "down";
            }

            if (enemy[i].direction == "down" && (labyrinth[enemy[i].x][enemy[i].y + 1] == ' ') && (labyrinth[enemy[i].x][enemy[i].y - 1] == '\u2588') && (labyrinth[enemy[i].x - 1][enemy[i].y] == ' ') && (labyrinth[enemy[i].x + 1][enemy[i].y] == ' '))
            {
                Random randomDirection = new Random();
                int enemyDirection = randomDirection.Next(9);
                if (enemyDirection % 8 == 0)
                {
                    enemy[i].direction = "right";
                }
                else if (enemyDirection % 8 != 0)
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
                int enemyDirection = randomDirection.Next(9);
                if (enemyDirection % 8 == 0)
                {
                    enemy[i].direction = "down";
                }
                else if (enemyDirection % 8 != 0)
                {
                    enemy[i].direction = "left";
                }
                enemy[i].lastDirection = "left";
            }

            if (enemy[i].direction == "left" && (labyrinth[enemy[i].x - 1][enemy[i].y] == ' ') && (labyrinth[enemy[i].x + 1][enemy[i].y] == '\u2588') && (labyrinth[enemy[i].x][enemy[i].y - 1] == ' ') && (labyrinth[enemy[i].x][enemy[i].y + 1] == ' '))
            {
                Random randomDirection = new Random();
                int enemyDirection = randomDirection.Next(9);
                if (enemyDirection % 8 == 0)
                {
                    enemy[i].direction = "up";
                }
                else if (enemyDirection % 8 != 0)
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
                int enemyDirection = randomDirection.Next(9);
                if (enemyDirection % 8 == 0)
                {
                    enemy[i].direction = "left";
                }
                else if (enemyDirection % 8 != 0)
                {
                    enemy[i].direction = "up";
                }
                enemy[i].lastDirection = "up";
            }

            if (enemy[i].direction == "up" && (labyrinth[enemy[i].x][enemy[i].y - 1] == '\u2588') && (labyrinth[enemy[i].x][enemy[i].y + 1] == ' ') && (labyrinth[enemy[i].x + 1][enemy[i].y] == ' ') && (labyrinth[enemy[i].x - 1][enemy[i].y] == ' '))
            {
                Random randomDirection = new Random();
                int enemyDirection = randomDirection.Next(9);
                if (enemyDirection % 8 == 0)
                {
                    enemy[i].direction = "right";
                }
                else if (enemyDirection % 8 != 0)
                {
                    enemy[i].direction = "up";
                }
                enemy[i].lastDirection = "up";
            }

            //if enemy[i] moves left Up here COPY>>>>
            if (enemy[i].direction == "right")
            {
                enemy[i].y += 1;
            }
            if (enemy[i].direction == "down")
            {
                enemy[i].x += 1;
            }
            if (enemy[i].direction == "left")
            {
                enemy[i].y -= 1;
            }
            if (enemy[i].direction == "up")
            {
                enemy[i].x -= 1;
            }
        }
    }

    static string[,] ReadLevelsFromFile(int playfieldHeight, int playfieldWidth)
    {
        //Read all the levels from the file.

        //Create StreamReader for Levels.txt file.
        StreamReader streamReader = new StreamReader(@"..\..\Levels.txt");

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
}