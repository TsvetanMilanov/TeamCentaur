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


        #region Old labyrinth
        // define labyrinth variable and build example
        //string[] labyrinth = new string[playfieldHeight];
        //Random rand = new Random();
        //for (int row = 0; row < playfieldHeight; row++)
        //{
        //    for (int col = 0; col < playfieldWidth; col++)
        //    {
        //        if (row == playfieldHeight / 2 && col == playfieldWidth / 2)
        //        {
        //            labyrinth[row] += " ";
        //            continue;
        //        }
        //        int chance = rand.Next(1, 101);
        //        if (chance < 20)
        //        {
        //            labyrinth[row] += "#";
        //        }
        //        else
        //        {
        //            labyrinth[row] += " ";
        //        }
        //    }
        //}
        #endregion

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
}

