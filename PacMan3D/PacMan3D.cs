using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

struct Element
{
    // define new object type with structure
    // screen position x,y
    public int x;
    public int y;
    // type of game object is defined by ASCII char
    public char skin;
    public ConsoleColor colour;
}

class PacMan3D
{
    static void Main()
    {
        #region Memory Initialization

        int playfieldHeight;
        int playfieldWidth;
        int levelNumber = 0; //The number of the level which will be printed.

        //Method for creating playing field
        SetPlayfieldSize(out playfieldHeight, out playfieldWidth);

        StartupScreen();

        // define hero pacMan as a variable of type element
        Element pacMan = new Element();
        // initial pacman position in center of playfield
        pacMan.x = playfieldWidth / 2;
        pacMan.y = playfieldHeight / 2;
        pacMan.skin = (char)9787; // utf8 decimal code 9787 (smile face) is our hero character
        pacMan.colour = ConsoleColor.Yellow;

        int levelsCount = 4; //The count of all the levels in Levels.txt file.

        //2D string array which will contain all the levels.
        string[,] allLevels = new string[levelsCount, playfieldWidth];

        //Read all the levels from the file useing ReadLevelsFromFile().
        allLevels = ReadLevelsFromFile(playfieldHeight, playfieldWidth);
        
        #endregion

        while (true)
        {
            //The array labyrinth is equal to the current level we select.
            string[] labyrinth = selectLevel(allLevels, levelNumber);

            #region Build Frame

            // move PacMan
            while (Console.KeyAvailable)
            {
                // we assign the pressed key value to a variable pressedKey
                ConsoleKeyInfo pressedKey = Console.ReadKey(true);
                // next we start checking the value of the pressed key and take action if neccessary
                if (pressedKey.Key == ConsoleKey.LeftArrow && pacMan.x > 1) // if left arrow is pressed then
                {
                    if (labyrinth[pacMan.y][pacMan.x - 1] != '#')
                    {
                        pacMan.x = pacMan.x - 1;
                    }
                }
                else if (pressedKey.Key == ConsoleKey.RightArrow && pacMan.x < playfieldWidth - 1)
                {
                    if (labyrinth[pacMan.y][pacMan.x + 1] != '#')
                    {

                        pacMan.x = pacMan.x + 1;
                    }
                }
                else if (pressedKey.Key == ConsoleKey.UpArrow && pacMan.y > 1)
                {
                    if (labyrinth[pacMan.y - 1][pacMan.x] != '#')
                    {
                        pacMan.y = pacMan.y - 1;
                    }
                }
                else if (pressedKey.Key == ConsoleKey.DownArrow && pacMan.y < playfieldHeight - 1)
                {
                    if (labyrinth[pacMan.y + 1][pacMan.x] != '#')
                    {
                        pacMan.y = pacMan.y + 1;
                    }
                }
            }

            #endregion

            #region Print Frame

            Console.Clear();    // fast screen clear
            PrintLabyrinth(labyrinth);
            PrintElement(pacMan);

            #endregion

            Thread.Sleep(150);  // control game speed
        }
    }

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
        Console.ForegroundColor = ConsoleColor.Gray;
        for (int i = 0; i < thisArray.Length; i++)
        {
            Console.WriteLine(thisArray[i]);
        }
    }

    /// <summary>
    /// The code below creates the main start menu and a second instructions submenu.
    /// A two-dimensional array for the border (smileyface) and one-dimensional
    /// arrays for the letters in the PacMan "Logo" are used. Moreover, three 
    /// methods - one for the game instructions and two for printing the smileyface and 
    /// PacMan "Logo" arrays are also implemented.
    /// </summary>
  
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