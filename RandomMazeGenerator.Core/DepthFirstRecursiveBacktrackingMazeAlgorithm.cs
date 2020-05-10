using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomMazeGenerator.WPF
{
    public class DepthFirstRecursiveBacktrackingMazeAlgorithm
    {
        private readonly Random _random;
        private readonly Maze _maze;
        private readonly Stack<MazeCell> cellStack;
        private MazeCell _currentCell;
        private bool _finished;

        public DepthFirstRecursiveBacktrackingMazeAlgorithm(Maze maze)
        {
            _random = new Random();
            _maze = maze;
            cellStack = new Stack<MazeCell>();

            var startCell = maze.Cells[0];
            cellStack.Push(startCell);
        }

        public async Task Run(int updateDelayMillis, int stepsPerUpdate)
        {
            while(!_finished)
            {
                Step(stepsPerUpdate);
                await Task.Delay(updateDelayMillis);
            }
        }

        public void Step(int steps = 1)
        {
            for(int i = 0;i < steps;i++)
            {
                if(cellStack.Count > 0)
                {
                    var currentCell = cellStack.Pop();
                    SetCurrentCell(currentCell);
                    currentCell.IsOnStack = false;

                    var unvisitedNeighbours = GetNeighbours(_maze, currentCell).Where(n => !n.HasBeenVisited).ToArray();
                    if(unvisitedNeighbours.Length > 0)
                    {
                        cellStack.Push(currentCell);
                        currentCell.IsOnStack = true;

                        var randomUnvisitedNeighbour = unvisitedNeighbours[_random.Next(unvisitedNeighbours.Length)];
                        RemoveWallsBetween(currentCell, randomUnvisitedNeighbour);
                        randomUnvisitedNeighbour.HasBeenVisited = true;
                        cellStack.Push(randomUnvisitedNeighbour);
                        randomUnvisitedNeighbour.IsOnStack = true;
                    }
                }
                else
                {
                    SetCurrentCell(null);
                    _finished = true;
                }
            }
        }

        private void SetCurrentCell(MazeCell cell)
        {
            if(_currentCell != null)
                _currentCell.IsCurrent = false;
            if(cell != null)
                cell.IsCurrent = true;
            _currentCell = cell;
        }

        private void RemoveWallsBetween(MazeCell cell1, MazeCell cell2)
        {
            if(cell1.X < cell2.X)
            {
                cell1.HasRightWall = false;
                cell2.HasLeftWall = false;
            }
            else if(cell1.X > cell2.X)
            {
                cell1.HasLeftWall = false;
                cell2.HasRightWall = false;
            }else if(cell1.Y < cell2.Y)
            {
                cell1.HasBottomWall = false;
                cell2.HasTopWall = false;
            }else if(cell1.Y > cell2.Y)
            {
                cell1.HasTopWall = false;
                cell2.HasBottomWall = false;
            }
        }

        private IEnumerable<MazeCell> GetNeighbours(Maze maze, MazeCell currentCell)
        {
            if(currentCell.X > 0)
                yield return maze.Cells[ToIndex(currentCell.X-1, currentCell.Y, maze.Width)];
            if(currentCell.Y > 0)
                yield return maze.Cells[ToIndex(currentCell.X, currentCell.Y-1, maze.Width)];
            if(currentCell.X < maze.Width-1)
                yield return maze.Cells[ToIndex(currentCell.X+1, currentCell.Y, maze.Width)];
            if(currentCell.Y < maze.Height-1)
                yield return maze.Cells[ToIndex(currentCell.X, currentCell.Y+1, maze.Width)];
        }

        private static int ToIndex(int x, int y, int width)
        {
            return (width * y) + x;
        }
    }
}
