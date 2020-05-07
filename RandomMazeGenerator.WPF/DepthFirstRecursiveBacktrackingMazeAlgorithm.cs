using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RandomMazeGenerator.WPF
{

    public class DepthFirstRecursiveBacktrackingMazeAlgorithm
    {
     
        public async Task Generate(Maze maze, int stepDelayMillis, CancellationToken cancellationToken)
        {
            var random = new Random();

            var cellStack = new Stack<MazeCell>();
            cellStack.Push(maze.Cells[0]);

            while(cellStack.TryPop(out var currentCell))
            {
                currentCell.IsCurrent = true;
                currentCell.IsOnStack = false;

                var unvisitedNeighbours = GetNeighbours(maze, currentCell).Where(n => !n.HasBeenVisited).ToArray();
                if(unvisitedNeighbours.Length > 0)
                {
                    cellStack.Push(currentCell);
                    currentCell.IsOnStack = true;

                    var randomUnvisitedNeighbour = unvisitedNeighbours[random.Next(unvisitedNeighbours.Length)];
                    RemoveWallsBetween(currentCell, randomUnvisitedNeighbour);
                    randomUnvisitedNeighbour.HasBeenVisited = true;
                    cellStack.Push(randomUnvisitedNeighbour);
                    randomUnvisitedNeighbour.IsOnStack = true;
                }

                if(cancellationToken.IsCancellationRequested)
                    return;

                await Task.Delay(stepDelayMillis).ConfigureAwait(false);
                currentCell.IsCurrent = false;
            }
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
