public class MoveRecord
{
    public int MoveNumber;
    public int Disc;
    public int From;
    public int To;

    public MoveRecord(int moveNumber, int disc, int from, int to)
    {
        this.MoveNumber = moveNumber;
        this.Disc = disc;
        this.From = from;
        this.To = to;
    }
}
