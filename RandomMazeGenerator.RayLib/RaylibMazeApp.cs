using System.Numerics;
using DotnetNoise;
using ImGuiNET;
using Kopfsick.CreativeCoding.Algorithms;
using RandomMazeGenerator.Core;
using Raylib_cs;
using rlImGui_cs;

namespace RandomMazeGenerator.RayLib;

public class RaylibMazeApp
{
    private MazeAppState? AppState { get; set; }
    private MazeRunSettings MazeRunSettings { get; set; }
    
    public void Run()
    {
        MazeRunSettings = new MazeRunSettings();
        InitializeAppState(MazeRunSettings);
        
        Raylib.InitWindow(1200, 1000, "Maze Generator");
        rlImGui.Setup();
        //Raylib.SetTargetFPS(60);

        var noise = new FastNoise(97617986);

        RunGameLoop(noise);

        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }

    private void RunGameLoop(FastNoise noise)
    {
        while (!Raylib.WindowShouldClose())
        {
            var frameTime = Raylib.GetFrameTime();
            var totalTime = (float)Raylib.GetTime();
            //Calc sizes
            var width = Raylib.GetScreenWidth();
            var height = Raylib.GetScreenHeight();
            var uiWidthPixels = 200;
            var mazeWidthPixels = width - uiWidthPixels;
            var cellWidth = mazeWidthPixels / AppState.Maze.Width;
            var cellHeight = height / AppState.Maze.Height;
            var halfCellWidth = cellWidth / 2;
            var halfCellHeight = cellHeight / 2;

            // Update
            AppState.CumulativeStepsToUpdate += MazeRunSettings.StepsPerSeconds * frameTime;
            AppState.CumulativeStepsToUpdateSolving += MazeRunSettings.StepsPerSecondsSolving * frameTime;
            var stepsToUpdate = (int)AppState.CumulativeStepsToUpdate;
            var stepsToUpdateSolving = (int)AppState.CumulativeStepsToUpdateSolving;
            AppState.CumulativeStepsToUpdate -= stepsToUpdate;
            AppState.CumulativeStepsToUpdateSolving -= stepsToUpdateSolving;

            if (!AppState.Algorithm.IsFinished)
                AppState.Algorithm.Step(stepsToUpdate);
            else if (!AppState.SolvingAlgorithm.IsFinished)
                AppState.SolvingAlgorithm.Step(stepsToUpdateSolving + 1);

            // Draw
            Raylib.BeginDrawing();
            rlImGui.Begin();

            Raylib.ClearBackground(Color.Black);

            foreach (var cell in AppState.Maze.Cells)
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
                var cellRectangle = new Rectangle(leftX, topY, cellWidth, cellHeight);

                //Colors
                var noiseValue = noise.GetPerlin(cell.X * 2, cell.Y * 2, totalTime * 50);
                var noise0To1 = Raymath.Remap(noiseValue, -1, 1, 0, 1);
                var wallColor = new Color((int)(noise0To1 * 254), 0, 255);

                if (cell.IsCurrent && (!AppState.Algorithm.IsFinished || !AppState.SolvingAlgorithm.IsFinished))
                    Raylib.DrawRectangleRec(cellRectangle, Color.Orange);

                if (cell.HasBeenVisited)
                {
                    if (cell.HasLeftWall)
                        Raylib.DrawLineEx(topLeftCorner, bottomLeftCorner, 2, wallColor);

                    if (cell.HasTopWall)
                        Raylib.DrawLineEx(topLeftCorner, topRightCorner, 2, wallColor);

                    if (cell.HasRightWall)
                        Raylib.DrawLineEx(topRightCorner, bottomRightCorner, 2, wallColor);

                    if (cell.HasBottomWall)
                        Raylib.DrawLineEx(bottomLeftCorner, bottomRightCorner, 2, wallColor);
                }

                if (cell.IsOnStack && MazeRunSettings.VisualizeStack)
                {
                    var radius = cellWidth / 4;
                    Raylib.DrawCircleV(center, radius, Color.Yellow);
                }
            }

            MazeCell? previousCell = null;
            foreach (var currentBestCell in AppState.SolvingAlgorithm.CurrentBestPath)
            {
                if (previousCell == null)
                {
                    previousCell = currentBestCell;
                    continue;
                }

                var centerCurrent = new Vector2(
                    currentBestCell.X * cellWidth + halfCellWidth,
                    currentBestCell.Y * cellHeight + halfCellHeight);
                var centerPrevious = new Vector2(
                    previousCell.X * cellWidth + halfCellWidth,
                    previousCell.Y * cellHeight + halfCellHeight);

                Raylib.DrawLineEx(centerCurrent, centerPrevious, 1, Color.Yellow);
                previousCell = currentBestCell;
            }

            RenderGui(mazeWidthPixels, uiWidthPixels, height, MazeRunSettings);

            rlImGui.End();
            Raylib.EndDrawing();
        }
    }

    private void InitializeAppState(MazeRunSettings mazeRunSettings)
    {
        var maze = new Maze(mazeRunSettings.MazeWidth);
        IStepableAlgorithm algorithm = mazeRunSettings.Algortihm switch
        {
            DepthFirstRecursiveBacktrackingMazeAlgorithm.Name => new DepthFirstRecursiveBacktrackingMazeAlgorithm(maze),
            RandomizedPrimsMazeAlgorithm.Name => new RandomizedPrimsMazeAlgorithm(maze),
            _ => throw new NotSupportedException($"Algorithm {mazeRunSettings.Algortihm} is not supported.")
        };

        IMazeSolvingAlgorithm solvingAlgorithm = mazeRunSettings.SolvingAlgortihm switch
        {
            AStarPathFindingAlgorithm.Name => new AStarPathFindingAlgorithm(maze),
            KeepRightPathFindingAlgorithm.Name => new KeepRightPathFindingAlgorithm(maze),
            _ => throw new NotSupportedException($"Solving Algorithm {mazeRunSettings.SolvingAlgortihm} is not supported.")
        };

        AppState = new MazeAppState(maze, algorithm, solvingAlgorithm);
    }

    private void RenderGui(int mazeWidthPixels, int uiWidthPixels, int height, MazeRunSettings mazeRunSettings)
    {
        // GUI
        ImGui.SetNextWindowPos(new Vector2(mazeWidthPixels, 0));
        ImGui.SetNextWindowSize(new Vector2(uiWidthPixels, height));

        ImGuiWindowFlags windowFlags =
            ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoResize |
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoScrollbar |
            ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoSavedSettings |
            ImGuiWindowFlags.NoBringToFrontOnFocus;

        if (ImGui.Begin("RightPanel", windowFlags))
        {
            ImGui.Columns(2, "settingsColumns");

            //ImGui.NextColumn();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0, 255, 0, 1));
            ImGui.Text("FPS:");
            ImGui.NextColumn();
            ImGui.Text(Raylib.GetFPS().ToString());
            ImGui.PopStyleColor();

            ImGui.Separator();

            ImGui.NextColumn();
            ImGui.Text("Maze Width");
            ImGui.NextColumn();
            ImGui.InputInt("##mazeWidth", ref mazeRunSettings.MazeWidth);

            ImGui.NextColumn();
            ImGui.Text("Algorithm:");
            ImGui.NextColumn();
            if (ImGui.BeginCombo("##algorithm", "something?"))
            {
                if (ImGui.Selectable(DepthFirstRecursiveBacktrackingMazeAlgorithm.Name)) 
                    mazeRunSettings.Algortihm = DepthFirstRecursiveBacktrackingMazeAlgorithm.Name;

                if (ImGui.Selectable(RandomizedPrimsMazeAlgorithm.Name)) 
                    mazeRunSettings.Algortihm = RandomizedPrimsMazeAlgorithm.Name;

                ImGui.EndCombo();
            }

            ImGui.Separator();
            ImGui.NextColumn();
            if (ImGui.Button("Restart"))
            {
                InitializeAppState(mazeRunSettings);
            }
        }

        ImGui.End();
    }
}

public class MazeRunSettings
{
    public int MazeWidth = 100;
    public int StepsPerSeconds = 2000;
    public int StepsPerSecondsSolving = 400;
    public bool VisualizeStack = false;
    public string Algortihm = DepthFirstRecursiveBacktrackingMazeAlgorithm.Name;
    public string SolvingAlgortihm = KeepRightPathFindingAlgorithm.Name;
    
    public enum AlgorithmSpeed
    {
        Slow = 1,
        Normal = 2,
        Fast = 3,
        VeryFast = 4,
        Instant = 5
    }
}

public class MazeAppState(Maze maze, IStepableAlgorithm algorithm, IMazeSolvingAlgorithm solvingAlgorithm)
{
    public IStepableAlgorithm Algorithm { get; } = algorithm;
    public IMazeSolvingAlgorithm SolvingAlgorithm { get; } = solvingAlgorithm;
    public Maze Maze { get; } = maze;
    public float CumulativeStepsToUpdate { get; set; }
    public float CumulativeStepsToUpdateSolving { get; set; }
}