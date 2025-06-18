using System.Collections.Generic;

namespace RandomMazeGenerator.Core
{
    public class KeepRightPathFindingAlgorithm : StepableMazeAlgorithmBase, IMazeSolvingAlgorithm
    {
        public const string Name = "Hug Right Wall Path Finding Algorithm";
        private readonly MazeCell _endCell;
        private MazeCell _previousCell;
        private readonly Maze _maze;


        public KeepRightPathFindingAlgorithm(Maze maze)
        {
            var startCell = maze.Cells[MazeOperations.ToIndex(maze.Width - 1, 0, maze.Width)];
            _endCell = maze.Cells[MazeOperations.ToIndex(0, maze.Height - 1, maze.Width)];

            CurrentBestPath = new List<MazeCell>();
            SetCurrentCell(startCell);
            _maze = maze;
        }

        public List<MazeCell> CurrentBestPath { get; }

        private readonly string[] _directionOrderFacingDown = { "left", "down", "right", "up" };
        private readonly string[] _directionOrderFacingRight = { "down", "right", "up", "left" };
        private readonly string[] _directionOrderFacingUp = { "right", "up", "left", "down" };
        private readonly string[] _directionOrderFacingLeft = { "up", "left", "down", "right" };

        protected override void Step()
        {
            if (!IsFinished)
            {
                CurrentBestPath.Add(CurrentCell);

                if (CurrentCell == _endCell)
                    Finish();

                string[] directionOrder;
                if (_previousCell == null || (_previousCell.X == CurrentCell.X && _previousCell.Y == CurrentCell.Y - 1)) // We're facing down
                {
                    directionOrder = _directionOrderFacingDown;
                }
                else if (_previousCell.X == CurrentCell.X && _previousCell.Y == CurrentCell.Y + 1) // We're facing up
                {
                    directionOrder = _directionOrderFacingUp;
                }
                else if (_previousCell.X == CurrentCell.X + 1 && _previousCell.Y == CurrentCell.Y) // We're facing left
                {
                    directionOrder = _directionOrderFacingLeft;
                }
                else  // We're facing right
                {
                    directionOrder = _directionOrderFacingRight;
                }

                foreach (var direction in directionOrder)
                {
                    if (!MazeOperations.HasWallInDirection(_maze, CurrentCell, direction) &&
                        MazeOperations.GetNeighbour(_maze, CurrentCell, direction) is MazeCell neighbour)
                    {
                        _previousCell = CurrentCell;
                        SetCurrentCell(neighbour);
                        break;
                    }
                }
            }
        }
    }
}