public class MoveRecord
{
    public int MoveNumber;
    public int Disc;
    public int From;
    public int To;
    public Towers TowerState;    // A copy of the Towers object (all three stacks) as of the end of the move.

    public MoveRecord(int moveNumber, int disc, int from, int to, Towers towers)
    {
        this.MoveNumber = moveNumber;
        this.Disc = disc;
        this.From = from;
        this.To = to;
        this.TowerState = towers;
    }
}
