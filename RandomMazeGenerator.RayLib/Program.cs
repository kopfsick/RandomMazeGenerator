using System.Numerics;
using DotnetNoise;
using Kopfsick.CreativeCoding.Algorithms;
using RandomMazeGenerator.Core;
using Raylib_cs;

var noise = new FastNoise(97617986);

Raylib.InitWindow(1200, 1000, "Maze Generator");
//Raylib.SetTargetFPS(60);
//Raylib.ToggleBorderlessWindowed();

var stepsPerSeconds = 2000;
var stepsPerSecondsSolving = 400;
var maze = new Maze(100);

var solvingAlgorithm = new AStarPathFindingAlgorithm(maze);
IStepableAlgorithm algorithm = new DepthFirstRecursiveBacktrackingMazeAlgorithm(maze);
//algorithm = new RandomizedPrimsMazeAlgorithm(maze);
var visualizeStack = false;

var isPaused = false;

float cumulativeStepsToUpdate = 0;
float cumulativeStepsToUpdateSolving = 0;

while (!Raylib.WindowShouldClose())
{
    var frameTime = Raylib.GetFrameTime();
    var totalTime = (float)Raylib.GetTime();
    //Calc sizes
    var width = Raylib.GetScreenWidth();
    var height = Raylib.GetScreenHeight();
    var uiWidthPixels = 200;
    var mazeWidthPixels = width - uiWidthPixels;
    var cellWidth = mazeWidthPixels / maze.Width;
    var cellHeight = height / maze.Height;
    var halfCellWidth = cellWidth / 2;
    var halfCellHeight = cellHeight / 2;
    var cellSize = new Vector2(cellWidth, cellHeight);
    
    // Update
    cumulativeStepsToUpdate += stepsPerSeconds * frameTime;
    cumulativeStepsToUpdateSolving += stepsPerSecondsSolving * frameTime;
    var stepsToUpdate = (int)cumulativeStepsToUpdate;
    var stepsToUpdateSolving = (int)cumulativeStepsToUpdateSolving;
    cumulativeStepsToUpdate -= stepsToUpdate;
    cumulativeStepsToUpdateSolving -= stepsToUpdateSolving;
    
    if (!algorithm.IsFinished)
        algorithm.Step(stepsToUpdate);
    else if (!solvingAlgorithm.IsFinished)
        solvingAlgorithm.Step(stepsToUpdateSolving+1);

    // Draw
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);
    Raylib.DrawFPS(width-uiWidthPixels, 10);

    foreach (var cell in maze.Cells)
    {
        var topY = cell.Y * cellHeight;
        var bottomY = (cell.Y + 1) * cellHeight;
        
        var leftX = cell.X * cellWidth;
        var rightX = (cell.X + 1) * cellWidth;
        
        var topLeftCorner = new Vector2(leftX, topY);
        var bottomRightCorner = new Vector2(rightX, bottomY);
        var topRightCorner = new Vector2(rightX, topY);
        var bottomLeftCorner = new Vector2(leftX, bottomY);
        var center = new Vector2(leftX + halfCellWidth, topY + halfCellHeight);
        var midLeft = new Vector2(leftX, topY + halfCellHeight);
        var midRight = new Vector2(rightX, topY + halfCellHeight);
        var midTop = new Vector2(leftX + halfCellWidth, topY);
        var midBottom = new Vector2(leftX + halfCellWidth, bottomY);
        var cellRectangle = new Rectangle(leftX, topY, cellWidth, cellHeight);
        
        //Colors
        var noiseValue = noise.GetPerlin(cell.X*2, cell.Y*2, totalTime*50);
        var noise0to1 = Raymath.Remap(noiseValue, -1,1, 0, 1);
        var wallColor = new Color((int)(noise0to1 * 254),0, 255);
        var solvedPathColor = new Color(255,128+(int)(noise0to1 * 127), 0);
        
        if (cell.IsCurrent && (!algorithm.IsFinished || !solvingAlgorithm.IsFinished)) 
            Raylib.DrawRectangleRec(cellRectangle, Color.Orange);

        if (cell.HasBeenVisited)
        {
            if (cell.HasLeftWall) 
                Raylib.DrawLineEx(topLeftCorner, bottomLeftCorner,2, wallColor);

            if (cell.HasTopWall) 
                Raylib.DrawLineEx(topLeftCorner, topRightCorner,2, wallColor);

            if (cell.HasRightWall) 
                Raylib.DrawLineEx(topRightCorner, bottomRightCorner,2, wallColor);

            if (cell.HasBottomWall) 
                Raylib.DrawLineEx(bottomLeftCorner, bottomRightCorner,2, wallColor);
        }
        else
        {
            Raylib.DrawRectangleLinesEx(cellRectangle, 1, wallColor);
            //Raylib.DrawRectangleRec(cellRectangle, Color.Blue);
        }
        
        if (cell.IsOnStack && visualizeStack)
        {
            var radius = cellWidth / 4;
            Raylib.DrawCircleV(center, radius, Color.Yellow);
        }
    }

    MazeCell previousCell = null;
    foreach (var currentBestCell in solvingAlgorithm.CurrentBestPath)
    {
        if (previousCell == null)
        {
            previousCell = currentBestCell;
            continue;
        }
        
        var centerCurrent = new Vector2(
            (currentBestCell.X * cellWidth) + halfCellWidth,
            (currentBestCell.Y * cellHeight) + halfCellHeight);
        var centerPrevious = new Vector2(
            (previousCell.X * cellWidth) + halfCellWidth,
            (previousCell.Y * cellHeight) + halfCellHeight);
        
        Raylib.DrawLineEx(centerCurrent, centerPrevious,1, Color.Yellow);
        previousCell = currentBestCell;
    }

    Raylib.EndDrawing();
}

Raylib.CloseWindow();