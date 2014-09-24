﻿namespace KingsAndQueensHat
{
    public interface IPenalty
    {
        double ScorePenalty(TeamSet teamSet);

        /// <summary>
        /// How important this penalty category is
        /// </summary>
        double Weighting { get; }
    }
}