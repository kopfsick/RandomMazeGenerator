using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomMazeGenerator.WPF
{

    public class AStarPathFindingAlgorithm : StepableMazeAlgorithmBase
    {
        private HashSet<MazeCell> _openSet;
        private Dictionary<MazeCell, MazeCell> _cameFrom;
        private Dictionary<MazeCell, int> _gScore;
        private Dictionary<MazeCell, int> _fScore;
        private MazeCell _startCell;
        private MazeCell _endCell;
        private List<MazeCell> _bestPath;
        private readonly Maze _maze;

        public bool IsOnBestPath(MazeCell cell) => _bestPath?.Contains(cell) ?? false;
        public bool IsInOpenSet(MazeCell cell) => _openSet?.Contains(cell) ?? false;

        public AStarPathFindingAlgorithm(Maze maze)
        {
            _startCell = maze.Cells[MazeOperations.ToIndex(0,0, maze.Width)];
            _endCell = maze.Cells[MazeOperations.ToIndex(maze.Width-1,maze.Height-1, maze.Width)];

            _startCell = maze.Cells[MazeOperations.ToIndex(maze.Width-1,0, maze.Width)];
            _endCell = maze.Cells[MazeOperations.ToIndex(0,maze.Height-1, maze.Width)];

            _openSet = new HashSet<MazeCell>();
            _cameFrom = new Dictionary<MazeCell, MazeCell>();
            _gScore = new Dictionary<MazeCell, int>();
            _fScore = new Dictionary<MazeCell, int>();

            _openSet.Add(_startCell);

            _gScore[_startCell] = 0;
            _fScore[_startCell] = 0;
            _maze = maze;
        }

        private int CalculateHeuristic(MazeCell cell)
        {
            return Math.Abs(cell.X-_endCell.X) + Math.Abs(cell.Y-_endCell.Y);
        }

        protected override void Step()
        {
            if(_openSet.Count > 0)
            {
                var currentCell = GetCellWithLowestFScoreInOpenSet();
                SetCurrentCell(currentCell);
                if(currentCell == _endCell)
                {
                    // fant den. Return reconstruct?
                    Finish();
                }
                else
                {
                    _openSet.Remove(currentCell);
                    foreach(var neighbour in MazeOperations.GetConnectedNeighbours(_maze, currentCell))
                    {
                        var tentativeGScore = GetGScore(currentCell) + 1;
                        if(tentativeGScore < GetGScore(neighbour))
                        {
                            // This path to neighbor is better than any previous one. Record it!
                            _cameFrom[neighbour] = currentCell;
                            _gScore[neighbour] = tentativeGScore;
                            _fScore[neighbour] = GetGScore(neighbour) + CalculateHeuristic(neighbour);
                            if(!_openSet.Contains(neighbour))
                                _openSet.Add(neighbour);
                        }
                    }
                }

                _bestPath = CreateCurrentBestPath(currentCell);
            }
            else
            {
                Finish();
                SetCurrentCell(null);
            }
        }

        private List<MazeCell> CreateCurrentBestPath(MazeCell currentCell)
        {
            var bestPath = new List<MazeCell> { currentCell };

            var temp = currentCell;
            while(_cameFrom.TryGetValue(temp, out var previous))
            {
                bestPath.Add(previous);
                temp = previous;
            }

            return bestPath;
        }

        private int GetGScore(MazeCell currentCell)
        {
            return _gScore.TryGetValue(currentCell, out var gScore) ? gScore : int.MaxValue;
        }

        private int GetFScore(MazeCell currentCell)
        {
            return _fScore.TryGetValue(currentCell, out var fScore) ? fScore : int.MaxValue;
        }

        private MazeCell GetCellWithLowestFScoreInOpenSet()
        {
            return _openSet.OrderBy(GetFScore).First();
        }
    }
}
