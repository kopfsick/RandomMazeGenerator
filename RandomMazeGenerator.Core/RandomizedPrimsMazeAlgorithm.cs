using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomMazeGenerator.WPF
{
    public class RandomizedPrimsMazeAlgorithm : StepableAlgorithmBase
    {
        private readonly Random _random;
        private readonly Maze _maze;
        private readonly List<(MazeCell, MazeCell)> _adjacentCells;
        private MazeCell _currentCell;

        public RandomizedPrimsMazeAlgorithm(Maze maze)
        {
            _random = new Random();
            _maze = maze;

            _adjacentCells = new List<(MazeCell, MazeCell)>();

            var startCell = maze.Cells[0];

            var neighbors = MazeOperations.GetNeighbours(_maze, startCell);
            _adjacentCells.AddRange(neighbors.Select(neighbour => (startCell, neighbour)));
            startCell.HasBeenVisited = true;
        }

        protected override void Step()
        {
            if(_adjacentCells.Count > 0)
            {
                var randomChosen = _adjacentCells[_random.Next(_adjacentCells.Count)];
                SetCurrentCell(randomChosen.Item1);
                if(randomChosen.Item1.HasBeenVisited ^ randomChosen.Item2.HasBeenVisited)
                {
                    MazeOperations.RemoveWallsBetween(randomChosen.Item1, randomChosen.Item2);
                    var unvisitedCell = randomChosen.Item1.HasBeenVisited ? randomChosen.Item2 : randomChosen.Item1;

                    unvisitedCell.HasBeenVisited = true;
                    _adjacentCells.AddRange(MazeOperations.GetUnconnectedNeighbours(_maze, unvisitedCell).Select(neighbour => (unvisitedCell, neighbour)));
                }
                _adjacentCells.Remove(randomChosen);
            }
            else
            {
                SetCurrentCell(null);
                Finish();
            }
        }

        public void SetCurrentCell(MazeCell cell)
        {
            if(_currentCell != null)
                _currentCell.IsCurrent = false;
            if(cell != null)
                cell.IsCurrent = true;
            _currentCell = cell;
        }
    }
}
