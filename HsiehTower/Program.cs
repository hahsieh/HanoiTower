using System;
using static System.Console;
using static TowerUtilities;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;

namespace TowersUI
{
    class Program
    {
        static readonly AVL<int, MoveRecord> moves = new AVL<int, MoveRecord> { };          
        static readonly Stack<MoveRecord> undoRecord = new Stack<MoveRecord> { };
        static readonly Stack<MoveRecord> redoRecord = new Stack<MoveRecord> { };        
        const string CtrlZ = "\u001a";
        const string CtrlY = "\u0019";

        static void Main(string[] args)
        {
            Towers myTowers;
            int mode;

            GetTower(out myTowers);
            moves.Insert(0, new MoveRecord(0, 0, 0, 0, new Towers(myTowers)));
            
            Console.Clear();
            DisplayTowers(myTowers);

            mode = SolveOption(myTowers);

            while (mode != 0)
            {
                moves.Clear();
                undoRecord.Clear();
                redoRecord.Clear();
                if (mode == 1)
                {
                    Console.Clear();
                    GetTower(out myTowers);
                    moves.Insert(0, new MoveRecord(0, 0, 0, 0, new Towers(myTowers)));
                }
                else if (mode == 2)
                {
                    myTowers = new Towers(myTowers.NumberOfDiscs);
                    moves.Insert(0, new MoveRecord(0, 0, 0, 0, new Towers(myTowers)));
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
                WriteLine("- S – Auto-solve step-by-step");
                WriteLine();
                WriteLine();
                Write("Choose an approach: ");
                input = ReadKey().KeyChar.ToString().ToUpper();
                WriteLine();
                if (input == "M")
                {
                    return Play(myTowers);
                }
                else if (input == "A" || input == "S")
                {
                    Console.Clear();
                    DisplayTowers(myTowers);
                    WriteLine();
                    if (input == "A")
                    {
                        WriteLine("Press a key and watch closely!");
                    }
                    else
                    {
                        WriteLine("Press a key to see the first move.");
                    }
                    ReadKey();
                    return AutoPlay(myTowers, input);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    WriteLine("Invalid value. Valid values: 'M', 'A, 'or 'S' ");
                    Console.ResetColor();                    
                }
            } while (input != "M" && input != "A");
            return -1;
        }

        private static int AutoPlay(Towers myTowers, string input)
        {
            int from = 1;
            int to = 3;
            int other = 2;
            string algorithm = input;

            if (algorithm == "A")
            {
                Recursive(myTowers, myTowers.NumberOfDiscs, from, to, other);
            }
            else
            {
                Iterative(myTowers, myTowers.NumberOfDiscs, from, to, other);
                WriteLine();
                WriteLine();
                if (myTowers.IsComplete == false)
                {
                    Write("Step-through aborted. ");
                }
                else
                {
                    Write("Step-through completed. ");
                }
            }
            WriteLine($"Number of moves: {myTowers.NumberOfMoves}");
            WriteLine();
            
            PostGameReview(myTowers);
            WriteLine();
            WriteLine();

            return AskForPlayAgain();
        }

        private static void Iterative(Towers myTowers, int numberOfDiscs, int from, int to, int other)
        {
            MoveRecord theRecord = null;
            int temp;

            if (numberOfDiscs % 2 == 0)
            {
                temp = to;
                to = other;
                other = temp;
            }
            for (int i = 1; i <= myTowers.MinimumPossibleMoves; i++)
            {
                if (i % 3 == 1)
                {
                    try
                    {
                        theRecord = myTowers.Move(from, to);
                    }
                    catch (InvalidMoveException e)
                    {

                        theRecord = myTowers.Move(to, from);
                    }
                    moves.Insert(myTowers.NumberOfMoves, theRecord);
                }
                else if (i % 3 == 2)
                {
                    try
                    {
                        theRecord = myTowers.Move(from, other);
                    }
                    catch (InvalidMoveException e)
                    {

                        theRecord = myTowers.Move(other, from);
                    }
                    moves.Insert(myTowers.NumberOfMoves, theRecord);
                }
                else if (i % 3 == 0)
                {
                    try
                    {
                        theRecord = myTowers.Move(to, other);
                    }
                    catch (InvalidMoveException e)
                    {

                        theRecord = myTowers.Move(other, to);
                    }
                    moves.Insert(myTowers.NumberOfMoves, theRecord);
                }

                Console.Clear();
                DisplayTowers(myTowers);
                WriteLine($"\nMove {myTowers.NumberOfMoves} complete. Successfully moved disc from pole {theRecord.From} to pole {theRecord.To}");
                if (myTowers.IsComplete == false)
                {
                    WriteLine();
                    Write("Press any key to move to the next step or \"x\" to exit. ");
                    if (ReadKey().KeyChar.ToString().ToUpper() == "X")
                    {
                        break;
                    }
                }
            }
        }

        private static void Recursive(Towers myTowers, int n, int from, int to, int other)
        {
            MoveRecord theRecord;
            if (n == 1)
            {
                theRecord = myTowers.Move(from, to);
                moves.Insert(myTowers.NumberOfMoves, theRecord);
                Console.Clear();
                DisplayTowers(myTowers);
                WriteLine($"\nMove {myTowers.NumberOfMoves} complete. Successfully moved disc from pole {from} to pole {to}");
                Thread.Sleep(250);
                return;
            }

            Recursive(myTowers, n - 1, from, other, to);
            theRecord = myTowers.Move(from, to);
            moves.Insert(myTowers.NumberOfMoves, theRecord);
            Console.Clear();
            DisplayTowers(myTowers);
            WriteLine($"\nMove {myTowers.NumberOfMoves} complete. Successfully moved disc from pole {from} to pole {to}");
            Thread.Sleep(250);
            Recursive(myTowers, n - 1, other, to, from);
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
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        if (redoRecord.Count == 0)
                        {
                            WriteLine("Invalid value. Valid values for 'From': '1', '2', '3', 'Ctrl-z', or 'x' ");
                        }
                        else
                        {
                            WriteLine("Invalid value. Valid values for 'From': '1', '2', '3', 'Ctrl-z', 'Ctrl-y' or 'x' ");
                        }
                        Console.ResetColor();                        
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
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    if (redoRecord.Count == 0)
                    {
                        WriteLine("Invalid value. Valid values for 'From': '1', '2', '3', 'Ctrl-z', or 'x' ");
                    }
                    else
                    {
                        WriteLine("Invalid value. Valid values for 'From': '1', '2', '3', 'Ctrl-z', 'Ctrl-y' or 'x' ");
                    }
                    Console.ResetColor();
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
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        WriteLine("Invalid tower value. Valid values for 'To': '1', '2', '3', or enter");
                        Console.ResetColor();
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
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    WriteLine("Invalid tower value. Valid values for 'To': '1', '2', '3', or enter");
                    Console.ResetColor();
                    foundTo = false;
                }
            } while (!foundTo);

            return 0;
        }

        private static void Redo(Towers myTowers)
        {
            MoveRecord theRecord;
            MoveRecord nextRecord;
            if (redoRecord.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                WriteLine("Invalid value. Valid values for 'From': '1', '2', '3', 'Ctrl-z', or 'x' ");
                Console.ResetColor();                 
            }
            else
            {
                theRecord = redoRecord.Pop();
                undoRecord.Push(theRecord);
                nextRecord = myTowers.Move(theRecord.From, theRecord.To);
                moves.Insert(myTowers.NumberOfMoves, nextRecord);
                Console.Clear();
                DisplayTowers(myTowers);
                WriteLine($"\nMove {myTowers.NumberOfMoves} complete by redo of move {theRecord.MoveNumber}. Disc {theRecord.Disc} returned to pole {theRecord.To} from pole {theRecord.From}");
            }
        }

        private static void Undo(Towers myTowers)
        {
            MoveRecord theRecord;
            MoveRecord nextRecord; 
            if (undoRecord.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                WriteLine("Can’t undo");
                Console.ResetColor();                
            }
            else
            {
                theRecord = undoRecord.Pop();
                redoRecord.Push(theRecord);
                nextRecord = myTowers.Move(theRecord.To, theRecord.From);
                moves.Insert(myTowers.NumberOfMoves, nextRecord);
                Console.Clear();
                DisplayTowers(myTowers);
                WriteLine($"\nMove {myTowers.NumberOfMoves} complete by undo of move {theRecord.MoveNumber}. Disc {theRecord.Disc} restored to pole {theRecord.From} from pole {theRecord.To}");
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
                        moves.Insert(theRecord.MoveNumber, theRecord);    
                        undoRecord.Push(theRecord);
                        redoRecord.Clear();
                        Console.Clear();
                        DisplayTowers(myTowers);
                        WriteLine($"\nMove {myTowers.NumberOfMoves} complete. Successfully moved disc from pole {from} to pole {to}");
                    }
                    catch (InvalidMoveException e)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        WriteLine(e.Message);
                        Console.ResetColor();                        
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
                    
                    PostGameReview(myTowers);
                    WriteLine();
                    WriteLine();

                    return AskForPlayAgain();
                }
                else
                {
                    // ask if the user want to try to solve the same problem again in the fewest possible moves.
                    WriteLine($"Not bad, but it can be done in {myTowers.MinimumPossibleMoves}.");
                    WriteLine();
                    
                    PostGameReview(myTowers);
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
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            WriteLine("Invalid value. Valid values: 'Y', or 'N'");
                            Console.ResetColor();                            
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
                if (moves.Root != null)
                {
                    WriteLine();
                    PostGameReview(myTowers);
                }
                WriteLine();
                WriteLine();

                return AskForPlayAgain();
            }
        }

        private static void PostGameReview(Towers myTowers)
        {
            string input;
            int moveNum = 0;
            bool done = false;
            MoveRecord theRecord = null;

            do
            {
                WriteLine(); 
                WriteLine("Post-game review:");
                WriteLine();
                WriteLine("- L – List moves");
                WriteLine("- R - Replay");
                WriteLine("- B – Replay backwards");
                WriteLine("- F – Find result of a specific move");
                WriteLine();
                WriteLine("- X – Exit post-game review");
                WriteLine();
                Write("Choose an option: ");
                input = ReadKey().KeyChar.ToString().ToUpper();
                WriteLine();
                switch (input)
                {
                    case "L":
                        Console.Clear();
                        WriteLine();
                        moves.Traverse(x =>
                        {
                            if (x.MoveNumber == 0) return;
                            WriteLine($" Move {x.MoveNumber}: Disc {x.Disc} was moved from pole {x.From} to pole {x.To}.");
                        });                        
                        break;
                    case "R":
                        Console.Clear();
                        WriteLine("<<< REPLAY >>>");
                        moves.Traverse(MoveWithStateDisplay);
                        break;
                    case "B":
                        Console.Clear();
                        WriteLine("<<< REPLAY >>>");
                        moves.TraverseReverse(MoveWithStateDisplay);
                        break;
                    case "F":
                        WriteLine();
                        do
                        {
                            Write("Enter move number (Please press 'enter' key after entering the value): ");
                            if (Int32.TryParse(ReadLine(), out moveNum))
                            {
                                done = true;                                 
                                theRecord = moves.Find(moveNum);
                                if (theRecord == null)
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                    WriteLine("No such move.");
                                    Console.ResetColor(); 
                                }
                                else
                                {
                                    Console.Clear();
                                    DisplayTowers(theRecord.TowerState);
                                    WriteLine();
                                    WriteLine($"In move {theRecord.MoveNumber}, disc {theRecord.Disc} was moved from pole {theRecord.From} to pole {theRecord.To}.");
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                WriteLine("Invalid value. Values should be integers.");
                                Console.ResetColor(); 
                            }
                        } while (!done); 
                        
                        break;
                    case "X":
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        WriteLine("Invalid command.");
                        Console.ResetColor(); 
                        break;
                }
            } while (input != "X");
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
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    WriteLine("Invalid value. Valid values: 'Y', or 'N'");
                    Console.ResetColor();
                    WriteLine();
                }
            } while (input != "Y" && input != "N");

            return -1;
        }

        private static void MoveWithStateDisplay(MoveRecord theRecord)
        {
            DisplayTowers(theRecord.TowerState);
            WriteLine();
            if (theRecord.MoveNumber == 0)
            {
                Thread.Sleep(250);
                return;
            } 
            WriteLine($" Move {theRecord.MoveNumber}: Disc {theRecord.Disc} was moved from pole {theRecord.From} to pole {theRecord.To}.");
            Thread.Sleep(250);
        }

        static void GetTower(out Towers myTowers)
        {
            myTowers = null;
            string input;
            bool done = false;

            do
            {
                WriteLine();
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
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            WriteLine(e.Message);
                            Console.ResetColor();
                            done = false;
                            WriteLine();
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        WriteLine("Oops, that input is invalid. Please try again.");
                        Console.ResetColor(); 
                        done = false;
                        WriteLine();
                    }
                }
            } while (!done);
        }
    }
}