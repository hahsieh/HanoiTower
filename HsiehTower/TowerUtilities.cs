using System;
using static System.Console;

public static class TowerUtilities
{
    const int NumberOfPoles = 3;
    const string TowerPole = "|";


    public static void DisplayTowers(Towers towers)
    {
        //Calculate tower width.  We'll make each disc twice as wide as its number, plus 1 so the pole can go up the middle
        int[][] poles;
        string[][] poleDiscs = new string[NumberOfPoles][];
        string rowToPrint;
        int posX, posY;

        int towerWidth = (towers.NumberOfDiscs * 2 + 1);
        string towerBase = new string('=', towerWidth * 3);
        int towerHeight = towers.NumberOfDiscs + 2;

        DisplayTowerTop(towerWidth);

        poles = towers.ToArray();       //Returns a jagged array - Dim 1 - 3 poles, Dim 2 - n discs on each pole

        //
        //Load one array per pole with the visualization of the discs on each pole
        //

        for (int i = 0; i < NumberOfPoles; i++)
        {

            poleDiscs[i] = new string[towers.NumberOfDiscs];
            posY = poleDiscs[i].Length - 1;

            int stackIndex = poles[i] == null ? -1 : poles[i].Length - 1;
            while (stackIndex >= 0)
            {
                poleDiscs[i][posY] = new string('X', poles[i][stackIndex] * 2 + 1);
                posY--;
                stackIndex--;
            }
        }


        //Now to print the contents of the three pole arrays one line at a time.  

        for (posY = 0; posY < towers.NumberOfDiscs; posY++)   //Iterate from the top of the poles to the base, by column then by row.  
        {
            rowToPrint = "";

            for (posX = 0; posX < NumberOfPoles; posX++)
            {
                if (poleDiscs[posX][posY] == null)
                {
                    rowToPrint = FormatRow(rowToPrint, TowerPole, towerWidth);
                }
                else
                {
                    rowToPrint = FormatRow(rowToPrint, poleDiscs[posX][posY], towerWidth);
                }
            }
            WriteLine(rowToPrint);
        }
        WriteLine(towerBase);
        InitializeTextArea();
        WriteLine();
    }
    private static string FormatRow(string currentString, string newContent, int towerWidth)
    {
        if (newContent.Length % 2 != 1) throw new ApplicationException();
        string spaces = new string(' ', (towerWidth - newContent.Length) / 2);
        string soFar = currentString + spaces + newContent + spaces;
        return soFar;
    }

    private static void DisplayTowerTop(int towerWidth)
    {
        int posX;
        string[] towerLabels = { "1", "2", "3" };


        SetCursorPosition(0, 0);    //Always at the top of the window
        WriteLine();
        string rowToPrint = "";
        for (posX = 0; posX < NumberOfPoles; posX++)
        {
            rowToPrint = FormatRow(rowToPrint, $"{towerLabels[posX]}", towerWidth);
        }
        WriteLine(rowToPrint);
        WriteLine();
        rowToPrint = "";

        //Display the tops of the poles.  
        rowToPrint = "";
        for (posX = 0; posX < NumberOfPoles; posX++)
        {
            rowToPrint = FormatRow(rowToPrint, TowerPole, towerWidth);
        }
        WriteLine(rowToPrint);

    }

    private static void InitializeTextArea()
    {
        int originalTop = CursorTop;

        for (int i = originalTop; i < WindowHeight - 1; i++)
        {
            SetCursorPosition(0, i);
            WriteLine(new string(' ', BufferWidth));
        }
        SetCursorPosition(0, originalTop);
    }



}