using System.Collections.Generic;
using Kopfsick.CreativeCoding.Algorithms;

namespace RandomMazeGenerator.Core
{
    public interface IMazeSolvingAlgorithm : IStepableAlgorithm
    {
        List<MazeCell> CurrentBestPath { get; }
    }
}