using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomMazeGenerator.Core
{
    public class DepthFirstRecursiveBacktrackingMazeAlgorithm : StepableMazeAlgorithmBase
    {
        public const string Name = "Depth First Recursive Backtracking";
        
        private readonly Random _random;
        private readonly Maze _maze;
        private readonly Stack<MazeCell> cellStack;

        public DepthFirstRecursiveBacktrackingMazeAlgorithm(Maze maze)
        {
            _random = new Random();
            _maze = maze;
            cellStack = new Stack<MazeCell>();

            var startCell = maze.Cells[0];
            cellStack.Push(startCell);
        }

        protected override void Step()
        {
            if(cellStack.Count > 0)
            {
                var currentCell = cellStack.Pop();
                SetCurrentCell(currentCell);
                currentCell.IsOnStack = false;

                var unvisitedNeighbours = MazeOperations.GetNeighbours(_maze, currentCell).Where(n => !n.HasBeenVisited).ToArray();
                if(unvisitedNeighbours.Length > 0)
                {
                    cellStack.Push(currentCell);
                    currentCell.IsOnStack = true;

                    var randomUnvisitedNeighbour = unvisitedNeighbours[_random.Next(unvisitedNeighbours.Length)];
                    MazeOperations.RemoveWallsBetween(currentCell, randomUnvisitedNeighbour);
                    randomUnvisitedNeighbour.HasBeenVisited = true;
                    cellStack.Push(randomUnvisitedNeighbour);
                    randomUnvisitedNeighbour.IsOnStack = true;
                }
            }
            else
            {
                SetCurrentCell(null);
                Finish();
            }
        }
    }
}
