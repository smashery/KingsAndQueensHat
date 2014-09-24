namespace KingsAndQueensHat
{
    public interface IPenalty
    {
        int ScorePenalty(TeamSet teamSet);

        /// <summary>
        /// How important this penalty category is
        /// </summary>
        double Weighting { get; }
    }
}