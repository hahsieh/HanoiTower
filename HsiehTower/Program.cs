﻿using System;
using static System.Console;
using static TowerUtilities;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;

namespace TowersUI
{
    class Program
    {
        static readonly Queue<MoveRecord> myRecord = new Queue<MoveRecord> { };
        static readonly Stack<MoveRecord> undoRecord = new Stack<MoveRecord> { };
        static readonly Stack<MoveRecord> redoRecord = new Stack<MoveRecord> { };
        const string CtrlZ = "\u001a";
        const string CtrlY = "\u0019";

        static void Main(string[] args)
        {
            Towers myTowers;            
            int mode;

            GetTower(out myTowers);

            Console.Clear();
            DisplayTowers(myTowers);

            mode = SolveOption(myTowers);
                                    
            while (mode != 0)
            {
                myRecord.Clear();
                undoRecord.Clear();
                redoRecord.Clear();
                if (mode == 1)
                {
                    Console.Clear();
                    GetTower(out myTowers);                    
                }
                else if (mode == 2)
                {
                    myTowers = new Towers(myTowers.NumberOfDiscs);
                }
                Console.Clear();
                DisplayTowers(myTowers);
                mode = SolveOption(myTowers); 
            }

            WriteLine();
            WriteLine("Press any key to finish up.");

            ReadKey();
        }

        // mode: return 0 when the user stops playing the game, return 1 when the user plays next round, 
        // and return 2 when the user tries again to solve the same problem in the fewest possible moves
        private static int SolveOption(Towers myTowers)
        {
            string input;

            do
            {
                WriteLine();
                WriteLine("Options: ");
                WriteLine("- M - Solve the puzzle manually");
                WriteLine("- A - Auto - solve");
                WriteLine();
                WriteLine();
                Write("Choose an approach: ");
                input = ReadKey().KeyChar.ToString().ToUpper();
                WriteLine();
                if (input == "M")
                {
                    return Play(myTowers);
                }
                else if (input == "A")
                {
                    Console.Clear();
                    DisplayTowers(myTowers);
                    WriteLine();
                    WriteLine("Press a key and watch closely!");
                    ReadKey();
                    return AutoPlay(myTowers);
                }
                else
                {
                    WriteLine("Invalid value. Valid values: 'M', or 'A' ");
                }
            } while (input != "M" && input != "A");
            return -1;
        }

        private static int AutoPlay(Towers myTowers)
        {
            int from = 1;
            int to = 3;
            int other = 2;

            AutoMove(myTowers, myTowers.NumberOfDiscs, from, to, other);
            WriteLine($"Number of moves: {myTowers.NumberOfMoves}");
            WriteLine();
            WriteLine();

            ListRecord();
            WriteLine();
            WriteLine();

            return AskForPlayAgain();
        }

        private static void AutoMove(Towers myTowers, int n, int from, int to, int other)
        {
            MoveRecord theRecord;
            if (n == 1)
            {
                theRecord = myTowers.Move(from,to);
                myRecord.Enqueue(theRecord);
                Console.Clear();
                DisplayTowers(myTowers);
                WriteLine($"\nMove {myTowers.NumberOfMoves} complete. Successfully moved disc from tower {from} to tower {to}");
                Thread.Sleep(250);
                return;
            }

            AutoMove(myTowers, n - 1, from, other, to);
            theRecord = myTowers.Move(from, to);
            myRecord.Enqueue(theRecord);
            Console.Clear();
            DisplayTowers(myTowers);
            WriteLine($"\nMove {myTowers.NumberOfMoves} complete. Successfully moved disc from tower {from} to tower {to}");
            Thread.Sleep(250);
            AutoMove(myTowers, n - 1, other, to , from);
        }

        // status: return 2 when the move is complete (the user presses enter for 'To' tower to cancel, undo or redo the move)  
        // return 1 when the user wants to quit (presses 'x' for 'From' tower),
        // and return 0 keep continuing to move the discs
        private static int GetMove(out int from, out int to, Towers myTowers)
        {
            from = 0;
            to = 0;
            bool foundFrom;
            bool foundTo;
            string fromInput;
            string toInput;
            Regex myRegex = new Regex(@"^[0-9]"); 

            do
            {
                Write($"\nMove {myTowers.NumberOfMoves + 1}:");
                if (redoRecord.Count != 0)
                {
                    Write("\nEnter 'from' tower number (1-3), \"Ctrl + z\" to undo, \"Ctrl + y\" to redo, or \"x\" to quit: ");
                }
                else
                {
                    Write("\nEnter 'from' tower number (1-3), \"Ctrl + z\" to undo, or \"x\" to quit: ");
                }
                fromInput = ReadKey(true).KeyChar.ToString();
                if (myRegex.IsMatch(fromInput))
                {
                    Write($"{fromInput}");
                }
                WriteLine();
                if (int.TryParse(fromInput, out from))
                {
                    if (from < 1 || from > 3)
                    {
                        WriteLine("Invalid tower value. Valid values for 'From': '1', '2', '3', 'Ctrl-z', or 'x' ");
                        foundFrom = false;
                    }
                    else
                    {
                        foundFrom = true;
                    }
                }
                else if (fromInput == CtrlZ)
                {
                    Undo(myTowers);
                    foundFrom = true;
                    return 2;
                }
                else if (fromInput == CtrlY)
                {
                    Redo(myTowers);
                    foundFrom = true;
                    return 2;
                }
                else if (fromInput.ToUpper() == "X" || fromInput.ToUpper() == "Q")
                {
                    foundFrom = true;
                    return 1;                 // the user wwants to quit 
                }
                else
                {
                    WriteLine("Invalid value. Valid values for 'From': '1', '2', '3', 'Ctrl-z', or 'x' ");
                    foundFrom = false;
                }
            } while (!foundFrom);

            do
            {
                Write("\nEnter 'to' tower number (1-3) or enter to cancel: ");
                toInput = ReadKey().KeyChar.ToString().ToUpper();
                WriteLine();
                if (int.TryParse(toInput, out to))
                {
                    if (to < 1 || to > 3)
                    {
                        WriteLine("Invalid tower value. Valid values for 'To': '1', '2', '3', or enter");
                        foundTo = false;
                    }
                    else
                    {
                        foundTo = true;
                    }
                }
                else if (toInput == "\r")
                {
                    WriteLine("\nOkay, cancel the move.");
                    foundTo = true;
                    return 2;               // the user wants to cancel the move
                }
                else
                {
                    WriteLine("Invalid input.");
                    foundTo = false;
                }
            } while (!foundTo);

            return 0;
        }

        private static void Redo(Towers myTowers)
        {
            if (redoRecord.Count == 0)
            {
                WriteLine("Invalid value. Valid values for 'From': '1', '2', '3', 'Ctrl-z', or 'x' ");
            }
            else
            {
                MoveRecord theRecord = redoRecord.Pop();
                undoRecord.Push(theRecord);
                myRecord.Enqueue(myTowers.Move(theRecord.From, theRecord.To));
                Console.Clear();
                DisplayTowers(myTowers);
                WriteLine($"\nMove {myTowers.NumberOfMoves} complete by redo of move {theRecord.MoveNumber}. Disc {theRecord.Disc} returned to tower {theRecord.To} from tower {theRecord.From}");
            }
        }

        private static void Undo(Towers myTowers)
        {
            if (undoRecord.Count == 0)
            {
                WriteLine("Can’t undo");
            }
            else
            {
                MoveRecord theRecord = undoRecord.Pop();
                redoRecord.Push(theRecord);
                myRecord.Enqueue(myTowers.Move(theRecord.To, theRecord.From));
                Console.Clear();
                DisplayTowers(myTowers);
                WriteLine($"\nMove {myTowers.NumberOfMoves} complete by undo of move {theRecord.MoveNumber}. Disc {theRecord.Disc} restored to tower {theRecord.From} from tower {theRecord.To}");
            }
        }

        
        private static int Play(Towers myTowers)
        {
            int status;
            MoveRecord theRecord;            

            do
            {
                status = GetMove(out int from, out int to, myTowers);
                if (status == 1)
                {
                    if (myTowers.NumberOfMoves == 1)
                    {
                        WriteLine($"\nWell, you hung in there for {myTowers.NumberOfMoves} move.");
                    }
                    else if (myTowers.NumberOfMoves > 1)
                    {
                        WriteLine($"\nWell, you hung in there for {myTowers.NumberOfMoves} moves. Nice try.");
                    }
                    break;
                }
                else if (status == 0)
                {
                    try
                    {
                        theRecord = myTowers.Move(from, to);
                        myRecord.Enqueue(theRecord);
                        undoRecord.Push(theRecord);
                        redoRecord.Clear();
                        Console.Clear();
                        DisplayTowers(myTowers);
                        WriteLine($"\nMove {myTowers.NumberOfMoves} complete. Successfully moved disc from tower {from} to tower {to}");
                    }
                    catch (InvalidMoveException e)
                    {
                        WriteLine(e.Message);
                    }
                }

            } while (!myTowers.IsComplete);

            return CompleteAction(myTowers);
        }

        private static int CompleteAction(Towers myTowers)
        {
            string input;

            if (myTowers.IsComplete == true)
            {
                Write($"It took you {myTowers.NumberOfMoves} moves. ");
                if (myTowers.NumberOfMoves == myTowers.MinimumPossibleMoves)
                {
                    WriteLine("Congrats! That's the minimum!");
                    WriteLine();
                    WriteLine();

                    ListRecord();
                    WriteLine();
                    WriteLine();

                    return AskForPlayAgain();
                }
                else
                {
                    // ask if the user want to try to solve the same problem again in the fewest possible moves.
                    WriteLine($"Not bad, but it can be done in {myTowers.MinimumPossibleMoves}.");
                    WriteLine();
                    WriteLine();

                    ListRecord();
                    WriteLine();
                    WriteLine();

                    do
                    {
                        Write("Want to try again ? (Y or N): ");
                        input = ReadKey().KeyChar.ToString().ToUpper();
                        if (input == "Y")
                        {
                            WriteLine();
                            return 2;   // mode 2: when the user tries again to solve the same problem in the fewest possible moves
                        }
                        else if (input != "Y" && input != "N")
                        {
                            WriteLine();
                            WriteLine("Invalid value. Valid values: 'Y', or 'N'");
                            WriteLine();
                        }
                    } while (input != "Y" && input != "N"); 

                    WriteLine();
                    WriteLine();
                    WriteLine();

                    return AskForPlayAgain();
                }
            }
            else
            {
                if (myRecord.Count != 0)
                {
                    WriteLine();
                    WriteLine();
                    ListRecord();
                }
                WriteLine();
                WriteLine();

                return AskForPlayAgain();
            }
        }

        private static int AskForPlayAgain()
        {
            string input;

            do
            {
                Write("Want to play next round? (Y or N): ");
                input = ReadKey().KeyChar.ToString().ToUpper();
                if (input == "Y")
                {
                    return 1;
                }
                else if (input == "N")
                {
                    WriteLine();
                    return 0;
                }
                else
                {
                    WriteLine();
                    WriteLine("Invalid value. Valid values: 'Y', or 'N'");
                    WriteLine();
                }
            } while (input != "Y" && input != "N");

            return -1;
        }

        private static void ListRecord()
        {
            string input;
            MoveRecord theRecord;

            do
            {
                Write("Would you like to see a list of the moves you made? (Y or N): ");
                input = ReadKey().KeyChar.ToString().ToUpper();
                WriteLine();

                if (input == "Y")
                {
                    WriteLine();
                    while (myRecord.Count > 0)
                    {
                        theRecord = myRecord.Dequeue();
                        WriteLine($" {theRecord.MoveNumber}. Disc {theRecord.Disc} moved from tower {theRecord.From} to tower {theRecord.To}.");
                    }
                }
                else if (input != "Y" && input != "N")
                {
                    WriteLine("Invalid value. Valid values: 'Y', or 'N'");
                    WriteLine();
                }
            } while (input != "Y" && input != "N");

        }

        static void GetTower(out Towers myTowers)
        {
            myTowers = null;
            string input;
            bool done = false;

            do
            {
                Write("How many discs in your tower (default is 5, max is 9): ");
                input = ReadKey().KeyChar.ToString();
                WriteLine();
                // default number is used when the user presses enter
                if (input == "\r")
                {
                    done = true;
                    WriteLine("Number of discs defaulting to 5. Press any key to continue. ");
                    ReadKey();
                    myTowers = new Towers(5);                    
                }
                else
                {
                    // check if the input is an integer or not
                    if (int.TryParse(input, out int numberOfDiscs))
                    {
                        try
                        {
                            myTowers = new Towers(numberOfDiscs);
                            done = true;
                        }
                        catch (InvalidHeightException e)
                        {
                            WriteLine(e.Message);
                            done = false;
                            WriteLine();
                        }
                    }
                    else
                    {
                        WriteLine("Oops, that input is invalid. Please try again.");
                        done = false;
                        WriteLine();
                    }
                }
            } while (!done);
        }
    }
}