using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomMazeGenerator.Core
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
            return GetNeighboursAndWallStatus(maze, cell).Select(o => o.Neighbor);
        }

        public static IEnumerable<MazeCell> GetUnconnectedNeighbours(Maze maze, MazeCell cell)
        {
            return GetNeighboursAndWallStatus(maze, cell).Where(o => o.HasWall).Select(o => o.Neighbor);
        }

        public static IEnumerable<MazeCell> GetConnectedNeighbours(Maze maze, MazeCell cell)
        {
            return GetNeighboursAndWallStatus(maze, cell).Where(o => !o.HasWall).Select(o => o.Neighbor);
        }

        public static IEnumerable<(MazeCell Neighbor, bool HasWall)> GetNeighboursAndWallStatus(Maze maze, MazeCell cell)
        {
            if(cell.X > 0)
                yield return (maze.Cells[ToIndex(cell.X - 1, cell.Y, maze.Width)], cell.HasLeftWall);
            if(cell.Y > 0)
                yield return (maze.Cells[ToIndex(cell.X, cell.Y - 1, maze.Width)], cell.HasTopWall);
            if(cell.X < maze.Width - 1)
                yield return (maze.Cells[ToIndex(cell.X + 1, cell.Y, maze.Width)], cell.HasRightWall);
            if(cell.Y < maze.Height - 1)
                yield return (maze.Cells[ToIndex(cell.X, cell.Y + 1, maze.Width)], cell.HasBottomWall);
        }

        public static int ToIndex(int x, int y, int width)
        {
            return width * y + x;
        }

        public static bool HasWallInDirection(Maze maze, MazeCell currentCell, string direction)
        {
            switch(direction.ToLower())
            {
                case "left":
                    return currentCell.HasLeftWall;
                case "up":
                    return currentCell.HasTopWall;
                case "right":
                    return currentCell.HasRightWall;
                case "down":
                    return currentCell.HasBottomWall;
                default:
                    throw new ArgumentException($"Invalid direction: {direction}. Use 'left', 'up', 'right', or 'down'.");
            }
        }
        
        public static MazeCell GetNeighbour(Maze maze, MazeCell currentCell, string direction)
        {
            switch(direction.ToLower())
            {
                case "left":
                    if(currentCell.X > 0)
                        return maze.Cells[ToIndex(currentCell.X - 1, currentCell.Y, maze.Width)];
                    break;
                case "up":
                    if(currentCell.Y > 0)
                        return maze.Cells[ToIndex(currentCell.X, currentCell.Y - 1, maze.Width)];
                    break;
                case "right":
                    if(currentCell.X < maze.Width - 1)
                        return maze.Cells[ToIndex(currentCell.X + 1, currentCell.Y, maze.Width)];
                    break;
                case "down":
                    if(currentCell.Y < maze.Height - 1)
                        return maze.Cells[ToIndex(currentCell.X, currentCell.Y + 1, maze.Width)];
                    break;
            }
            return null;
        }
    }
}
