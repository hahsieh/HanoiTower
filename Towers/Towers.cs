using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class Towers
{
    private readonly Stack<int>[] threePoles = new Stack<int>[3];
    public int NumberOfDiscs { get; private set; }
    public int NumberOfMoves { get; private set; }

    // True if all discs have been successfully moved to the rightmost pole
    public bool IsComplete { get; private set; }

    public int MinimumPossibleMoves { get; set; }

    public Towers(Towers copied)
    {
        this.threePoles = new Stack<int>[3];           // ref data type
        this.threePoles[0] = new Stack<int>();
        for (int i = 0; i < 3; ++i)
        {
            this.threePoles[i] = new Stack<int>(copied.threePoles[i]);        // FILO
            this.threePoles[i] = new Stack<int>(this.threePoles[i]);
        }
        
        this.NumberOfDiscs = copied.NumberOfDiscs;
        this.NumberOfMoves = copied.NumberOfMoves;
        this.IsComplete = copied.IsComplete;
        this.MinimumPossibleMoves = copied.MinimumPossibleMoves;
    }
    public Towers(int numberOfDiscs)
    {
        if (numberOfDiscs > 0 && numberOfDiscs < 10)
        {
            this.NumberOfDiscs = numberOfDiscs;
            threePoles[0] = new Stack<int>();
            for (int i = numberOfDiscs; i > 0; i--)
            {
                threePoles[0].Push(i);
            }
            threePoles[1] = new Stack<int>();
            threePoles[2] = new Stack<int>();
            NumberOfMoves = 0;
            IsComplete = false;
            MinimumPossibleMoves = (int)Math.Pow(2, NumberOfDiscs) - 1;
        }
        else
        {
            throw new InvalidHeightException("Oops. Number of discs must be greater than 0 and less than 10.");
        }
    }

    public MoveRecord Move(int from, int to)
    {
        MoveRecord theRecord;
        int theDisc;

        if (from < 1 || from > 3)
        {
            throw new InvalidMoveException("Invalid tower value. Valid values for 'From': '1', '2', '3', or 'x' ");
        }
        if (to < 1 || to > 3)
        {
            throw new InvalidMoveException("Invalid tower value. Valid values for 'To': '1', '2', '3', or enter");
        }
        if (from == to)
        {
            throw new InvalidMoveException("Move cancelled.");
        }
        if (threePoles[from - 1].Count == 0)
        {
            throw new InvalidMoveException($"Tower {from} is empty.");
        }
        if (threePoles[to - 1].Count != 0 && threePoles[from - 1].Peek() > threePoles[to - 1].Peek())
        {
            throw new InvalidMoveException($"Top disc of tower {from} is larger than top disc on tower {to}");
        }
        else
        {
            theDisc = threePoles[from - 1].Pop();
            threePoles[to - 1].Push(theDisc);
            NumberOfMoves++;
            theRecord = new MoveRecord(NumberOfMoves, theDisc, from, to, new Towers(this));
            if (threePoles[0].Count == 0 && threePoles[1].Count == 0)
            {
                IsComplete = true;
            }
        }
        return theRecord;
    }


    public int[][] ToArray()
    {
        int[][] towers = new int[3][];
        for (int i = 0; i < threePoles.Length; i++)
        {
            towers[i] = threePoles[i].ToArray();
        }

        return towers;
    }
}


[Serializable]
public class InvalidMoveException : Exception
{
    public InvalidMoveException()
    {
    }

    public InvalidMoveException(string message) : base(message)
    {
    }

    public InvalidMoveException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected InvalidMoveException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class InvalidHeightException : Exception
{
    public InvalidHeightException()
    {
    }

    public InvalidHeightException(string message) : base(message)
    {
    }

    public InvalidHeightException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected InvalidHeightException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}