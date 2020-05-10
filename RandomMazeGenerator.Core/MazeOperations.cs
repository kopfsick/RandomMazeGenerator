using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomMazeGenerator.WPF
{

    public static class MazeOperations
    {
        public static void RemoveWallsBetween(MazeCell cell1, MazeCell cell2)
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
            }
            else if(cell1.Y < cell2.Y)
            {
                cell1.HasBottomWall = false;
                cell2.HasTopWall = false;
            }
            else if(cell1.Y > cell2.Y)
            {
                cell1.HasTopWall = false;
                cell2.HasBottomWall = false;
            }
        }

        public static IEnumerable<MazeCell> GetNeighbours(Maze maze, MazeCell cell)
        {
            if(cell.X > 0)
                yield return maze.Cells[ToIndex(cell.X - 1, cell.Y, maze.Width)];
            if(cell.Y > 0)
                yield return maze.Cells[ToIndex(cell.X, cell.Y - 1, maze.Width)];
            if(cell.X < maze.Width - 1)
                yield return maze.Cells[ToIndex(cell.X + 1, cell.Y, maze.Width)];
            if(cell.Y < maze.Height - 1)
                yield return maze.Cells[ToIndex(cell.X, cell.Y + 1, maze.Width)];
        }

        public static IEnumerable<MazeCell> GetUnconnectedNeighbours(Maze maze, MazeCell cell)
        {
            if(cell.X > 0 && cell.HasLeftWall)
                yield return maze.Cells[ToIndex(cell.X - 1, cell.Y, maze.Width)];
            if(cell.Y > 0 && cell.HasTopWall)
                yield return maze.Cells[ToIndex(cell.X, cell.Y - 1, maze.Width)];
            if(cell.X < maze.Width - 1 && cell.HasRightWall)
                yield return maze.Cells[ToIndex(cell.X + 1, cell.Y, maze.Width)];
            if(cell.Y < maze.Height - 1 && cell.HasBottomWall)
                yield return maze.Cells[ToIndex(cell.X, cell.Y + 1, maze.Width)];
        }

        public static int ToIndex(int x, int y, int width)
        {
            return (width * y) + x;
        }
    }
}
