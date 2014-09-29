using KingsAndQueensHat.Model;
using System.Collections.Generic;

namespace KingsAndQueensHat.TeamGeneration
{
    public interface IPenalty
    {
        double ScorePenalty(List<Team> teamSet);

        /// <summary>
        /// How important this penalty category is
        /// </summary>
        double Weighting { get; }
    }
}